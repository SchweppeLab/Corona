using Nova.Data;
using Nova.Io.Read;
using System.Diagnostics;

/* MStreamer takes inspiration from the original MSim from the Gygi? lab.
 * Some class names appear similarly.
 */
namespace MStreamer
{

  /// <summary>
  /// Contains information about a simulated MS run.
  /// </summary>
  public struct RunInfo
  {
    /// <summary>
    /// Path of the data file being simulated.
    /// </summary>
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

    //For detecting when actual playback can't keep up with the requested Speed
    private long speedCheckRealMS = 0;
    private double speedCheckSimRT = 0;
    private bool speedLagging = false;

    /// <summary>
    /// Raised when simulated acquisition ends.
    /// </summary>
    public event AcquisitionEvent? AcquisitionEnd;

    /// <summary>
    /// Raised when simulated acquisition starts.
    /// </summary>
    public event AcquisitionEvent? AcquisitionStart;

    /// <summary>
    /// Raised when simulated MS scan is acquired.
    /// </summary>
    public event EventHandler<MStreamerEventArgs>? MsScanArrived;

    /// <summary>
    /// Raised when a run fails to open or read its file. AcquisitionEnd still follows.
    /// </summary>
    public event RunErrorEvent? RunError;

    /// <summary>
    /// Raised when actual playback throughput crosses the "keeping up with the requested
    /// Speed" threshold in either direction - i.e. scan reading/processing has become (or
    /// has stopped being) the bottleneck rather than the pacing wait.
    /// </summary>
    public event SpeedWarningEvent? SpeedWarning;

    /// <summary>
    /// The last scan that was acquired.
    /// </summary>
    private SpectrumEx? LastMsScan { get; set; }

    /// <summary>
    /// Gains access to the last scan that was simulated.
    /// </summary>
    /// <returns>Spectrum object, or null before the first scan has been acquired</returns>
    public SpectrumEx? GetLastMsScan()
    {
        return LastMsScan;
    }

    /// <summary>
    /// Current simulated retention time.
    /// </summary>
    /// <returns>Minutes since the start of the file being simulated</returns>
    public double GetRT()
    {
      return Convert.ToDouble(ms + fastForwardMS) /60000;
    }

    /// <summary>
    /// Handler for the acquisition start and end events.
    /// </summary>
    /// <param name="info">: the run that started or ended.</param>
    public delegate void AcquisitionEvent(RunInfo info);

    /// <summary>
    /// Handler for the RunError event.
    /// </summary>
    /// <param name="info">: the run that failed.</param>
    /// <param name="error">: the failure.</param>
    public delegate void RunErrorEvent(RunInfo info, Exception error);

    /// <summary>
    /// Handler for the SpeedWarning event.
    /// </summary>
    /// <param name="message">: describes the change in playback pace.</param>
    public delegate void SpeedWarningEvent(string message);

    /// <summary>
    /// Used to match simulation times with scan retention times
    /// </summary>
    private Stopwatch RunTimer = new Stopwatch();

    /// <summary>
    /// Speed (in fold change) of the simulation.
    /// </summary>
    private volatile int Speed = 1;

    /// <summary>
    /// Pauses simulation when true.
    /// </summary>
    private volatile bool PauseFlag = false;

    /// <summary>
    /// Cancels simulation when true
    /// </summary>
    private volatile bool CancelFlag = false;

    /// <summary>
    /// Ends the start delay early when true.
    /// </summary>
    private volatile bool StartDelayReleased = false;

    //TODO: Rather than yes/no, create states such as Running/Stopped/Paused/etc.
    /// <summary>
    /// Indicates if the simulation is currently running.
    /// </summary>
    public bool IsRunning { get; private set; } = false;

    /// <summary>
    /// Get the total elapsed time (real time) of the current simulation.
    /// </summary>
    /// <returns>Real time spent running, excluding time paused</returns>
    public TimeSpan ElapsedRunTime()
    {
      return RunTimer.Elapsed;
    }

    /// <summary>
    /// Raise the acquisition end event.
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
    /// Ends a pending start delay (see Run) so the first scan can go out. No effect without one.
    /// </summary>
    public void ReleaseStartDelay()
    {
      StartDelayReleased = true;
    }

