using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMS
{
  public sealed class FileStats
  {
    public int LowScan { get; set; } = 0;
    public int HighScan { get; set; } = 0;
    public int ScanCount { get; set; } = 0;
    public double LowRT { get; set; } = 0;
    public double HighRT { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string? Path { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public int StartScan { get; set; } = 0;
    public int EndScan { get; set; } = 0;
    public double StartRT { get; set; } = 0;
    public double EndRT { get; set; } = 0;

    public FileStats() { }

    public FileStats(FileStats fs)
    {
      LowScan = fs.LowScan;
      HighScan = fs.HighScan;
      ScanCount = fs.ScanCount;
      LowRT = fs.LowRT;
      HighRT = fs.HighRT;
      Name = fs.Name;
      Path = fs.Path;
      FullPath = fs.FullPath;   
      StartScan = fs.StartScan;
      EndScan = fs.EndScan;
      StartRT = fs.StartRT;
      EndRT = fs.EndRT;
    }

    public override string ToString()
    {
      return Name.ToString();
    }
  }
}
