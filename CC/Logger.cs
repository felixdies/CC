﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CC
{
  public class Logger
  {
    public Logger(string logPath)
    {
      this.LogPath = logPath;
    }

    private string LogPath { get; set; }

    public void Write(string text)
    {
      // replace CR to CRLF
      string beautifiedText = text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n  ");	// add spaces
      AppendAllText(beautifiedText);
    }

    public void WriteCommand(string command, DateTime time)
    {
      string text = ">>> " + time.ToString("yy-MM-dd HH:mm:ss") + " " + command + Environment.NewLine;
      AppendAllText(text);
    }

    private void AppendAllText(string log)
    {
      if (!File.Exists(LogPath))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
        File.Create(LogPath).Close();
      }

      File.AppendAllText(LogPath, log);
    }
  }
}
