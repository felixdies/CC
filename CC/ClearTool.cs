using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace CC
{
	public class ClearTool
	{
		public ClearTool(string executingPath)
		{
			this.ExecutingPath = executingPath;
		}

		public ClearTool(string executingPath, string branchName)
		{
			this.ExecutingPath = executingPath;
			this.BranchName = branchName;
		}

		private string ExecutingPath { get; set; }
		private string BranchName { get; set; }
		private string OutPath
		{ 
			get { return Path.Combine(ExecutingPath, "ccout.txt"); } 
		}
		private string LogPath
		{
			get { return Path.Combine(ExecutingPath, "cclog.txt"); } 
		}
		private string VobPath { get; set; }

		private string Fmt
		{
			get
			{
				return "'"
				+ "Attributes=%a"
				+ "|Comment=%Nc"
				+ "|CreatedDate=%d"
				+ "|EventDescription=%e"
				+ "|CheckoutInfo=%Rf"
				+ "|HostName=%h"
				+ "|IndentLevel=%i"
				+ "|Labels=%l"
				+ "|ObjectKind=%m"
				+ "|ElementName=%En"
				+ "|Version=%Vn"
				+ "|PredecessorVersion=%PVn"
				+ "|Operation=%o"
				+ "|Type=%[type]p"
				+ "|SymbolicLink=%[slink_text]p"
				+ "|OwnerLoginName=%[owner]p"
				+ "|OwnerFullName=%[owner]Fp"
				+ "|HyperLink=%[hlink]p"
				+ "\n'";
			}
		}

		public List<string> ListCCFilesOnBranch()
		{
			return FindAllFilesInBranch();
		}

		public void ViewCCVersionTrees()
		{
			FindAllFilesInBranch().ForEach(filePath => ViewVersionTree(filePath));
		}

		public void LabelLastElements(string labeledBranch, string label)
		{
			// todo: validate label

			FindAllFilesInBranch()
				.Where(filePath => IsLabelingTargetExtension(filePath)).ToList()
				.ForEach(filePath => LabelLatestElement(filePath, labeledBranch, label));
		}

		public List<string> CatCS()
		{
			return GetExecutedResultList("catcs");
		}

		private bool IsLabelingTargetExtension(string filePath)
		{
			List<string> targetExtension = new List<string>(new string[] { ".aspx", ".ascx", ".js", ".sql" });
			return targetExtension.Contains(Path.GetExtension(filePath.Split('@')[0]));
		}

		private string CurrentView
		{
			get { return GetExecutedResult("pwv").Split(' ')[3]; }
		}

		private string LogInUser
		{
			get
			{
				List<string> viewInfo = GetExecutedResultList("lsview -long " + CurrentView);
				return viewInfo.Find(info => info.StartsWith("View owner")).Split(' ')[2];
			}
		}

		private string LogInUserName
		{
			get { return LogInUser.Split('\\').Last(); }
		}

		private void CheckCheckedoutFileIsNotExist()
		{
			List<string> checkedoutFileList = LscheckoutInCurrentViewByLoginUser();
			if (checkedoutFileList.Count > 0)
			{
				string message =
					"체크아웃 된 파일이 있습니다." + Environment.NewLine
					+ string.Join(Environment.NewLine, checkedoutFileList);
				throw new CCException(message);
			}
		}

		private void CheckAllSymbolicLinksAreMounted()
		{
			List<CCElementVersion> slinkList = FindAllSymbolicLinks();

			foreach (CCElementVersion link in slinkList)
			{
				if (!System.IO.Directory.Exists(link.SymbolicLinkAbsPath))
					throw new CCException(link.SymbolicLinkAbsPath + " VOB 이 mount 되지 않았습니다.");
			}
		}

		private void SetCS(string[] configSpec)
		{
			File.WriteAllLines(OutPath, configSpec);
			Execute("setcs " + OutPath);
		}

		private void SetBranchCS()
		{
			string[] branchCS = new string[]{
				"element * CHECKEDOUT"
				, "element -dir * /main/LATEST"
				, "element -file * /main/" + BranchName + "/LATEST"
				, "element -file * /main/LATEST -mkbranch " + BranchName
			};

			SetCS(branchCS);
		}

		private void SetBranchCS(DateTime time)
		{
			string[] branchCS = new string[] {
				"time " + time.ToString()
				, "element * CHECKEDOUT"
				, "element -dir * /main/LATEST"
				, "element -file * /main/" + BranchName + "/LATEST"
				, "element -file * /main/LATEST -mkbranch " + BranchName
				, "end time"
			};

			SetCS(branchCS);
		}

		private void SetDefaultCS()
		{
			Execute("setcs -default");
		}

		private List<string> FindAllFilesInBranch()
		{
      string args = string.Empty;
			
			if (!string.IsNullOrWhiteSpace(BranchName))
				args += " -branch 'brtype(" + BranchName + ")'";

			return GetExecutedResultList("find . " + args + " -print");
		}

		private List<string> FindAllFilesInBranch(DateTime since, DateTime until)
		{
			string args = string.Empty;

			if (!string.IsNullOrWhiteSpace(BranchName))
				args += " -branch 'brtype(" + BranchName + ")'";

			args += " -version '{created_since(" + since.AddSeconds(1).ToString() + ") && !created_since(" + until.AddSeconds(1).ToString() + ")}'";

			return GetExecutedResultList("find . " + args + " -print");
		}

    private void ViewVersionTree(string filePath)
    {
			Execute("lsvtree -graphical " + filePath + "\\LATEST", false);
    }

    private void LabelLatestElement(string filePath, string branch, string label)
    {
			Execute("mklabel -replace -version \\" + branch + "\\LATEST " + label + " " + filePath, false);
    }

		private List<CCElementVersion> FindAllSymbolicLinks()
		{
			List<CCElementVersion> resultSLinkList = new List<CCElementVersion>();
			List<string> foundSLinkList;

			foundSLinkList = GetExecutedResultList("find " + VobPath + " -type l -print");

			foundSLinkList.ForEach(link => resultSLinkList.Add(Describe(link)));
			
			return resultSLinkList;
		}

		private List<CCElementVersion> Lshistory(string pname)
		{
			List<CCElementVersion> resultList = new List<CCElementVersion>();

			GetExecutedResultList("lshistory -fmt " + Fmt + " " + pname)
				.ForEach(elemVersion => resultList.Add(
					new CCElementVersion(elemVersion) { RootPath = this.ExecutingPath })
					);

			return resultList;
		}

		private List<CCElementVersion> Lshistory(string pname, DateTime since)
		{
			List<CCElementVersion> resultList = new List<CCElementVersion>();

			GetExecutedResultList("lshistory -fmt " + Fmt + " -since" + since.AddSeconds(1) + " " + pname)
				.ForEach(elemVersion => resultList.Add(
					new CCElementVersion(elemVersion) { RootPath = this.ExecutingPath })
					);

			return resultList;
		}

		private CCElementVersion Describe(string pname)
		{
			string description = GetExecutedResult("describe -fmt " + Fmt + " " + pname);
			return new CCElementVersion(description) { RootPath = this.ExecutingPath }; 
		}

		private string Pwd()
		{
			return GetExecutedResult("pwd");
		}

		private List<string> LscheckoutInCurrentViewByLoginUser()
		{
			return GetExecutedResultList("lscheckout -short -cview -me -recurse");
		}

		// vob path 의 상위 디렉터리에서 mount 를 실행해야 한다.
		private void Mount(string vobTag)
		{
			Execute("mount \\" + vobTag);
		}

		// vob path 의 상위 디렉터리에서 umount 를 실행해야 한다.
		private void UMount(string vobTag)
		{
			Execute("umount \\" + vobTag);
		}

		private void Checkout(string pname)
		{
			Execute("checkout -ncomment " + pname);
		}

		private void Uncheckout(string pname)
		{
			Execute("uncheckout -keep " + pname);
		}

		private string Command
		{
			get {
				ProcessStartInfo proInfo = new ProcessStartInfo()
				{
					WorkingDirectory = ExecutingPath,
					FileName = "rcleartool",
					CreateNoWindow = true,
					UseShellExecute = false,
				};

				try
				{
					Process.Start(proInfo); // check if the user uses CCRC
				}
				catch
				{
					return "cleartool";
				}

				return "rcleartool";
			}
		}

		private void Execute(string arg, bool wait = true)
		{
			Process proc = new Process();
			ProcessStartInfo proInfo = new ProcessStartInfo()
			{
				WorkingDirectory = ExecutingPath,
				FileName = @"powershell",
				Arguments = Command + " " + arg,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardError = true
			};

			proc.StartInfo = proInfo;
			proc.Start();
			new Logger(LogPath).WriteCommand(proInfo.Arguments, DateTime.Now);

			if (wait)
			{
				using (StreamReader errReader = proc.StandardError)
				{
					string err = errReader.ReadToEnd(); // wait for exit
					if (!string.IsNullOrWhiteSpace(err))
						new Logger(LogPath).Write(err);
				}
			}
		}

		private string GetExecutedResult(string arg)
		{
			Execute(arg + " > '" + OutPath + "'");
			return File.ReadAllText(OutPath);
		}

		private List<string> GetExecutedResultList(string arg)
		{
			Execute(arg + " > '" + OutPath + "'");
			return File.ReadAllLines(OutPath).ToList();
		}
	}
}
