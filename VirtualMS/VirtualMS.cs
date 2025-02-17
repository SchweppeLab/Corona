using MSim;
using MSim.lib;
using ScottPlot;
using ScottPlot.Colormaps;
using ScottPlot.Plottables;

using Nova.Data.Spectrum;
using Pipes;

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System;

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
  }


  public partial class VirtualMs : Form
  {
    SimRunner simMS = new SimRunner();
    VMSServer server = new VMSServer("VirtualMS");
    readonly System.Windows.Forms.Timer UpdatePlotTimer = new System.Windows.Forms.Timer();
    readonly DataLogger plot;
    readonly Scatter plotSpec;

    private long lastTicks = 0;
    private volatile bool refreshSpectrum = false;
    private VMsStats stats = new VMsStats();

    private int curSimCount = 0;
    private double curSimRT = 0;
    private TimeSpan allSimTime = TimeSpan.Zero;

    private int runSpeed = 1;

    private bool paused = false;

    private List<string> Files = new List<string>();

    public VirtualMs()
    {
      InitializeComponent();

      UpdatePlotTimer.Enabled = true;
      UpdatePlotTimer.Interval = 100;

      plotTIC.UserInputProcessor.Disable();
      plot = plotTIC.Plot.Add.DataLogger();

      plot.Add(0, 0);
      plotTIC.Refresh();

      Coordinates[] co = new Coordinates[2];
      co[0].X = 1;
      co[0].Y = 1;
      co[1].X = 2;
      co[1].Y = 2;

      plotSpec = plotSpectrum.Plot.Add.Scatter(co);
      plotSpec.MarkerSize = 1;
      plotSpectrum.Plot.Axes.SetupMultiplierNotation(plotSpectrum.Plot.Axes.Left);
      plotSpectrum.Plot.YLabel("Intensity");
      plotSpectrum.Refresh();

      //myScat = plotTIC.Plot.Add.Scatter(plotX, plotY);
      //myScat.MarkerSize = 1;


      //plotTIC.Plot.HideGrid();
      plotTIC.Plot.Title("TIC");
      plotTIC.Plot.XLabel("Retention Time (min)");
      plotTIC.Plot.YLabel("TIC");

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

      simMS.AcquisitionEnd += OnAcquisitionEnd;
      simMS.AcquisitionStart += OnAcquisitionStart;

      simMS.MsScanArrived += OnMsScanArrived;
      server.ClientConnected += OnClientConnected;
      server.ClientDisconnected += OnClientDisconnected;

      //var exists = System.Diagnostics.Process.GetProcessesByName(
      //  System.IO.Path.GetFileNameWithoutExtension(
      //    System.Reflection.Assembly.GetEntryAssembly().Location
      //    )
      //  ).Count() > 1;

      log(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
      log("Server running: " + server.IsRunning());

      lastTicks = DateTime.Now.Ticks;

    }

    private void tspAdd_Click(object sender, EventArgs e)
    {
      if (ofdAdd.ShowDialog() == DialogResult.OK)
      {
        foreach (string s in ofdAdd.FileNames)
        {
          fileListBox.Items.Add(s);
        }
      }

    }

    private void tsbRemove_Click(object sender, EventArgs e)
    {
      foreach (var item in fileListBox.SelectedItems)
      {
        fileListBox.Items.Remove(item);
      }
    }

    private void tsbRun_Click(object sender, EventArgs e)
    {
      if (simMS.IsRunning)
      {
        if (paused)
        {
          simMS.Resume();
          paused = false;
        }
      }
      else
      {
        stats = new VMsStats();
        plot.Clear();
        plot.Add(0, 0);
        plot.Axes.YAxis.Range.Reset();
        plotTIC.Refresh();
        log("Starting Virtal MS");

        Files.Clear();
        foreach (var item in fileListBox.Items)
        {
          string s = item.ToString();
          if (File.Exists(s) && (Path.GetExtension(s) == ".raw" || Path.GetExtension(s) == ".mzML"))
          {
            Files.Add(s);
          }
        }

        simMS.Run(Files[0]);
        Files.RemoveAt(0);
      }
    }

    private void Acquisition_AcquisitionStreamClosing(object sender, EventArgs e)
    {
      log("Stream closing: " + e.ToString());
    }

    //private void Acquisition_AcquisitionStreamOpening(object sender, Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.AcquisitionOpeningEventArgs e)
    //{
    //  log("Stream opening: " + e.ToString());
    //}

    private void OnAcquisitionEnd(RunInfo info)
    {
      server?.SendAcquisitionEnd(info);
      log("Acquisition ended: " + info.Name);
      allSimTime += simMS.ElapsedRunTime();
      if (Files.Count > 0)
      {
        simMS.Run(Files[0]);
        Files.RemoveAt(0);
      }
    }

    private void OnAcquisitionStart(RunInfo info)
    {
      server?.SendAcquisitionStart(info);
      plot.Clear();
      plot.Add(0, 0);
      plotTIC.Refresh();
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

    private void OnMsScanArrived(object sender, MSimEventArgs e)
    {
      //log("Scan arrived");
      Spectrum scan = e.GetScan();
      if (scan != null)
      {
        server?.SendSpectrum(scan);
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

          double[] x = new double[scan.Count];
          double[] y = new double[scan.Count];
          for (int a = 0; a < scan.Count; a++)
          {
            x[a] = scan.DataPoints[a].Mz;
            y[a] = scan.DataPoints[a].Intensity;
          }
          lock (plotSpectrum.Plot.Sync)
          {
            plotSpectrum.Plot.Clear();
            if (scan.Centroid)
            {
              var bar = plotSpectrum.Plot.Add.Bars(x, y);
            }
            else
            {
              var scat = plotSpectrum.Plot.Add.Scatter(x, y);
              scat.MarkerSize = 1;
            }

            plotSpectrum.Plot.Axes.AutoScale();
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

    private void log(string msg)
    {
      rtbLog.AppendText(msg + Environment.NewLine);
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
      int[] speeds = new int[7];
      speeds[0] = 1;
      speeds[1] = 2;
      speeds[2] = 5;
      speeds[3] = 10;
      speeds[4] = 20;
      speeds[5] = 50;
      speeds[6] = 100;
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
      }
    }

    private void tsbStop_Click(object sender, EventArgs e)
    {
      simMS.Stop();
    }
  }
}
