using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.IO;

namespace CC
{
  public class Usage
  {
    public static string Main
    { get { return GetResource("usage_main.txt"); } }

    public static string List
    { get { return GetResource("usage_list.txt"); } }

    public static string Tree
    { get { return GetResource("usage_tree.txt"); } }

    public static string Label
    { get { return GetResource("usage_label.txt"); } }

    public static string CS
    { get { return GetResource("usage_cs.txt"); } }

    private static string GetResource(string path)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = "CC.resources." + path;

      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      using (StreamReader reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }
  }
}
