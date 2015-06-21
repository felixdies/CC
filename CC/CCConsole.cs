using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC
{
	public class CCConsole
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				WriteLine(Usage.Main);
				return;
			}

			switch (args[0].ToLower())
			{
				case "list":
					if (args.Length < 2)
					{
						WriteLine(Usage.List);
						return;
					}
					new ClearTool(Environment.CurrentDirectory, args[1]).ListCCFilesOnBranch()
						.ForEach(file => WriteLine(file));
					break;

				case "tree":
					if (args.Length < 2)
					{
						WriteLine(Usage.Tree);
						return;
					}
					new ClearTool(Environment.CurrentDirectory, args[1]).ViewCCVersionTrees();
					break;

				case "label":
					Label(args);
					break;

				case "cs":
					ConfigSpec(args);
					break;

				default:
					WriteLine(Usage.Main);
					return;
			}
		}

		static private void Label(string[] args)
		{
			if (args.Length < 4)
			{
				WriteLine(Usage.Label);
				return;
			}

			string labeledBranch;

			switch (args[1].ToLower())
			{
				case "-main":
				case "-m":
					labeledBranch = "main";
					break;

				case "-branch":
				case "-b":
					labeledBranch = "main\\" + args[2];
					break;

				default:
					WriteLine(Usage.Label);
					return;
			}

			new ClearTool(Environment.CurrentDirectory, args[2]).LabelLastElements(labeledBranch, args[3]);
		}

		static private void ConfigSpec(string[] args)
		{
			switch (args.Length)
			{
				case 2:
					WriteLine(new ClearTool(Environment.CurrentDirectory).CatCS());
					break;

				case 3:
					new ClearTool(Environment.CurrentDirectory, args[2]).SetBranchCS();
					break;

				default:
					return;
			}
		}

		static void WriteLine(string value)
		{
			System.Console.WriteLine(value);
		}
	}
}
