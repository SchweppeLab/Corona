using System.IO;
using MStreamer;

namespace Corona.Tests
{
  // The replay as an instrument's acquisition, seen by a client on a real pipe: which state
  // reports surround the stream messages, and in what order. Scans are not part of it (the view
  // model sends those). The fixture holds four scans at 0.50, 0.60, 0.90 and 1.00 min; at 100x
  // a full replay takes about 0.6 s, and at 1x the first scan is 30 s away, so a replay at 1x
  // stays in its first gap for as long as a test needs it running.
  [TestClass]
  public class AcquisitionControlTests
  {
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

    private static readonly FileStats Fixture = new()
    {
      FullPath = Path.Combine(AppContext.BaseDirectory, "Files", "NovaTestFixture.mzML"),
      StartScan = 1,
      EndScan = 4,
    };

    private const string Idle = "Mode=On\nState=ReadyForRun\nCanPause=false\nCanResume=false";

    private sealed class Rig : IDisposable
    {
      public readonly SimRunner Runner = new();
      public readonly VMSServer Server;
      public readonly AcquisitionControl Control;
      public readonly PipeRecorder Client;
      public readonly ManualResetEventSlim Ended = new();

      public Rig(int speed = 100)
      {
        string pipe = "CoronaTestPipe_" + Guid.NewGuid().ToString("N");
        Server = new VMSServer(pipe);
        Control = new AcquisitionControl(Runner, Server);
        Runner.AcquisitionEnd += _ => Ended.Set();
        Runner.SetSpeed(speed);
        Client = new PipeRecorder(pipe);
        Assert.IsTrue(Client.WaitUntil(m => m.Count >= 1, Timeout), "no state report on connect");
      }

      public void WaitForCodes(string codes) =>
        Assert.IsTrue(Client.WaitUntil(m => string.Concat(m.Select(x => x.Code)) == codes, Timeout),
          $"expected messages {codes}, got {Client.Codes()}");

      public void Dispose()
      {
        Client.Dispose();
        Server.Stop();
      }
    }

    [TestMethod]
    public void ConnectingClient_GetsTheCurrentStateFirst()
    {
      using var rig = new Rig();
      var first = rig.Client.Snapshot()[0];
      Assert.AreEqual('6', first.Code);
      Assert.AreEqual(Idle, first.Text);
    }

    [TestMethod]
    public void OwnStart_ReportsRunningBeforeTheStreamOpens_AndReadyAfterItCloses()
    {
      using var rig = new Rig();

      rig.Control.Start(Fixture);

      Assert.IsTrue(rig.Ended.Wait(Timeout), "the replay never ended");
      rig.WaitForCodes("66236");
      var m = rig.Client.Snapshot();
      Assert.AreEqual("Mode=On\nState=Running\nCanPause=true\nCanResume=false", m[1].Text);
      Assert.AreEqual(Fixture.FullPath, m[2].Text, "the stream opens under the file's name");
      Assert.AreEqual(Idle, m[4].Text);
      Assert.IsTrue(rig.Control.CanStart);
    }

    [TestMethod]
    public void ClientStart_RunsUnderDirectControl_WithItsRawFileNameLimitAndContinuation()
    {
      using var rig = new Rig();
      var request = AcquisitionRequest.Parse("Kind=LimitedByCount\nRawFileName=probe.raw\nContinuation=Standby\nNumberOfScans=2\n");

      rig.Control.Start(Fixture, request);

      Assert.IsTrue(rig.Ended.Wait(Timeout), "the replay never ended");
      rig.WaitForCodes("66236");
      var m = rig.Client.Snapshot();
      Assert.AreEqual("Mode=DirectControl\nState=Running\nCanPause=true\nCanResume=false", m[1].Text);
      Assert.AreEqual("probe.raw", m[2].Text);
      Assert.AreEqual(2, rig.Runner.GetLastMsScan()?.ScanNumber, "the scan count limit");
      Assert.AreEqual("Mode=Standby\nState=StandBy\nCanPause=false\nCanResume=false", m[4].Text);
      Assert.IsFalse(rig.Control.CanStart);
    }

    [TestMethod]
    public void ModeOff_DuringAReplay_EndsIt_AndIsReportedOnlyAfterTheStreamCloses()
    {
      using var rig = new Rig(speed: 1);
      rig.Control.Start(Fixture);
      rig.WaitForCodes("662");

      rig.Control.SetMode(SystemMode.Off);

      Assert.IsTrue(rig.Ended.Wait(Timeout), "the replay did not end");
      rig.WaitForCodes("66236");
      Assert.AreEqual("Mode=Off\nState=Off\nCanPause=false\nCanResume=false", rig.Client.Snapshot()[4].Text);
      Assert.ThrowsException<InvalidOperationException>(() => rig.Control.Start(Fixture), "a start while off");
    }

    [TestMethod]
    public void ModeOn_DuringAClientsReplay_TakesItOverWithoutEndingIt()
    {
      using var rig = new Rig(speed: 1);
      rig.Control.Start(Fixture, AcquisitionRequest.Parse("Kind=Permanent\n"));
      rig.WaitForCodes("662");

      rig.Control.SetMode(SystemMode.On);

      rig.WaitForCodes("6626");
      Assert.AreEqual("Mode=On\nState=Running\nCanPause=true\nCanResume=false", rig.Client.Snapshot()[3].Text);
      Assert.IsTrue(rig.Runner.IsRunning);
      rig.Control.Cancel();
      Assert.IsTrue(rig.Ended.Wait(Timeout));
    }

    [TestMethod]
    public void PauseAndResume_ReportWhichIsPossible_AndTheClientsPausedMode()
    {
      using var rig = new Rig(speed: 1);
      rig.Control.Start(Fixture, AcquisitionRequest.Parse("Kind=Permanent\n"));
      rig.WaitForCodes("662");

      rig.Control.Pause();
      rig.WaitForCodes("6626");
      Assert.AreEqual("Mode=DirectControlPaused\nState=Running\nCanPause=false\nCanResume=true", rig.Client.Snapshot()[3].Text);

      rig.Control.Resume();
      rig.WaitForCodes("66266");
      Assert.AreEqual("Mode=DirectControl\nState=Running\nCanPause=true\nCanResume=false", rig.Client.Snapshot()[4].Text);

      rig.Control.Cancel();
      Assert.IsTrue(rig.Ended.Wait(Timeout), "the replay did not end");
      rig.WaitForCodes("6626636");
      Assert.AreEqual(Idle, rig.Client.Snapshot()[6].Text, "a permanent acquisition stays on when it ends");
    }
  }
}
