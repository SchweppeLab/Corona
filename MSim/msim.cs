using MSim.lib;
using Nova.Data;
using Nova.Io.Read;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThermoFisher.CommonCore.Data.Business;

namespace MSim
{

  /// <summary>
  /// Contains information about a simulated MS run.
  /// </summary>
  public struct RunInfo
  {
    public string Name;
  }

  /// <summary>
  /// Class that operates the MS simulation asynchronously.
  /// </summary>
  public class SimRunner
  {
    //For storing timer values
    private long ms = 0;
    private long lastMS = 0;
    private long fastForwardMS = 0;

    public string DetectorClass => throw new NotImplementedException();

    /// <summary>
    /// Raised when simulated acquisition ends.
    /// </summary>
    public event AcquisitionEvent AcquisitionEnd;

    /// <summary>
    /// Raised when simulated acquisition starts.
    /// </summary>
    public event AcquisitionEvent AcquisitionStart;

    /// <summary>
    /// Raised when simulated MS scan is acquired.
    /// </summary>
    public event EventHandler<MSimEventArgs> MsScanArrived;

    /// <summary>
    /// The last scan that was acquired.
    /// </summary>
    private SpectrumEx LastMsScan { get; set; }

    /// <summary>
    /// Gains access to the last scan that was simulated.
    /// </summary>
    /// <returns>Spectrum object</returns>
    public SpectrumEx GetLastMsScan()
    {
        return LastMsScan;
    }

    public double GetRT()
    {
      return Convert.ToDouble(ms + fastForwardMS) /60000;
    }

    public delegate void MsScanEvent(Spectrum spec);
    public delegate void AcquisitionEvent(RunInfo info);

    /// <summary>
    /// Used to match simulation times with scan retention times
    /// </summary>
    private Stopwatch RunTimer = new Stopwatch();

    /// <summary>
    /// Speed (in fold change) of the simulation.
    /// </summary>
    private int Speed = 1;

    /// <summary>
    /// Pauses simulation when true.
    /// </summary>
    private bool PauseFlag = false;

    /// <summary>
    /// Cancels simulation when true
    /// </summary>
    private bool CancelFlag = false;

    //TODO: Rather than yes/no, create states such as Running/Stopped/Paused/etc.
    /// <summary>
    /// Indicates if the simulation is currently running.
    /// </summary>
    public bool IsRunning { get; private set; } = false;

    /// <summary>
    /// Get the total elapsed time (real time) of the current simulation.
    /// </summary>
    /// <returns></returns>
    public TimeSpan ElapsedRunTime()
    {
      return RunTimer.Elapsed;
    }

    /// <summary>
    /// Raise the aqcuisition end event.
    /// </summary>
    /// <param name="info">: the run information that just finished.</param>
    private void OnAcquisitionEnd(RunInfo info)
    {
      AcquisitionEnd?.Invoke(info);
    }

    /// <summary>
    /// Raise the acquisition start event.
    /// </summary>
    /// <param name="info">: the run information that just started.</param>
    private void OnAcquisitionStart(RunInfo info)
    {
      AcquisitionStart?.Invoke(info);
    }

    /// <summary>
    /// Pauses the simulation. No additional effect if simulation is already paused.
    /// </summary>
    public void Pause()
    {
      PauseFlag = true;
    }

    /// <summary>
    /// Resumes the simulation from the paused state. No effect if the simulation is already running.
    /// </summary>
    public void Resume()
    {
      PauseFlag = false;
    }

    /// <summary>
    /// Raise an event indicating a scan has been acquired.
    /// </summary>
    /// <param name="scan">: the Spectrum object acquired.</param>
    protected virtual void SendMsScanArrived(SpectrumEx scan, SpectrumEx? cent)
    {
      if (cent == null)
      {
        MsScanArrived?.Invoke(this, new MSimEventArgs(scan, true, true));
      }
      else
      {
        MsScanArrived?.Invoke(this, new MSimEventArgs(scan, true, false));
        MsScanArrived?.Invoke(this, new MSimEventArgs(cent, false, true));
      }
    }

