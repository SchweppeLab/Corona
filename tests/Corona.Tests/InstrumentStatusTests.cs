namespace Corona.Tests
{
  // The transitions an instrument makes, as InstrumentStatus reports them.
  [TestClass]
  public class InstrumentStatusTests
  {
    private static (InstrumentStatus Status, List<StateReport> Reports) NewStatus()
    {
      var status = new InstrumentStatus();
      var reports = new List<StateReport>();
      status.Changed += reports.Add;
      return (status, reports);
    }

    [TestMethod]
    public void StartsOnAndReadyForRun()
    {
      var (status, _) = NewStatus();
      Assert.AreEqual(new StateReport(SystemMode.On, InstrumentState.ReadyForRun, false, false), status.Current);
      Assert.IsTrue(status.CanStart);
    }

    [TestMethod]
    public void ToPayload_HasEveryKeyInProtocolForm()
    {
      var report = new StateReport(SystemMode.DirectControl, InstrumentState.Running, true, false);
      Assert.AreEqual("Mode=DirectControl\nState=Running\nCanPause=true\nCanResume=false", report.ToPayload());
    }

    [TestMethod]
    public void OwnAcquisition_StaysOnWhileRunning()
    {
      var (status, reports) = NewStatus();
      status.BeginAcquisition(underDirectControl: false);
      Assert.AreEqual(new StateReport(SystemMode.On, InstrumentState.Running, true, false), status.Current);
      Assert.IsFalse(status.CanStart);
      Assert.AreEqual(1, reports.Count);
    }

    [TestMethod]
    public void ClientAcquisition_RunsUnderDirectControl()
    {
      var (status, _) = NewStatus();
      status.BeginAcquisition(underDirectControl: true);
      Assert.AreEqual(new StateReport(SystemMode.DirectControl, InstrumentState.Running, true, false), status.Current);
    }

    [TestMethod]
    public void Pause_SwapsTheFlags_AndAClientAcquisitionShowsThePausedMode()
    {
      var (status, _) = NewStatus();
      status.BeginAcquisition(underDirectControl: true);
      status.SetPaused(true);
      Assert.AreEqual(new StateReport(SystemMode.DirectControlPaused, InstrumentState.Running, false, true), status.Current);
      status.SetPaused(false);
      Assert.AreEqual(new StateReport(SystemMode.DirectControl, InstrumentState.Running, true, false), status.Current);
    }

    [TestMethod]
    public void Pause_OfTheInstrumentsOwnAcquisition_KeepsOn()
    {
      var (status, _) = NewStatus();
      status.BeginAcquisition(underDirectControl: false);
      status.SetPaused(true);
      Assert.AreEqual(new StateReport(SystemMode.On, InstrumentState.Running, false, true), status.Current);
    }

    [TestMethod]
    public void Pause_WhileIdle_ChangesNothing()
    {
      var (status, reports) = NewStatus();
      status.SetPaused(true);
      Assert.AreEqual(0, reports.Count);
    }

    [TestMethod]
    public void EndAcquisition_ShowsTheIdleStateOfTheNextMode()
    {
      var expected = new Dictionary<SystemMode, InstrumentState>
      {
        [SystemMode.On] = InstrumentState.ReadyForRun,
        [SystemMode.Standby] = InstrumentState.StandBy,
        [SystemMode.Off] = InstrumentState.Off,
        [SystemMode.Disconnected] = InstrumentState.NotConnected,
      };
      foreach (var (mode, state) in expected)
      {
        var (status, _) = NewStatus();
        status.BeginAcquisition(underDirectControl: true);
        status.EndAcquisition(mode);
        Assert.AreEqual(new StateReport(mode, state, false, false), status.Current, mode.ToString());
      }
    }

    [TestMethod]
    public void SetMode_WhileIdle_EntersTheModeWithItsIdleState()
    {
      var (status, _) = NewStatus();
      status.SetMode(SystemMode.Standby);
      Assert.AreEqual(new StateReport(SystemMode.Standby, InstrumentState.StandBy, false, false), status.Current);
      Assert.IsFalse(status.CanStart);
      status.SetMode(SystemMode.On);
      Assert.IsTrue(status.CanStart);
    }

    [TestMethod]
    public void SetMode_WhileAcquiring_OnlyOnIsPossible()
    {
      var (status, _) = NewStatus();
      status.BeginAcquisition(underDirectControl: true);
      status.SetMode(SystemMode.On);
      Assert.AreEqual(new StateReport(SystemMode.On, InstrumentState.Running, true, false), status.Current);
      Assert.ThrowsException<InvalidOperationException>(() => status.SetMode(SystemMode.Off));
      Assert.AreEqual(InstrumentState.Running, status.Current.State);
    }

    [TestMethod]
    public void Changed_IsRaisedOnlyWhenSomethingChanges()
    {
      var (status, reports) = NewStatus();
      status.SetMode(SystemMode.On);
      status.EndAcquisition(SystemMode.On);
      Assert.AreEqual(0, reports.Count);
      status.SetMode(SystemMode.Off);
      Assert.AreEqual(1, reports.Count);
      Assert.AreEqual(status.Current, reports[0]);
    }
  }
}
