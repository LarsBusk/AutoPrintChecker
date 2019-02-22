using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace AutoPrintChecker
{
  class Program
  {
    private static readonly ILog log = log4net.LogManager.GetLogger(typeof(Program));

    private const string FileName = "PrintOutInfo.csv";
    private static DateTime receiveTime;

    static void Main(string[] args)
    {
      FileSystemWatcher watcher = new FileSystemWatcher(Properties.Settings.Default.PathToCheck);
      CreateCsvFile(FileName);
      watcher.Created += Watcher_Created;
      log.InfoFormat("Monitoring {0}", Properties.Settings.Default.PathToCheck);
      log.Debug("Start monitoring");
      watcher.EnableRaisingEvents = true;
      while (true)
      {
        Thread.Sleep(300);
      }
    }

    private static void Watcher_Created(object sender, FileSystemEventArgs e)
    {
      receiveTime = DateTime.UtcNow;
      log.InfoFormat("New file detected {0}", e.Name);
      Thread.Sleep(200);
      string[] lines = File.ReadAllLines(e.FullPath);
      log.Info("Writing to csvfile.");
      WriteToFile(lines);
      log.Info("Lines written, deleting file");

      File.Delete(e.FullPath);
      log.Info("File is deleted.");
    }


    private static void CreateCsvFile(string fileName)
    {
      if (!File.Exists(FileName))
      {
        File.AppendAllLines(FileName, new[] {"SampleId;SampleNumber;SampleStartUtc;SampleEndUtc;ReceivedUtc"});
      }
    }

    private static void WriteToFile(string[] lines)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append(lines[0]);
      builder.Append(";");
      builder.Append(receiveTime.ToString("yyyy'-'MM'-'dd hh':'mm':'ss','fff"));

      var outString = builder.ToString().Replace("\"", "").Replace(",", ";").Replace(".", ",").Replace("T", " ");

      File.AppendAllLines(FileName, new[] {outString});
    }
  }
}
