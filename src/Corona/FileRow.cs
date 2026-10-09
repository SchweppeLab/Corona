using CommunityToolkit.Mvvm.ComponentModel;

namespace VirtualMS
{
  // Wraps a FileStats with the per-row "Run Span" editing state the Run Parameters grid needs
  // (Spectrum = scan numbers, Retention Time = minutes) - state that belongs to the grid row,
  // not to FileStats itself.
  public partial class FileRow : ObservableObject
  {
    public FileStats Stats { get; }

    public string Name => Stats.Name;
    public string? Path => Stats.Path;

    [ObservableProperty]
    private string runSpan = "Spectrum";

    [ObservableProperty]
    private string start = "0";

    [ObservableProperty]
    private string end = "0";

    public FileRow(FileStats stats)
    {
      Stats = stats;
      start = stats.LowScan.ToString();
      end = stats.HighScan.ToString();
    }

    partial void OnRunSpanChanged(string value)
    {
      if (value == "Spectrum")
      {
        Start = Stats.LowScan.ToString();
        End = Stats.HighScan.ToString();
      }
      else
      {
        Start = Stats.LowRT.ToString();
        End = Stats.HighRT.ToString();
      }
    }
  }
}
