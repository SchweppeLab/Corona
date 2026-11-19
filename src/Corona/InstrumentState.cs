namespace Corona
{
  /// <summary>
  /// The IAPI system modes Corona can be in, under their IAPI names: Helios parses the names.
  /// </summary>
  public enum SystemMode
  {
    /// <summary>The instrument has lost its link, as a rebooting instrument does.</summary>
    Disconnected,
    /// <summary>On: idle, or acquiring on the instrument's own behalf.</summary>
    On,
    Standby,
    Off,
    /// <summary>Acquiring for the client that started the acquisition.</summary>
    DirectControl,
    /// <summary>A client's acquisition, paused.</summary>
    DirectControlPaused,
  }

  /// <summary>
  /// The IAPI instrument states Corona can be in, under their IAPI names.
  /// </summary>
  public enum InstrumentState
  {
    NotConnected,
    ReadyForRun,
    Running,
    StandBy,
    Off,
  }

  /// <summary>
  /// A state report, pipe message '6': what every Helios client holds as the instrument's state.
  /// </summary>
  public sealed record StateReport(SystemMode Mode, InstrumentState State, bool CanPause, bool CanResume)
  {
    /// <summary>
    /// The report as the protocol's Key=Value lines.
    /// </summary>
    public string ToPayload() =>
      $"Mode={Mode}\nState={State}\nCanPause={(CanPause ? "true" : "false")}\nCanResume={(CanResume ? "true" : "false")}";

    public override string ToString() => $"{Mode} / {State}";
  }

  /// <summary>
  /// The instrument's status and the transitions an instrument makes. Corona reports only a
  /// status it is in, so each transition here follows something the replay has done.
  /// Thread-safe; Changed is raised after each change, outside the lock, with the new report.
  /// </summary>
  public sealed class InstrumentStatus
  {
    /// <summary>
    /// The modes a user or client can put the instrument in. DirectControl is not one: a
    /// client's acquisition enters it, and the acquisition's end leaves it.
    /// </summary>
    public static readonly SystemMode[] SelectableModes =
      { SystemMode.On, SystemMode.Standby, SystemMode.Off, SystemMode.Disconnected };

    private readonly object gate = new();
    private StateReport current = new(SystemMode.On, InstrumentState.ReadyForRun, false, false);

    public event Action<StateReport>? Changed;

    public StateReport Current
    {
      get { lock (gate) return current; }
    }

    /// <summary>
    /// True when a start would be accepted: on and idle, the IAPI's condition.
    /// </summary>
    public bool CanStart
    {
      get
      {
        StateReport c = Current;
        return c.Mode == SystemMode.On && c.State == InstrumentState.ReadyForRun;
      }
    }

    /// <summary>
    /// The state an idle instrument shows in each mode.
    /// </summary>
    public static InstrumentState IdleStateOf(SystemMode mode) => mode switch
    {
      SystemMode.Disconnected => InstrumentState.NotConnected,
      SystemMode.Standby => InstrumentState.StandBy,
      SystemMode.Off => InstrumentState.Off,
      _ => InstrumentState.ReadyForRun,
    };

    /// <summary>
    /// Puts an idle instrument in a mode. While acquiring, only On is possible (a client's
    /// acquisition then goes on as the instrument's own); any other mode must wait for the
    /// acquisition to end.
    /// </summary>
    public void SetMode(SystemMode mode)
    {
      Update(c =>
      {
        if (c.State != InstrumentState.Running) return new StateReport(mode, IdleStateOf(mode), false, false);
        if (mode != SystemMode.On)
          throw new InvalidOperationException($"The instrument cannot enter {mode} while acquiring; end the acquisition first.");
        return c with { Mode = SystemMode.On };
      });
    }

    /// <summary>
    /// An acquisition has begun: the instrument's own (the mode stays On) or a client's
    /// (DirectControl).
    /// </summary>
    public void BeginAcquisition(bool underDirectControl) =>
      Update(c => new StateReport(underDirectControl ? SystemMode.DirectControl : SystemMode.On, InstrumentState.Running, true, false));

    /// <summary>
    /// The acquisition paused or resumed. The state stays Running, as on an instrument, where a
    /// pause stops the data from being stored; the flags say which of pause and resume is
    /// possible, and a client's acquisition shows the IAPI's paused mode.
    /// </summary>
    public void SetPaused(bool paused)
    {
      Update(c =>
      {
        if (c.State != InstrumentState.Running) return c;
        SystemMode mode = c.Mode switch
        {
          SystemMode.DirectControl when paused => SystemMode.DirectControlPaused,
          SystemMode.DirectControlPaused when !paused => SystemMode.DirectControl,
          _ => c.Mode,
        };
        return new StateReport(mode, InstrumentState.Running, !paused, paused);
      });
    }

    /// <summary>
    /// The acquisition has ended and the instrument is idle in the given mode.
    /// </summary>
    public void EndAcquisition(SystemMode mode) =>
      Update(c => new StateReport(mode, IdleStateOf(mode), false, false));

    private void Update(Func<StateReport, StateReport> transition)
    {
      StateReport next;
      lock (gate)
      {
        next = transition(current);
        if (next == current) return;
        current = next;
      }
      Changed?.Invoke(next);
    }
  }
}
