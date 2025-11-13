using MSim;
using MSim.lib;
using ScottPlot;
using ScottPlot.Colormaps;
using ScottPlot.Plottables;
using Nova.Data;
using Nova.IPC.Pipes;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;
using Nova.Io.Read;
using ThermoFisher.CommonCore.Data;
using System.Collections.Generic;

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
    public VMsStats()
    {
      curScanCount = 0;
      curScanMS1 = 0; 
      curScanMS2 = 0; 
      allScanCount = 0;
      allScanMS1 = 0;
      allScanMS2 = 0;
    }
  }

  public partial class Corona : Form
  {
    SimRunner simMS = new SimRunner();
    VMSServer server = new VMSServer("Corona");
    readonly System.Windows.Forms.Timer UpdatePlotTimer = new() { Interval = 100, Enabled = true };
    readonly System.Windows.Forms.Timer UpdateRunTimer = new() { Interval = 1, Enabled = true };
    int runCounter = 0;
    int nextPanel = 0;

    private DataLogger plot;
    readonly Scatter plotSpec;

    private long lastTicks = 0;
    private volatile bool refreshSpectrum = false;
    private VMsStats stats = new VMsStats();

    private int curSimCount = 0;
    private double curSimRT = 0;
    private TimeSpan allSimTime = TimeSpan.Zero;

    private int runSpeed = 1;

    private bool paused = false;

    //private List<string> Files = new List<string>();
    private List<CustomScan> CustomScans = new List<CustomScan>();
    private CustomScan UserCustomScan = new CustomScan();
    private int CustomScansIndex = -1;

    private List<FileStats> FileList = new List<FileStats>();
    private List<FileStats> FileQueue = new List<FileStats>();

    private List<Panel> PanelList = new List<Panel>();

    public Corona()
    {
      InitializeComponent();
      dgvFiles.DefaultCellStyle.Font = new Font("Segoe", 8);

      plotTIC.UserInputProcessor.Disable();
      plot = plotTIC.Plot.Add.DataLogger();

      plot.Add(0, 0);
      plotTIC.Refresh();

      Coordinates[] co = {
        new (1, 1),
        new (2, 2)
      };

      plotSpec = plotSpectrum.Plot.Add.Scatter(co);
      plotSpec.MarkerSize = 1;
      plotSpectrum.Plot.Axes.SetupMultiplierNotation(plotSpectrum.Plot.Axes.Left);
      plotSpectrum.Plot.YLabel("Intensity");
      plotSpectrum.Plot.HideGrid();
      plotSpectrum.Refresh();

      //myScat = plotTIC.Plot.Add.Scatter(plotX, plotY);
      //myScat.MarkerSize = 1;


      //plotTIC.Plot.HideGrid();
      plotTIC.Plot.Title("TIC");
      plotTIC.Plot.XLabel("Retention Time (min)");
      plotTIC.Plot.YLabel("TIC");
      plotTIC.Plot.HideGrid();

      //plotTIC.Plot.Axes.SetupMultiplierNotation(plotTIC.Plot.Axes.Left);
      //plotTIC.Plot.Axes.AutoScale();

      UpdatePlotTimer.Tick += (s, e) =>
      {
        if (plot.HasNewData) plotTIC.Refresh();
        if (refreshSpectrum)
        {
          plotSpectrum.Refresh();
          refreshSpectrum = false;
        }
      };

      UpdateRunTimer.Tick += (s, e) =>
      {
        runCounter += (tbSpeed.Value + 1);
        if (runCounter > 10)
        {
          runCounter -= 10;
          UpdateRunViewer();
        }
      };

      simMS.AcquisitionEnd += OnAcquisitionEnd;
      simMS.AcquisitionStart += OnAcquisitionStart;

      simMS.MsScanArrived += OnMsScanArrived;
      server.ClientConnected += OnClientConnected;
      server.ClientDisconnected += OnClientDisconnected;
      server.CustomScanRequest += OnCustomScanRequest;

      dgvCustomScan.DataSource = null;

      fileListBox.DataSource = FileQueue;
      fileListBox.DisplayMember = "Name";
      fileListBox.ValueMember = "FullPath";

      PanelList.Add(pan1);
      PanelList.Add(pan2);
      PanelList.Add(pan3);
      PanelList.Add(pan4);
      PanelList.Add(pan5);
      PanelList.Add(pan6);
      PanelList.Add(pan7);
      PanelList.Add(pan8);
      PanelList.Add(pan9);
      PanelList.Add(pan10);

      log(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
      log("Server running: " + server.IsRunning());

      lastTicks = DateTime.Now.Ticks;

    }

    private void tsbAdd_Click(object sender, EventArgs e)
    {
      //Sometimes C# drives me crazy. dgvFiles.SelectedRows enumerates in reverse,
      //so push them onto a stack, then enumerate the stack...
      Stack<DataGridViewRow> forwardRows = new Stack<DataGridViewRow>();
      foreach (DataGridViewRow row in dgvFiles.SelectedRows)
      {
        forwardRows.Push(row);
      }
      foreach (DataGridViewRow row in forwardRows)
      {
        FileStats fs = new FileStats(FileList[row.Index]);
        if (row.Cells[2].Value.ToString() == "Spectrum")
        {
          fs.StartScan = Convert.ToInt32(row.Cells[3].Value);
          fs.EndScan = Convert.ToInt32(row.Cells[4].Value);
          fs.StartRT = 0;
          fs.EndRT = 0;
        }
        else
        {
          fs.StartRT = Convert.ToDouble(row.Cells[3].Value);
          fs.EndRT = Convert.ToDouble(row.Cells[4].Value);
          fs.StartScan = FindScan(fs.FullPath, fs.StartRT, true);
          fs.EndScan = FindScan(fs.FullPath, fs.EndRT, false);
          if (fs.StartScan == 0 || fs.EndScan == 0)
          {
            MessageBox.Show("Bad RT");
          }
          //MessageBox.Show("RT: " + fs.StartScan + " " + fs.EndScan);
        }
        FileQueue.Add(fs);
        if (!simMS.IsRunning) tsbRun.Enabled = true;
      }
      fileListBox.DataSource = null;
      fileListBox.DataSource = FileQueue;
    }

    private void tsbRemove_Click(object sender, EventArgs e)
    {
      Stack<int> indexes = new Stack<int>();
      foreach (int i in fileListBox.SelectedIndices) indexes.Push(i);
      foreach (int i in indexes)
      {
        log("Removing index: " + i.ToString());
        if (i == 0 && simMS.IsRunning)
        {
          simMS.Stop();
        }
        else FileQueue.RemoveAt(i);
      }
      fileListBox.DataSource = null;
      fileListBox.DataSource = FileQueue;
    }

    private void tsbRun_Click(object sender, EventArgs e)
    {
      if (simMS.IsRunning)
      {
        if (paused)
        {
          simMS.Resume();
          tsbRun.Enabled = false;
          tsbPause.Enabled = true;
          paused = false;
        }
      }
      else
      {
        if (FileQueue.Count == 0)
        {
          log("FileQueue is empty. Should not have been allowed to start simulation.");
          return;
        }

        stats = new VMsStats();
        plot.Clear();
        plot.Add(0, 0);
        plotTIC.Refresh();
        log("Starting Virtal MS");

        while (FileQueue.Count > 0)
        {
          string s = FileQueue[0].FullPath;
          if (File.Exists(s) && (Path.GetExtension(s) == ".raw" || Path.GetExtension(s) == ".mzML" || Path.GetExtension(s) == ".mzXML"))
          {
            simMS.Run(FileQueue[0].FullPath, -1, FileQueue[0].StartScan, FileQueue[0].EndScan);
            tsbPause.Enabled = true;
            tsbStop.Enabled = true;
            tsbRun.Enabled = false;
            break;
          }
          else
          {
            PopQueue();
          }
        }
      }
    }

    private void Acquisition_AcquisitionStreamClosing(object sender, EventArgs e)
    {
      log("Stream closing: " + e.ToString());
    }

    //private void Acquisition_AcquisitionStreamOpening(object sender, AcquisitionOpeningEventArgs e)
    //{
    //  log("Stream opening: " + e.ToString());
    //}

    //Binary search for the scan number closest to the retention time, either before or after.
    private int FindScan(string fn, double rt, bool after)
    {
      FileReader fr = new FileReader(fn);
      int first = fr.FirstScan;
      int last = fr.LastScan;
      int count = fr.ScanCount;
      int mid = (last + first) / 2;

      //initialize the closest scan to be the limits.
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
        //have to account for gaps in the scan series
        s = fr.ReadSpectrum("", mid);
        while (s.ScanNumber == 0 && mid < last)
        {
          mid++;
          s = fr.ReadSpectrum("", mid);
        }
        if (s.ScanNumber == 0) return sn;

        //check RT and keep going
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
      //Return sn.
      return sn;
    }

    private void OnAcquisitionEnd(RunInfo info)
    {
      server?.SendAcquisitionEnd(info);
      log("Acquisition ended: " + info.Name);

      tsbRun.Enabled = false;
      tsbPause.Enabled = false;
      tsbStop.Enabled = false;
      allSimTime += simMS.ElapsedRunTime();
      PopQueue();
      while (FileQueue.Count > 0)
      {
        string s = FileQueue[0].FullPath;
        if (File.Exists(s) && (Path.GetExtension(s) == ".raw" || Path.GetExtension(s) == ".mzML" || Path.GetExtension(s) == ".mzXML"))
        {
          simMS.Run(FileQueue[0].FullPath, -1, FileQueue[0].StartScan, FileQueue[0].EndScan);
          tsbPause.Enabled = true;
          tsbStop.Enabled = true;
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
      if(cbClearCS.Checked) ClearCustomScans();
      plot.Clear();
      plot.Add(0, 0);
      plotTIC.Plot.Axes.SetLimitsY(0, 1);
      plotTIC.Refresh();
      curSimCount++;
      log("Acquisition started: " + info.Name);
    }

    private void OnClientConnected(PipesConnection pc)
    {
      log("Client connected: " + pc.ID);
    }

    private void OnClientDisconnected(PipesConnection pc)
    {
      log("Client disconnected: " + pc.ID);
    }

    private void OnCustomScanRequest(object? sender, CustomScan customScan)
    {
      customScan.RetentionTime = simMS.GetRT();
      //log("Custom Scan Requested at: " + customScan.RetentionTime.ToString());
      CustomScans.Add(customScan);
      nudCustomScans.Maximum = CustomScans.Count;
      if (CustomScans.Count == 1)
      {
        nudCustomScans.Minimum = 1;
        CustomScansIndex = 0;
        UserCustomScan = CustomScans[0];  
      }
      UpdateCustomScans(false);
    }

    private void OnMsScanArrived(object? sender, MSimEventArgs e)
    {
      //log("Scan arrived");
      SpectrumEx scan = e.GetScan();
      if (scan != null)
      {
        //IAPI formatted scans get broadcast.
        if (e.IsIAPI) server?.SendSpectrum(scan);
        
        //Only process scans formatted as read from file
        if (!e.IsNative) return;

        double tic = scan.TotalIonCurrent;
        int scanNumber = scan.ScanNumber;
        double rt = scan.RetentionTime;

        lock (plotTIC.Plot.Sync)
        {
          plot.Add(rt, tic);
          plotTIC.Plot.Axes.SetupMultiplierNotation(plotTIC.Plot.Axes.Left);
        }

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
              y[a] = scan.DataPoints[a].Intensity / scan.BasePeakIntensity * 100; ;
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
              //Log(qs.mz[i].ToString()+" " + (qs.intensity[i] / qs.maxIntensity * 100).ToString());
              //centroided peaks need to be plotted with 3 points 
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
          lock (plotSpectrum.Plot.Sync)
          {
            plotSpectrum.Plot.Clear();
            var scat = plotSpectrum.Plot.Add.Scatter(x, y);
            scat.MarkerSize = 1;
            plotSpectrum.Plot.Axes.SetLimitsX(scan.StartMz, scan.EndMz);
            plotSpectrum.Plot.Axes.SetLimitsY(0, 100);
            //plotSpectrum.Plot.Axes.AutoScale();
          }
          labelSpectrum.Text = scan.ScanFilter;
          labelScanNumber.Text = "Scan #" + scan.ScanNumber.ToString() + "  RT:" + scan.RetentionTime.ToString();
          refreshSpectrum = true;
          lastTicks = curTicks;
        }

        //update statistics
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
        RefreshStats();
      }

      //Scan testScan = (Scan)e.GetScan();
      //IMsScan testScan_I = e.GetScan();
      //Console.WriteLine(testScan.ScanNumber + " : " + testScan_I.Header["ScanNumber"]);
      //Console.WriteLine(">>>>######################");
      //foreach (KeyValuePair<string, string> kvp in testScan_I.Header)
      //{
      //  Console.WriteLine(kvp.Key + ": " + kvp.Value);
      //}
      //Console.WriteLine("######################");
      //foreach (string item in testScan_I.Trailer.ItemNames.ToList())
      //{
      //  testScan_I.Trailer.TryGetValue(item, out string value);
      //  Console.WriteLine(item + ": " + value);
      //}
      //Console.WriteLine("######################<<<<");
      //Console.WriteLine(testScan.Centroids.First().Mz + " - " + testScan.Centroids.Last().Mz);
      //Console.WriteLine(testScan_I.Centroids.First().Mz + " - " + testScan_I.Centroids.Last().Mz);
      //testScan.Meta.Trailer.TryGetValue("Ion Injection Time (ms):", out string valueTest);
      //testScan_I.Trailer.TryGetValue("Ion Injection Time (ms):", out string valueTest1);
      //Console.WriteLine(valueTest + " ms - " + valueTest1 + " ms");
    }

    void UpdateRunViewer()
    {
      if (simMS.IsRunning)
      {
        if (tbSpeed.Value < 3)
        {
          PanelList[nextPanel].BackColor = System.Drawing.Color.DarkGray;
          nextPanel++;
          if (nextPanel == 10) nextPanel = 0;
          PanelList[nextPanel].BackColor = System.Drawing.Color.Cyan;
        }
        else
        {
          Random rnd = new Random();
          if (rnd.Next(tbSpeed.Value) == 0)
          {
            PanelList[rnd.Next(10)].BackColor = System.Drawing.Color.DarkGray;
          }
          else
          {
            if (rnd.Next(2) == 0)
            {
              PanelList[rnd.Next(10)].BackColor = System.Drawing.Color.Cyan;
            }
            else
            {
              PanelList[rnd.Next(10)].BackColor = System.Drawing.Color.Magenta;
            }
          }
        }
      }
      else
      {
        foreach (Panel p in PanelList)
        {
          p.BackColor = System.Drawing.Color.DarkGray;
        }
      }
    }

    private void log(string msg)
    {
      rtbLog.AppendText(msg + Environment.NewLine);
    }

    void PopQueue()
    {
      if (FileQueue.Count == 0) return;
      FileQueue.RemoveAt(0);
      fileListBox.DataSource = null;
      fileListBox.DataSource = FileQueue;
    }

    private void RefreshStats()
    {
      TimeSpan ts = simMS.ElapsedRunTime();
      string aa = ts.ToString(@"hh\:mm\:ss");
      string bb = (allSimTime + ts).ToString(@"hh\:mm\:ss");
      ts = TimeSpan.FromMinutes(curSimRT);
      string cc = String.Format("{0}:{1}:{2}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"), ts.Seconds.ToString("00"));
      lblRunTime.Text = String.Format("{0}\n{1}\n{2}\n{3}", cc, aa, bb, curSimCount.ToString());

      string a = String.Format("{0}{1,9}", stats.curScanCount, stats.allScanCount);
      string b = String.Format("{0}{1,9}", stats.curScanMS1, stats.allScanMS1);
      string c = String.Format("{0}{1,9}", stats.curScanMS2, stats.allScanMS2);
      lblScanStats.Text = String.Format("{0}\n{1}\n{2}", a, b, c);
    }

    private void tbSpeed_ValueChanged(object sender, EventArgs e)
    {
      int[] speeds = [1, 2, 5, 10, 20, 50, 100];
      runSpeed = speeds[tbSpeed.Value];
      lblSpeed.Text = runSpeed.ToString() + "x";
      simMS.SetSpeed(runSpeed);
    }

    private void tsbPause_Click(object sender, EventArgs e)
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
        tsbRun.Enabled = true;
        tsbPause.Enabled = false;
      }
    }

    private void tsbStop_Click(object sender, EventArgs e)
    {
      simMS.Stop();
    }

    private void UpdateCustomScans(bool refreshDataSource)
    {
      if (refreshDataSource)
      {
        dgvCustomScan.DataSource = UserCustomScan.Values.ToList();
        dgvCustomScan.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvCustomScan.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        lblCustomScansRT.Text = "RT: " + UserCustomScan.RetentionTime.ToString("0.####");
      }
      lblCustomScans.Text = CustomScans.Count.ToString();
    }

    private void nudCustomScans_ValueChanged(object sender, EventArgs e)
    {
      if (nudCustomScans.Value == 0) return;
      CustomScansIndex = Convert.ToInt32(nudCustomScans.Value) - 1;
      UserCustomScan = CustomScans[CustomScansIndex];
      UpdateCustomScans(true);
    }


    protected override void WndProc(ref Message m)
    {
      const int WM_PARENTNOTIFY = 0x0210;
      if (m.Msg == WM_PARENTNOTIFY)
      {
        if (!Focused)
          Activate();
      }
      base.WndProc(ref m);
    }

    private void dgvFiles_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (dgvFiles.CurrentCell == null) return;
      if (e.ColumnIndex == 2)
      {
        var val = dgvFiles.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        if (val == null) return;
        if (val.ToString() == "Spectrum")
        {
          dgvFiles.Rows[e.RowIndex].Cells[3].Value = FileList[e.RowIndex].LowScan.ToString();
          dgvFiles.Rows[e.RowIndex].Cells[4].Value = FileList[e.RowIndex].HighScan.ToString();
        }
        else
        {
          dgvFiles.Rows[e.RowIndex].Cells[3].Value = FileList[e.RowIndex].LowRT.ToString();
          dgvFiles.Rows[e.RowIndex].Cells[4].Value = FileList[e.RowIndex].HighRT.ToString();
        }
      }
    }

    private void dgvFiles_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      e.Control.KeyPress -= new KeyPressEventHandler(DGVKeyPress);
      if (dgvFiles.CurrentCell.ColumnIndex > 2) //Desired Column
      {
        TextBox tb = e.Control as TextBox;
        if (tb != null)
        {
          tb.KeyPress += new KeyPressEventHandler(DGVKeyPress);
        }
      }
    }

    private void DGVKeyPress(object sender, KeyPressEventArgs e)
    {
      if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
      {
        e.Handled = true;
      }

      // only allow one decimal point
      if ((e.KeyChar == '.') && ((TextBox)sender).Text.IndexOf('.') > -1)
      {
        e.Handled = true;
      }
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      bool changeView = false;
      if (ofdAdd.ShowDialog() == DialogResult.OK)
      {
        foreach (string s in ofdAdd.FileNames)
        {
          FileStats stats = new FileStats();
          FileReader fileReader = new FileReader(s);
          stats.ScanCount = fileReader.ScanCount;
          stats.HighScan = fileReader.LastScan;
          stats.HighRT = fileReader.MaxRetentionTime;
          stats.LowScan = fileReader.FirstScan;
          stats.Name = Path.GetFileName(s);
          stats.Path = Path.GetDirectoryName(s);
          stats.FullPath = s;
          FileList.Add(stats);

          //fileListBox.Items.Add(s);
          var row = dgvFiles.Rows.Add();
          dgvFiles.Rows[row].Cells[0].Value = stats.Name;
          dgvFiles.Rows[row].Cells[1].Value = stats.Path;
          dgvFiles.Rows[row].Cells[2].Value = "Spectrum";
          dgvFiles.Rows[row].Cells[3].Value = stats.LowScan.ToString();
          dgvFiles.Rows[row].Cells[4].Value = stats.HighScan.ToString();

          changeView = true;
        }
      }

      if (changeView)
      {
        tabControlMain.SelectedIndex = 0;
      }
    }

    private void tsbDelete_Click(object sender, EventArgs e)
    {
      List<int> indexes = new List<int>();
      foreach (DataGridViewRow row in dgvFiles.SelectedRows)
      {
        indexes.Add(row.Index);
      }
      foreach (int index in indexes)
      {
        FileList.RemoveAt(index);
      }
      foreach (DataGridViewRow row in dgvFiles.SelectedRows)
      {
        dgvFiles.Rows.Remove(row);
      }
    }

    private void dgvFiles_SelectionChanged(object sender, EventArgs e)
    {
      if (dgvFiles.SelectedRows.Count > 0)
      {
        tsbAdd.Enabled = true;
        tsbDelete.Enabled = true;
      }
      else
      {
        tsbAdd.Enabled = false;
        tsbDelete.Enabled = false;
      }
    }

    private void fileListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (fileListBox.SelectedItems.Count > 0) tsbRemove.Enabled = true;
      else tsbRemove.Enabled = false;
    }

    private void CheckRunParametersButtons()
    {
      if (tabControlMain.SelectedIndex == 0)
      {
        if (dgvFiles.SelectedRows.Count > 0)
        {
          tsbAdd.Enabled = true;
          tsbDelete.Enabled = true;
        }
        else
        {
          tsbAdd.Enabled = false;
          tsbDelete.Enabled = false;
        }
        tsbLoad.Enabled = true;
      }
      else
      {
        tsbAdd.Enabled = false;
        tsbLoad.Enabled = false;
        tsbDelete.Enabled = false;
      }
    }

    private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
    {
      CheckRunParametersButtons();
    }

    private void panelMeter_Paint(object sender, PaintEventArgs e)
    {

    }

    private void ClearCustomScans()
    {
      dgvCustomScan.DataSource = null;
      dgvCustomScan.Refresh();
      CustomScans.Clear();
      nudCustomScans.Minimum = 0;
      nudCustomScans.Value = 0;
      nudCustomScans.Maximum = CustomScans.Count;
      CustomScansIndex = -1;
      lblCustomScansRT.Text = "RT: " + UserCustomScan.RetentionTime.ToString("0.####");
      lblCustomScans.Text = CustomScans.Count.ToString();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      ClearCustomScans();
    }
  }
}
