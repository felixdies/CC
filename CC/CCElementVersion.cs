using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CC
{
  public class CCElementVersion
  {
    public CCElementVersion()
    {
    }

    public CCElementVersion(string versionInfo)
    {
      ParseFileInfo(versionInfo);
    }

    public string RootPath { get; set; }

    public string Attributes { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public string EventDescription { get; set; }
    public string CheckoutInfo { get; set; }
    public string HostName { get; set; }
    public string IndentLevel { get; set; }
    public string Labels { get; set; }
    public string ObjectKind { get; set; }
    /// <summary> Rootpath 로부터의 상대 경로 </summary>
    public string ElementName { get; set; }
    public string Version { get; set; }
    public string PredecessorVersion { get; set; }
    public string Operation { get; set; }
    public string Type { get; set; }
    public string SymbolicLinkAbsPath { get; set; }
    public string OwnerLoginName { get; set; }
    public string OwnerFullName { get; set; }
    public string HyperLinkInfo { get; set; }

    private string Branch
    {
      get
      {
        string[] elemArr = Version.Split(new char[] { '\\', '/' });
        return elemArr[elemArr.Length - 2];
      }
    }

    private CCElementVersion Predecessor { get; set; }
    private CCElementVersion HyperLinkedFrom { get; set; }
    private CCElementVersion HyperLinkedTo { get; set; }

    private void ParseFileInfo(string versionInfo)
    {
      List<string> versionInfoList = versionInfo.Split('|').ToList();
      Dictionary<string, string> versionInfoDic = new Dictionary<string, string>();

      foreach (string info in versionInfoList)
      {
        int i = info.IndexOf('=');
        if (i < 0 || i == info.Length - 1)
          continue;

        string key = info.Substring(0, i);
        string value = info.Substring(i + 1, info.Length - (i + 1));
        versionInfoDic.Add(key, value);
      }

      foreach (KeyValuePair<string, string> pair in versionInfoDic)
      {
        System.Reflection.PropertyInfo propertyInfo = this.GetType().GetProperty(pair.Key);
        if (propertyInfo != null && propertyInfo.PropertyType == typeof(string))
          propertyInfo.SetValue(this, pair.Value);
      }

      CreatedDate = DateTime.Parse(versionInfoDic["CreatedDate"]);

      if (versionInfoDic.ContainsKey("SymbolicLink"))
        SymbolicLinkAbsPath = Path.GetFullPath((new Uri(Path.Combine(RootPath, versionInfoDic["SymbolicLink"]))).LocalPath);
    }
  }
}
