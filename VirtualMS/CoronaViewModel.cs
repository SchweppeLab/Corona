using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MStreamer;
using Nova.Data;
using Nova.IPC.Pipes;
using Nova.Io.Read;
using ThermoFisher.CommonCore.Data;

namespace VirtualMS
{
  public struct VMsStats
  {
    public int curScanCount;
    public int curScanMS1;
    public int curScanMS2;
    public int allScanCount;
    public int allScanMS1;
    public int allScanMS2;
    public int curCSHz;
    public int maxCSHz;
  }

  // Carries a freshly-arrived scan's already-prepared plot data out to the View so ScottPlot
  // mutation (which doesn't bind naturally) can stay in MainWindow's code-behind.
  public sealed record SpectrumUpdate(double[] X, double[] Y, double StartMz, double EndMz);

  public partial class CoronaViewModel : ObservableObject
  {
    private readonly SimRunner simMS = new();
    private readonly VMSServer server = new("Corona");

    private readonly DispatcherTimer meterTimer = new() { Interval = TimeSpan.FromMilliseconds(1) };

    private long lastTicks = 0;
    private volatile bool statsDirty = false;
    private VMsStats stats = new();

    private const int LogMaxLines = 1000;
    private const int LogTrimMargin = 100; // batch the trim so it isn't an O(n) operation on every log line
    private int logLineCount = 0;

    private int curSimCount = 0;
    private double curSimRT = 0;
    private TimeSpan allSimTime = TimeSpan.Zero;

    private int runSpeed = 1;
    private bool paused = false;
    private int runCounter = 0;
    private int nextPanel = 0;

    private readonly List<CustomScan> CustomScans = new();
    private CustomScan UserCustomScan = new();
    private int CustomScansIndex = -1;

    private readonly LinkedList<double> customScanQueue = new();

    private static readonly Brush MeterIdle = Brushes.Gray;
    private static readonly Brush MeterOff = Brushes.DarkGray;
    private static readonly Brush MeterOn = Brushes.Cyan;
    private static readonly Brush MeterBurst = Brushes.Magenta;

    public ObservableCollection<FileRow> FileList { get; } = new();
    public ObservableCollection<FileStats> FileQueue { get; } = new();
    public ObservableCollection<KeyValuePair<string, string>> CustomScanValues { get; } = new();
    public ObservableCollection<Brush> MeterSlots { get; } = new(Enumerable.Repeat(MeterIdle, 10));

    // Fired directly from whatever thread SimRunner.MsScanArrived fires on (a background thread) -
    // matches the original's un-marshaled ScottPlot.Sync-locked Add() calls. Do not wrap in a
    // Dispatcher call here; that would serialize scan arrival onto the UI thread and defeat the
    // point of ScottPlot's thread-safe append path.
    public event Action<double, double>? TicPointArrived;
    public event Action<SpectrumUpdate>? SpectrumReady;
    public event Action? TicResetRequested;
    public event Action? AcquisitionTicResetRequested;

    [ObservableProperty]
    private string logText = "";

    [ObservableProperty]
    private string runTimeText = "00:00\n00:00:00\n00:00:00\n0";

    [ObservableProperty]
    private string scanStatsText = "0        0\n0        0\n0        0";

    [ObservableProperty]
    private string curCSRateText = "0 Hz";

    [ObservableProperty]
    private string maxCSRateText = "0 Hz";

    [ObservableProperty]
    private string speedLabelText = "1x";

    [ObservableProperty]
    private int speedValue = 0;

    [ObservableProperty]
    private string spectrumLabelText = "Spectrum";

    [ObservableProperty]
    private string scanNumberLabelText = "ScanNumber";

    [ObservableProperty]
    private string customScanCountText = "0";

    [ObservableProperty]
    private string customScanRTText = "RT: 0";

    [ObservableProperty]
    private int customScanNumber = 0;

    [ObservableProperty]
    private int customScanMax = 0;

    [ObservableProperty]
    private bool clearCustomScansOnStart = true;