    /// <summary>
    /// Raise an event indicating a scan has been acquired.
    /// </summary>
    /// <param name="scan">: the Spectrum object acquired.</param>
    /// <param name="cent">: the centroided form of the same scan, or null when the file has none.</param>
    protected virtual void SendMsScanArrived(SpectrumEx scan, SpectrumEx? cent)
    {
      if (cent == null)
      {
        MsScanArrived?.Invoke(this, new MStreamerEventArgs(scan, true, true));
      }
      else
      {
        MsScanArrived?.Invoke(this, new MStreamerEventArgs(scan, true, false));
        MsScanArrived?.Invoke(this, new MStreamerEventArgs(cent, false, true));
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
    /// <param name="maxRunMinutes">: stops the simulation once this much simulated time has passed since it started.</param>
    /// <param name="startDelaySeconds">: holds the first scan back for this many real seconds, or until ReleaseStartDelay is called.</param>
    public async void Run(string path, int maxNumScans = -1, int FirstScan = -1, int LastScan = -1, double maxRunMinutes = -1, double startDelaySeconds = 0)
    {
      CancelFlag = false;
      PauseFlag = false;
      StartDelayReleased = false;
      IsRunning = true;

      RunInfo runInfo = new RunInfo();
      runInfo.Name = path;

      //Toss up the start event
      OnAcquisitionStart(runInfo);

      Exception? failure = null;
      await Task.Run(() =>
      {
        try
        {
          Replay(path, maxNumScans, FirstScan, LastScan, maxRunMinutes, startDelaySeconds);
        }
        catch (Exception ex)
        {
          //Nova throws when it cannot open or index the file. Report it through RunError
          //rather than letting it escape this async void method onto the caller's thread.
          failure = ex;
        }
        if (RunTimer.IsRunning) RunTimer.Stop();
      });
      IsRunning = false;

      if (failure != null) RunError?.Invoke(runInfo, failure);

      //throw up that we're done with the file.
      OnAcquisitionEnd(runInfo);
    }

    /// <summary>
    /// The replay loop Run executes on a background thread.
    /// </summary>
    private void Replay(string path, int maxNumScans, int FirstScan, int LastScan, double maxRunMinutes, double startDelaySeconds)
    {
      FileReader reader = new FileReader();
      SpectrumEx spectrum;
      SpectrumEx? specCent = null; //centroid spectrum is raw file only, replicates IAPI.

      int scanCount = 0;
      ms = 0;
      lastMS = 0;
      fastForwardMS = 0;

      //Start a high performance timer.
      Stopwatch sw = Stopwatch.StartNew();
      RunTimer.Reset();
      RunTimer.Start();

      //Open the file and read the first spectrum
      FileFormat fileFormat = FileReader.CheckFileFormat(path);
      spectrum = reader.ReadSpectrumEx(path, FirstScan, false);
      if (fileFormat == FileFormat.ThermoRaw) specCent = reader.ReadSpectrumEx("", spectrum.ScanNumber, true);

      //if user requested a scan from the middle of the file, advance the stopwatch to that time;
      if (FirstScan > -1)
      {
        fastForwardMS = (long)(spectrum.RetentionTime * 60000);
      }

      //An instrument given a start delay takes no scan until the delay passes or a custom scan
      //arrives (ReleaseStartDelay). The hold is not run time, so both timers wait it out.
      if (startDelaySeconds > 0)
      {
        sw.Stop();
        RunTimer.Stop();
        long holdMS = (long)(startDelaySeconds * 1000);
        Stopwatch hold = Stopwatch.StartNew();
        while (!StartDelayReleased && hold.ElapsedMilliseconds < holdMS)
        {
          if (CancelFlag) return;
          Thread.Sleep(10);
        }
        sw.Start();
        RunTimer.Start();
      }

      //Where the simulated clock, which starts at fastForwardMS, ends a duration-limited run.
      double limitMS = maxRunMinutes > 0 ? fastForwardMS + maxRunMinutes * 60000 : double.PositiveInfinity;
      bool timeIsUp = false;

      speedCheckRealMS = 0;
      speedCheckSimRT = spectrum.RetentionTime;
      speedLagging = false;

      while (spectrum.ScanNumber > 0)
      {

        //if we read past the desired last scan, stop now. This is a failsafe if spectrum file skips scan numbers.
        if (LastScan > -1 && spectrum.ScanNumber > LastScan) break;

        LastMsScan = spectrum;

        //don't fire off the spectra until the instrument acquired it.
        while (ms+fastForwardMS < spectrum.RetentionTime*60000)
        {
          //A Stop() during a long gap must not wait for the gap to elapse, nor must the
          //duration limit wait for the next scan.
          if (CancelFlag) break;
          if (ms+fastForwardMS >= limitMS) { timeIsUp = true; break; }

          long tmpMS = sw.ElapsedMilliseconds;
          ms += (tmpMS-lastMS) * Speed;
          lastMS = tmpMS;

          //Sleep through most of a long gap so the wait doesn't peg a CPU core;
          //only tight-spin for the final few milliseconds to keep firing precise.
          double realMsRemaining = (spectrum.RetentionTime*60000 - (ms+fastForwardMS)) / Speed;
          if (realMsRemaining > 5) Thread.Sleep(1);
        }
        if (CancelFlag || timeIsUp) return;
        SendMsScanArrived(spectrum,specCent);

        //Periodically compare actual scan-delivery throughput against the requested Speed.
        //Reading/processing a scan takes real wall-clock time regardless of Speed, so a
        //dense file or slow storage can silently cap achievable playback below what was
        //requested; only report on a change of state so this doesn't spam the log.
        long nowRealMS = sw.ElapsedMilliseconds;
        long elapsedSinceCheck = nowRealMS - speedCheckRealMS;
        if (elapsedSinceCheck >= 2000)
        {
          double achievedSpeed = (spectrum.RetentionTime - speedCheckSimRT) * 60000 / elapsedSinceCheck;
          bool isLaggingNow = achievedSpeed < Speed * 0.9;
          if (isLaggingNow && !speedLagging)
          {
            SpeedWarning?.Invoke($"Falling behind requested {Speed}x speed - achieving roughly {achievedSpeed:0.#}x. Scan reading/processing is likely the bottleneck.");
          }
          else if (!isLaggingNow && speedLagging)
          {
            SpeedWarning?.Invoke($"Back on pace with requested {Speed}x speed.");
          }
          speedLagging = isLaggingNow;
          speedCheckRealMS = nowRealMS;
          speedCheckSimRT = spectrum.RetentionTime;
        }

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
        if (CancelFlag) return;

        //on to the next scan
        spectrum = reader.ReadSpectrumEx("", -1, false);
        if (fileFormat == FileFormat.ThermoRaw) specCent = reader.ReadSpectrumEx("", spectrum.ScanNumber, true);
      }
    }
  }

}
