using MStreamer;

namespace Corona
{
  /// <summary>
  /// Corona's counterpart of the IAPI's acquisition interface: the instrument status, the replay
  /// that stands in for an acquisition, and the state reports every Helios client receives. A
  /// start, pause, cancel or mode change goes through here whether a Corona button or a client
  /// asked for it, so both drive the same state and both report. Subscribes to the runner before
  /// the view model does, which puts each report before the stream message it belongs to: a
  /// start is '6' Running then '2', an end is '3' then '6' idle.
  /// </summary>
  public sealed class AcquisitionControl
  {
    private readonly SimRunner runner;
    private readonly VMSServer server;
    // Keeps a report to one client and a broadcast apart, so a client that connects during a
    // transition ends up with the newer state whichever report reaches it first.
    private readonly object reportGate = new();
    private AcquisitionRequest? clientRequest; // behind the current replay; null for the panel's
    private SystemMode? modeAfterRun;          // requested during the replay, entered when it ends

    public InstrumentStatus Status { get; } = new();

    public AcquisitionControl(SimRunner runner, VMSServer server)
    {
      this.runner = runner;
      this.server = server;
      runner.AcquisitionStart += OnRunnerStart;
      runner.AcquisitionEnd += OnRunnerEnd;
      Status.Changed += report => { lock (reportGate) server.SendStateReport(report); };
      server.ClientConnected += client => { lock (reportGate) server.SendStateReport(Status.Current, client); };
    }

    /// <summary>
    /// True when a start would be accepted: the instrument is on and idle.
    /// </summary>
    public bool CanStart => Status.CanStart;

    /// <summary>
    /// Starts a replay: as the instrument's own acquisition when there is no request (the mode
    /// stays On), or for a client's request under direct control, with what Corona can honor of
    /// it: the scan-count or duration limit, the start delay, the raw file name and the
    /// continuation. Throws when the instrument is not on and idle, as the IAPI does.
    /// </summary>
    public void Start(FileStats file, AcquisitionRequest? request = null)
    {
      if (!Status.CanStart)
        throw new InvalidOperationException($"The instrument is {Status.Current}; a start needs On / ReadyForRun.");
      clientRequest = request;
      modeAfterRun = null;
      int maxScans = request?.Kind == AcquisitionKind.LimitedByCount ? request.NumberOfScans : -1;
      double maxMinutes = request?.Duration?.TotalMinutes ?? -1;
      double startDelay = request?.SingleProcessingDelay ?? 0;
      runner.Run(file.FullPath, maxScans, file.StartScan, file.EndScan, maxMinutes, startDelay);
    }

    public void Pause()
    {
      runner.Pause();
      Status.SetPaused(true);
    }

    public void Resume()
    {
      runner.Resume();
      Status.SetPaused(false);
    }

    /// <summary>
    /// Ends the replay; its end reports as any end does.
    /// </summary>
    public void Cancel() => runner.Stop();

    /// <summary>
    /// A custom scan has arrived, which ends a start delay as it does on the instrument.
    /// </summary>
    public void CustomScanArrived() => runner.ReleaseStartDelay();

    /// <summary>
    /// Puts the instrument in a mode. On during a replay keeps the replay (a client's becomes the
    /// instrument's own); any other mode ends the replay first and is reported once the stream
    /// has closed, since the instrument is not in that mode until then.
    /// </summary>
    public void SetMode(SystemMode mode)
    {
      if (Status.Current.State == InstrumentState.Running && mode != SystemMode.On)
      {
        modeAfterRun = mode;
        runner.Stop();
        return;
      }
      Status.SetMode(mode);
    }

    private void OnRunnerStart(RunInfo info)
    {
      Status.BeginAcquisition(underDirectControl: clientRequest != null);
      string name = string.IsNullOrEmpty(clientRequest?.RawFileName) ? info.Name : clientRequest.RawFileName;
      server.SendAcquisitionStart(name);
    }

    private void OnRunnerEnd(RunInfo info)
    {
      server.SendAcquisitionEnd(info.Name);
      SystemMode next = modeAfterRun ?? (clientRequest?.Continuation switch
      {
        AcquisitionContinuation.Standby => SystemMode.Standby,
        AcquisitionContinuation.Off => SystemMode.Off,
        _ => SystemMode.On,
      });
      clientRequest = null;
      modeAfterRun = null;
      Status.EndAcquisition(next);
    }
  }
}