    [ObservableProperty]
    private int selectedTabIndex = 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadCommand))]
    private bool canLoad = true;

    [ObservableProperty]
    private bool canAdd = false;

    [ObservableProperty]
    private bool canDelete = false;

    [ObservableProperty]
    private bool canRemove = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private bool canRun = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PauseCommand))]
    private bool canPause = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    private bool canStop = false;

    public int FileListSelectionCount { get; set; }
    public int QueueSelectionCount { get; set; }

    public CoronaViewModel()
    {
      simMS.AcquisitionEnd += OnAcquisitionEnd;
      simMS.AcquisitionStart += OnAcquisitionStart;
      simMS.MsScanArrived += OnMsScanArrived;
      simMS.SpeedWarning += OnSpeedWarning;
      server.ClientConnected += OnClientConnected;
      server.ClientDisconnected += OnClientDisconnected;
      server.CustomScanRequest += OnCustomScanRequest;

      meterTimer.Tick += (s, e) =>
      {
        runCounter += (SpeedValue + 1);
        if (runCounter > 10)
        {
          runCounter -= 10;
          UpdateRunViewer();
        }
      };
      meterTimer.Start();

      AppendLog(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly()!.Location));
      AppendLog("Server running: " + server.IsRunning());

      lastTicks = DateTime.Now.Ticks;
    }

    // Called from MainWindow's 100ms plot-refresh timer tick (UI thread) alongside the ScottPlot
    // refresh it does directly - keeps stats formatting centralized here instead of scattered
    // through the View.
    public void RefreshStatsIfDirty()
    {
      if (!statsDirty) return;
      statsDirty = false;
      RefreshStats();
    }

    private void RefreshStats()
    {
      TimeSpan ts = simMS.ElapsedRunTime();
      string aa = ts.ToString(@"hh\:mm\:ss");
      TimeSpan totalTs = simMS.IsRunning ? allSimTime + ts : allSimTime;
      string bb = totalTs.ToString(@"hh\:mm\:ss");
      ts = TimeSpan.FromMinutes(curSimRT);
      string cc = string.Format("{0}:{1}:{2}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"), ts.Seconds.ToString("00"));
      RunTimeText = string.Format("{0}\n{1}\n{2}\n{3}", cc, aa, bb, curSimCount.ToString());

      string a = string.Format("{0}{1,9}", stats.curScanCount, stats.allScanCount);
      string b = string.Format("{0}{1,9}", stats.curScanMS1, stats.allScanMS1);
      string c = string.Format("{0}{1,9}", stats.curScanMS2, stats.allScanMS2);
      ScanStatsText = string.Format("{0}\n{1}\n{2}", a, b, c);

      CurCSRateText = stats.curCSHz.ToString() + " Hz";
      MaxCSRateText = stats.maxCSHz.ToString() + " Hz";
    }

    partial void OnSpeedValueChanged(int value)
    {
      int[] speeds = [1, 2, 5, 10, 20, 50, 100];
      runSpeed = speeds[value];
      SpeedLabelText = runSpeed.ToString() + "x";
      simMS.SetSpeed(runSpeed);
    }

    partial void OnSelectedTabIndexChanged(int value) => UpdateFileListButtonStates();

    public void UpdateFileListButtonStates()
    {
      if (SelectedTabIndex == 0)
      {
        CanAdd = FileListSelectionCount > 0;
        CanDelete = FileListSelectionCount > 0;
        CanLoad = true;
      }
      else
      {
        CanAdd = false;
        CanDelete = false;
        CanLoad = false;
      }
    }

    public void UpdateQueueButtonState() => CanRemove = QueueSelectionCount > 0;

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private void Load()
    {
      var dlg = new Microsoft.Win32.OpenFileDialog
      {
        Filter = "All MS files|*.mzML;*.mzXML;*.raw|MzML|*.mzML|MzXML|*.mzXML|Thermo Raw|*.raw",
        Multiselect = true
      };
      if (dlg.ShowDialog() != true) return;

      bool changeView = false;
      foreach (string s in dlg.FileNames)
      {
        FileReader fileReader = new(s);
        FileStats stats = new()
        {
          ScanCount = fileReader.ScanCount,
          HighScan = fileReader.LastScan,
          HighRT = fileReader.MaxRetentionTime,
          LowScan = fileReader.FirstScan,
          Name = System.IO.Path.GetFileName(s),
          Path = System.IO.Path.GetDirectoryName(s),
          FullPath = s,
        };
        FileList.Add(new FileRow(stats));
        changeView = true;
      }

      if (changeView) SelectedTabIndex = 0;
    }

    public void AddFilesToQueue(IEnumerable<FileRow> rows)
    {
      foreach (FileRow row in rows)
      {
        FileStats fs = new(row.Stats);
        if (row.RunSpan == "Spectrum")
        {
          fs.StartScan = Convert.ToInt32(row.Start);
          fs.EndScan = Convert.ToInt32(row.End);
          fs.StartRT = 0;
          fs.EndRT = 0;
        }
        else
        {
          fs.StartRT = Convert.ToDouble(row.Start);
          fs.EndRT = Convert.ToDouble(row.End);
          fs.StartScan = FindScan(fs.FullPath, fs.StartRT, true);
          fs.EndScan = FindScan(fs.FullPath, fs.EndRT, false);
          if (fs.StartScan == 0 || fs.EndScan == 0)
          {
            MessageBox.Show("Bad RT");
          }
        }
        FileQueue.Add(fs);
        if (!simMS.IsRunning) CanRun = true;
      }
    }

    public void DeleteFromFileList(IEnumerable<FileRow> rows)
    {
      foreach (FileRow row in rows.ToList()) FileList.Remove(row);
    }

    public void RemoveFromQueue(IEnumerable<int> selectedIndices)
    {
      foreach (int i in selectedIndices.OrderByDescending(x => x))
      {
        AppendLog("Removing index: " + i.ToString());
        if (i == 0 && simMS.IsRunning) simMS.Stop();
        else if (i >= 0 && i < FileQueue.Count) FileQueue.RemoveAt(i);
      }
    }

    [RelayCommand(CanExecute = nameof(CanRun))]
    private void Run()
    {
      if (simMS.IsRunning)
      {
        if (paused)
        {
          simMS.Resume();
          CanRun = false;
          CanPause = true;
          paused = false;
        }
      }
      else
      {
        if (FileQueue.Count == 0)
        {
          AppendLog("FileQueue is empty. Should not have been allowed to start simulation.");
          return;
        }

        stats = new VMsStats();
        TicResetRequested?.Invoke();
        AppendLog("Starting Virtal MS");

        while (FileQueue.Count > 0)
        {
          string s = FileQueue[0].FullPath;
          if (IsSupportedFile(s))
          {
            simMS.Run(FileQueue[0].FullPath, -1, FileQueue[0].StartScan, FileQueue[0].EndScan);
            CanPause = true;
            CanStop = true;
            CanRun = false;
            break;
          }
          else
          {
            PopQueue();
          }
        }
      }
    }

    private static bool IsSupportedFile(string s)
    {
      string ext = System.IO.Path.GetExtension(s);
      return File.Exists(s) && (ext == ".raw" || ext == ".mzML" || ext == ".mzXML");
    }

    [RelayCommand(CanExecute = nameof(CanPause))]
    private void Pause()
    {
      if (paused)
      {
        paused = false;
        simMS.Resume();
      }
      else
      {
        paused = true;
        simMS.Pause();
        CanRun = true;
        CanPause = false;
      }
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    private void Stop() => simMS.Stop();

    [RelayCommand]
    private void ResetStats()
    {
      curSimCount = 0;
      allSimTime = TimeSpan.Zero;
      stats = new VMsStats();
      RefreshStats();
    }

    [RelayCommand]
    private void SaveLog()
    {
      var dlg = new Microsoft.Win32.SaveFileDialog
      {
        Filter = "Text files|*.txt|All files|*.*",
        FileName = "Corona_Log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt"
      };
      if (dlg.ShowDialog() == true)
      {
        File.WriteAllText(dlg.FileName, LogText);
        AppendLog("Log saved to: " + dlg.FileName);
      }
    }

    [RelayCommand]
    private void ClearCustomScans()
    {
      CustomScanValues.Clear();
      CustomScans.Clear();
      CustomScanMax = 0;
      CustomScanNumber = 0;
      CustomScansIndex = -1;
      CustomScanRTText = "RT: " + UserCustomScan.RetentionTime.ToString("0.####");
      CustomScanCountText = CustomScans.Count.ToString();
    }

    [RelayCommand]
    private void CustomScanNext()
    {
      if (CustomScanMax == 0) return;
      CustomScanNumber = Math.Clamp(CustomScanNumber + 1, 1, CustomScanMax);
    }

    [RelayCommand]
    private void CustomScanPrevious()
    {
      if (CustomScanMax == 0) return;
      CustomScanNumber = Math.Clamp(CustomScanNumber - 1, 1, CustomScanMax);
    }

    partial void OnCustomScanNumberChanged(int value)
    {
      if (value <= 0 || value > CustomScans.Count) return;
      CustomScansIndex = value - 1;
      UserCustomScan = CustomScans[CustomScansIndex];
      UpdateCustomScans(true);
    }

    private void UpdateCustomScans(bool refreshDataSource)
    {
      if (refreshDataSource)
      {
        CustomScanValues.Clear();
        foreach (KeyValuePair<string, string> kv in UserCustomScan.Values) CustomScanValues.Add(kv);
        CustomScanRTText = "RT: " + UserCustomScan.RetentionTime.ToString("0.####");
      }
      CustomScanCountText = CustomScans.Count.ToString();
    }

    private void PopQueue()
    {
      if (FileQueue.Count == 0) return;
      FileQueue.RemoveAt(0);
    }

    private void UpdateRunViewer()
    {
      if (simMS.IsRunning)
      {
        if (SpeedValue < 3)
        {
          MeterSlots[nextPanel] = MeterOff;
          nextPanel++;
          if (nextPanel == 10) nextPanel = 0;
          MeterSlots[nextPanel] = MeterOn;
        }
        else
        {
          Random rnd = new();
          if (rnd.Next(SpeedValue) == 0)
          {
            MeterSlots[rnd.Next(10)] = MeterOff;
          }
          else
          {
            MeterSlots[rnd.Next(10)] = rnd.Next(2) == 0 ? MeterOn : MeterBurst;
          }
        }
      }
      else
      {
        for (int i = 0; i < MeterSlots.Count; i++) MeterSlots[i] = MeterOff;
      }
    }

    // Binary search for the scan number closest to the retention time, either before or after.
    private static int FindScan(string fn, double rt, bool after)
    {
      FileReader fr = new(fn);
      int first = fr.FirstScan;
      int last = fr.LastScan;
      int mid = (last + first) / 2;

      int sn;
      double snRT;
      if (after)
      {
        sn = 0;
        snRT = fr.ReadSpectrum("", last).RetentionTime;
      }
      else
      {
        sn = 0;
        snRT = 0;
      }

      Spectrum s;
      while (first < last)
      {
        s = fr.ReadSpectrum("", mid);
        while (s.ScanNumber == 0 && mid < last)
        {
          mid++;
          s = fr.ReadSpectrum("", mid);
        }
        if (s.ScanNumber == 0) return sn;

        if (s.RetentionTime < rt)
        {
          if (!after && s.RetentionTime > snRT)
          {
            snRT = s.RetentionTime;
            sn = s.ScanNumber;
          }
          first = mid + 1;
          mid = (last + first) / 2;
        }
        else if (s.RetentionTime > rt)
        {
          if (after && s.RetentionTime < snRT)
          {
            snRT = s.RetentionTime;
            sn = s.ScanNumber;
          }
          last = mid - 1;
          mid = (last + first) / 2;
        }
        else if (s.RetentionTime == snRT)
        {
          return s.ScanNumber;
        }
      }
      return sn;
    }

    private void OnAcquisitionEnd(RunInfo info)
    {
      server?.SendAcquisitionEnd(info);
      AppendLog("Acquisition ended: " + info.Name);

      CanRun = false;
      CanPause = false;
      CanStop = false;
      allSimTime += simMS.ElapsedRunTime();
      PopQueue();
      while (FileQueue.Count > 0)
      {
        string s = FileQueue[0].FullPath;
        if (IsSupportedFile(s))
        {
          simMS.Run(FileQueue[0].FullPath, -1, FileQueue[0].StartScan, FileQueue[0].EndScan);
          CanPause = true;
          CanStop = true;
          break;
        }
        else
        {
          PopQueue();
        }
      }
    }

    private void OnAcquisitionStart(RunInfo info)
    {
      server?.SendAcquisitionStart(info);
      if (ClearCustomScansOnStart) ClearCustomScans();

      AcquisitionTicResetRequested?.Invoke();
      curSimCount++;
      stats.curScanCount = 0;
      stats.curScanMS1 = 0;
      stats.curScanMS2 = 0;
      customScanQueue.Clear();
      AppendLog("Acquisition started: " + info.Name);
    }

    private void OnSpeedWarning(string message) => UiPost(() => AppendLog(message));

    private void OnClientConnected(PipesConnection pc) => UiPost(() => AppendLog("Client connected: " + pc.ID));

    private void OnClientDisconnected(PipesConnection pc) => UiPost(() => AppendLog("Client disconnected: " + pc.ID));

    private void OnCustomScanRequest(object? sender, CustomScan customScan)
    {
      // Stamp the request time immediately so marshaling delay doesn't skew the recorded RT / Hz calculation.
      customScan.RetentionTime = simMS.GetRT();
      UiPost(() =>
      {
        while (customScanQueue.Count > 0 && customScanQueue.First!.Value < customScan.RetentionTime * 60 - 1)
        {
          customScanQueue.RemoveFirst();
        }
        customScanQueue.AddLast(customScan.RetentionTime * 60);
        if (customScanQueue.Count > stats.maxCSHz)
        {
          stats.maxCSHz = customScanQueue.Count;
        }
        stats.curCSHz = customScanQueue.Count;

        CustomScans.Add(customScan);
        CustomScanMax = CustomScans.Count;
        if (CustomScans.Count == 1)
        {
          CustomScansIndex = 0;
          UserCustomScan = CustomScans[0];
          CustomScanNumber = 1;
        }
        UpdateCustomScans(false);
      });
    }

    private void OnMsScanArrived(object? sender, MStreamerEventArgs e)
    {
      SpectrumEx? scan = e.GetScan();
      if (scan == null) return;

      if (e.IsIAPI) server?.SendSpectrum(scan);
      if (!e.IsNative) return;

      double tic = scan.TotalIonCurrent;
      double rt = scan.RetentionTime;

      TicPointArrived?.Invoke(rt, tic);

      long curTicks = DateTime.Now.Ticks;
      long tickDif = curTicks - lastTicks;
      if (tickDif > 5e6)
      {
        double[] x;
        double[] y;
        if (!scan.Centroid)
        {
          x = new double[scan.Count];
          y = new double[scan.Count];
          for (int a = 0; a < scan.Count; a++)
          {
            x[a] = scan.DataPoints[a].Mz;
            y[a] = scan.DataPoints[a].Intensity / scan.BasePeakIntensity * 100;
          }
        }
        else
        {
          x = new double[scan.Count * 3 + 2];
          y = new double[scan.Count * 3 + 2];
          int a = 0;
          x[a] = scan.StartMz;
          y[a++] = 0;
          for (int i = 0; i < scan.DataPoints.Count(); i++)
          {
            x[a] = scan.DataPoints[i].Mz;
            y[a++] = 0;
            x[a] = scan.DataPoints[i].Mz;
            y[a++] = scan.DataPoints[i].Intensity / scan.BasePeakIntensity * 100;
            x[a] = scan.DataPoints[i].Mz;
            y[a++] = 0;
          }
          x[a] = scan.EndMz;
          y[a] = 0;
        }
        SpectrumReady?.Invoke(new SpectrumUpdate(x, y, scan.StartMz, scan.EndMz));

        string scanFilter = scan.ScanFilter;
        string scanLabel = "Scan #" + scan.ScanNumber.ToString() + "  RT:" + scan.RetentionTime.ToString();
        UiPost(() =>
        {
          SpectrumLabelText = scanFilter;
          ScanNumberLabelText = scanLabel;
        });
        lastTicks = curTicks;
      }

      curSimRT = scan.RetentionTime;
      stats.curScanCount++;
      stats.allScanCount++;
      if (scan.MsLevel == 1)
      {
        stats.curScanMS1++;
        stats.allScanMS1++;
      }
      else if (scan.MsLevel == 2)
      {
        stats.curScanMS2++;
        stats.allScanMS2++;
      }
      statsDirty = true;
    }

    private void AppendLog(string msg)
    {
      LogText += DateTime.Now.ToString("HH:mm:ss.fff") + "  " + msg + Environment.NewLine;
      logLineCount++;
      if (logLineCount > LogMaxLines + LogTrimMargin)
      {
        int firstLineToKeep = logLineCount - LogMaxLines;
        int idx = NthIndexOfNewLine(LogText, firstLineToKeep);
        if (idx >= 0) LogText = LogText[(idx + Environment.NewLine.Length)..];
        logLineCount = LogMaxLines;
      }
    }

    private static int NthIndexOfNewLine(string text, int n)
    {
      int idx = -1;
      for (int i = 0; i < n; i++)
      {
        idx = text.IndexOf(Environment.NewLine, idx + 1, StringComparison.Ordinal);
        if (idx < 0) return -1;
      }
      return idx;
    }

    // Marshals a property-touching action onto the UI thread. Scan arrival, custom-scan
    // requests, and client connect/disconnect all originate on background threads (SimRunner's
    // Task.Run and the Nova pipe server's I/O threads), which bound properties cannot be
    // touched from directly.
    private void UiPost(Action action)
    {
      Dispatcher dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
      if (dispatcher.CheckAccess()) action();
      else dispatcher.BeginInvoke(action);
    }
  }
}