    /// <summary>
    /// Sets the speed of the simulation (in multiplication factor). Range limited from 1 to 100.
    /// Can be set while the simulation is running or not.
    /// </summary>
    /// <param name="speed">: the desired speed.</param>
    /// <returns>On success, returns the new speed, otherwise returns the current speed.</returns>
    public int SetSpeed(int speed)
    {
      if (speed < 1) return Speed;
      if (speed > 100) return Speed;
      Speed = speed;
      return Speed;
    }

    /// <summary>
    /// Stops the simulation. No effect if simulation is already stopped.
    /// </summary>
    public void Stop()
    {
      CancelFlag = true;
    }

    /// <summary>
    /// Starts asynchronous simulation of a mass spectrometer.
    /// </summary>
    /// <param name="path">: the MS data file to be simulated.</param>
    /// <param name="maxNumScans">: stops the simulation when this number of scans are acquired.</param>
    /// <param name="FirstScan">: starts the simulation from this scan number.</param>
    /// <param name="LastScan">: stops the simulation when this scan number is reached.</param>
    public async void Run(string path, int maxNumScans = -1, int FirstScan = -1, int LastScan = -1)
    {
      CancelFlag = false;
      PauseFlag = false;
      IsRunning = true;

      RunInfo runInfo = new RunInfo();
      runInfo.Name = path;

      //Toss up the start event
      OnAcquisitionStart(runInfo);

      await Task.Run(() =>
      {
        FileReader reader = new FileReader();
        SpectrumEx spectrum;
        SpectrumEx specCent = null; //centroid spectrum is raw file only, replicates IAPI.

        int scanCount = 0;
        ms = 0;
        lastMS = 0;
        fastForwardMS = 0;

        //Start a high performance timer.
        Stopwatch sw = Stopwatch.StartNew();
        RunTimer.Reset();
        RunTimer.Start();

        //Open the file and read the first spectrum
        FileFormat fileFormat = reader.CheckFileFormat(path);
        spectrum = reader.ReadSpectrumEx(path, FirstScan, false);
        if (fileFormat == FileFormat.ThermoRaw) specCent = reader.ReadSpectrumEx(null, spectrum.ScanNumber, true);

        //if user requested a scan from the middle of the file, advance the stopwatch to that time;
        if (FirstScan > -1)
        {
          fastForwardMS = (long)(spectrum.RetentionTime * 60000);
        }

        while (spectrum.ScanNumber > 0)
        {

          //if we read past the desired last scan, stop now. This is a failsafe if spectrum file skips scan numbers.
          if (LastScan > -1 && spectrum.ScanNumber > LastScan) break;

          LastMsScan = spectrum;
  
          //don't fire off the spectra until the instrument acquired it.
          while (ms+fastForwardMS < spectrum.RetentionTime*60000)
          {
            long tmpMS = sw.ElapsedMilliseconds;
            ms += (tmpMS-lastMS) * Speed;
            lastMS = tmpMS;
            //Thread.Sleep(10); //sleeping might make virtual MS less resource greedy.
          }
          SendMsScanArrived(spectrum,specCent);

          //Check if we reached the requested limits of simulation.
          scanCount++;
          if (maxNumScans>0 && scanCount >= maxNumScans) break;
          if (spectrum.ScanNumber == LastScan) break;

          //Manage requests to pause, stop, or restart
          if (PauseFlag)
          {
            sw.Stop();
            RunTimer.Stop();
            while (PauseFlag)
            {
              if (CancelFlag) break;
              Thread.Sleep(10);
            }
            sw.Start();
            RunTimer.Start();
          }
          if (CancelFlag)
          {
            if (RunTimer.IsRunning) RunTimer.Stop();
            return;
          }

          //on to the next scan
          spectrum = reader.ReadSpectrumEx(null, -1, false);
          if (fileFormat == FileFormat.ThermoRaw) specCent = reader.ReadSpectrumEx(null, spectrum.ScanNumber, true);
        }
        if(RunTimer.IsRunning) RunTimer.Stop();              
      });
      IsRunning = false;

      //throw up that we're done with the file.
      OnAcquisitionEnd(runInfo);
    }
  }

}
