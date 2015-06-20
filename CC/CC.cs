using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CC
{
	public class CC
	{
		public CC(string cwd)
		{
			this.CWD = cwd;
		}

		internal string CWD { get; set; }
		internal string VobPath { get; set; }
		internal string BranchName { get; set; }
		internal string RepoPath { get; set; }

		ClearToolConstructInfo CCInfo
		{
			get
			{
				return new ClearToolConstructInfo()
				{
					BranchName = this.BranchName,
					ExecutingPath = this.CWD,
					OutPath = Path.Combine(this.CWD, "ccout.txt"),
					LogPath = Path.Combine(this.CWD, "cclog.txt")
				};
			}
		}
		
		public List<string> ListCCFilesOnBranch(string branchName)
		{
			this.BranchName = branchName;
			ClearTool cc = new ClearTool(CCInfo);

			return cc.FindAllFilesInBranch();
		}

		public void ViewCCVersionTrees(string branchName)
		{
			this.BranchName = branchName;
			ClearTool cc = new ClearTool(CCInfo);

			cc.FindAllFilesInBranch().ForEach(filePath => cc.ViewVersionTree(filePath));
		}

		public void LabelLastElements(string searchedBranch, string labeledBranch, string label)
		{
			// todo: validate label

			this.BranchName = searchedBranch;
			ClearTool cc = new ClearTool(CCInfo);

			cc.FindAllFilesInBranch()
				.Where(filePath => IsLabelingTargetExtension(filePath)).ToList()
				.ForEach(filePath => cc.LabelLatestElement(filePath, labeledBranch, label));
		}

		internal bool IsLabelingTargetExtension(string filePath)
		{
			List<string> targetExtension = new List<string>(new string[] { ".aspx", ".ascx", ".js", ".sql" });
			return targetExtension.Contains(Path.GetExtension(filePath.Split('@')[0]));
		}
	}
}
