using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC
{
	public class Usage
	{
		public static string Main
		{ get { return ResourceHandler.GetResource("usage_main.txt"); } }

		public static string List
		{ get { return ResourceHandler.GetResource("usage_list.txt"); } }

    public static string Tree
    { get { return ResourceHandler.GetResource("usage_tree.txt"); } }

    public static string Label
    { get { return ResourceHandler.GetResource("usage_label.txt"); } }

	}
}
