using System.Diagnostics;
using MStreamer;

namespace MStreamer.Tests
{
  // Replays Files/NovaTestFixture.mzML: four scans, numbers 1-4, retention times 0.50, 0.60,
  // 0.90, and 1.00 minutes, MS levels 1, 2, 1, 2. At the maximum speed (100x) a full replay
  // takes about 0.6 s of real time, so every test below runs at 100x unless the gap itself is
  // what the test is about.
  [TestClass]
  public class SimRunnerTests
  {
    private static readonly string FixturePath =
      Path.Combine(AppContext.BaseDirectory, "Files", "NovaTestFixture.mzML");

    private static readonly TimeSpan EndTimeout = TimeSpan.FromSeconds(15);

    // Collects everything SimRunner raises so a test can assert on the whole run after
    // AcquisitionEnd. Run is async void, so the end event is the only completion signal.
    private sealed class Recorder
    {
      public readonly List<int> ScanNumbers = new();
      public readonly List<bool> IsNative = new();
      public readonly List<string> Starts = new();
      public readonly List<string> Ends = new();
      public readonly ManualResetEventSlim Ended = new();
      public readonly ManualResetEventSlim FirstScan = new();

      public Recorder(SimRunner runner)
      {
        runner.AcquisitionStart += info => Starts.Add(info.Name);
        runner.AcquisitionEnd += info => { Ends.Add(info.Name); Ended.Set(); };
        runner.MsScanArrived += (_, e) =>
        {
          ScanNumbers.Add(e.GetScan().ScanNumber);
          IsNative.Add(e.IsNative);
          FirstScan.Set();
        };
      }
    }

    [TestMethod]
    public void FixtureIsPresent()
    {
      Assert.IsTrue(File.Exists(FixturePath), $"Missing fixture {FixturePath}");
    }

    [TestMethod]
    public void FullReplay_DeliversEveryScanInOrderThenEnds()
    {
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(100);

      runner.Run(FixturePath);

      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired");
      CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, rec.ScanNumbers);
      Assert.IsTrue(rec.IsNative.TrueForAll(n => n), "mzML scans are always native");
      CollectionAssert.AreEqual(new[] { FixturePath }, rec.Starts);
      CollectionAssert.AreEqual(new[] { FixturePath }, rec.Ends);
      Assert.IsFalse(runner.IsRunning);
      Assert.AreEqual(4, runner.GetLastMsScan()?.ScanNumber);
    }

    [TestMethod]
    public void FirstAndLastScan_BoundTheReplay()
    {
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(100);

      runner.Run(FixturePath, FirstScan: 2, LastScan: 3);

      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired");
      CollectionAssert.AreEqual(new[] { 2, 3 }, rec.ScanNumbers);
    }

    [TestMethod]
    public void MaxNumScans_StopsAfterThatManyScans()
    {
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(100);

      runner.Run(FixturePath, maxNumScans: 2);

      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired");
      CollectionAssert.AreEqual(new[] { 1, 2 }, rec.ScanNumbers);
    }

    [TestMethod]
    public void SetSpeed_ClampsToOneThroughOneHundred()
    {
      var runner = new SimRunner();
      Assert.AreEqual(1, runner.SetSpeed(0));
      Assert.AreEqual(1, runner.SetSpeed(101));
      Assert.AreEqual(50, runner.SetSpeed(50));
      Assert.AreEqual(50, runner.SetSpeed(-5));
    }

    [TestMethod]
    public void Stop_DuringALongGap_EndsTheRunWithoutWaitingForTheGap()
    {
      // At 1x the first scan (RT 0.50 min) is 30 s away. Stop must end the run well before
      // that; it used to be honored only after the next scan fired.
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(1);

      runner.Run(FixturePath);
      Thread.Sleep(300);
      Assert.IsTrue(runner.IsRunning);
      runner.Stop();

      Assert.IsTrue(rec.Ended.Wait(TimeSpan.FromSeconds(5)), "Stop did not end the run promptly");
      Assert.AreEqual(0, rec.ScanNumbers.Count, "no scan should have been delivered");
      Assert.IsFalse(runner.IsRunning);
    }

    [TestMethod]
    public void Pause_HoldsDelivery_ResumeFinishesTheRun()
    {
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(100);
      // Pause from inside the first scan's handler: the handler runs on the replay thread
      // before the loop's pause check, so the pause is in force before scan 2 is due.
      runner.MsScanArrived += (_, e) => { if (e.GetScan().ScanNumber == 1) runner.Pause(); };

      runner.Run(FixturePath);

      Assert.IsTrue(rec.FirstScan.Wait(EndTimeout), "first scan never arrived");
      // Scan 2 is 0.10 min later, 60 ms at 100x. Wait far longer and expect nothing more.
      Thread.Sleep(600);
      CollectionAssert.AreEqual(new[] { 1 }, rec.ScanNumbers);
      Assert.IsTrue(runner.IsRunning);

      runner.Resume();
      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired after Resume");
      CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, rec.ScanNumbers);
    }

    [TestMethod]
    public void Run_UnreadableFile_RaisesRunErrorThenEnds()
    {
      // Nova 1.1.0 throws SpectrumFileOpenException for a file it cannot open or index,
      // where earlier builds silently produced empty scans. Run must report it and still end.
      string junk = Path.Combine(Path.GetTempPath(), "CoronaJunk_" + Guid.NewGuid().ToString("N") + ".mzML");
      File.WriteAllText(junk, "this is not an mzML file");
      try
      {
        var runner = new SimRunner();
        var rec = new Recorder(runner);
        Exception? reported = null;
        runner.RunError += (_, error) => reported = error;

        runner.Run(junk);

        Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired");
        Assert.IsInstanceOfType(reported, typeof(Nova.Io.Read.SpectrumFileOpenException), reported?.ToString());
        Assert.AreEqual(0, rec.ScanNumbers.Count);
        Assert.AreEqual(1, rec.Starts.Count);
        Assert.IsFalse(runner.IsRunning);
      }
      finally
      {
        File.Delete(junk);
      }
    }

    [TestMethod]
    public void Run_UnknownExtension_RaisesRunErrorThenEnds()
    {
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      Exception? reported = null;
      runner.RunError += (_, error) => reported = error;

      runner.Run(Path.ChangeExtension(FixturePath, ".txt"));

      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired");
      Assert.IsInstanceOfType(reported, typeof(FormatException), reported?.ToString());
      Assert.IsFalse(runner.IsRunning);
    }

    [TestMethod]
    public void MaxRunMinutes_EndsTheRunWhenTheSimulatedClockReachesIt()
    {
      // The limit falls in the gap between the second scan (0.60 min) and the third (0.90 min),
      // so the run ends there without waiting for the third scan to come due.
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(100);

      runner.Run(FixturePath, maxRunMinutes: 0.75);

      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired");
      CollectionAssert.AreEqual(new[] { 1, 2 }, rec.ScanNumbers);
      Assert.IsFalse(runner.IsRunning);
    }

    [TestMethod]
    public void StartDelay_HoldsTheFirstScanUntilReleased()
    {
      // At 100x the first scan is 0.3 s away; a 30 s hold must keep it back until released.
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(100);

      runner.Run(FixturePath, startDelaySeconds: 30);
      Thread.Sleep(800);
      Assert.AreEqual(0, rec.ScanNumbers.Count, "a scan went out during the hold");
      Assert.AreEqual(1, rec.Starts.Count, "the run must start before the hold");
      Assert.IsTrue(runner.IsRunning);

      runner.ReleaseStartDelay();

      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired after the release");
      CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, rec.ScanNumbers);
    }

    [TestMethod]
    public void StartDelay_ExpiresOnItsOwn()
    {
      var runner = new SimRunner();
      var rec = new Recorder(runner);
      runner.SetSpeed(100);
      var clock = Stopwatch.StartNew();

      runner.Run(FixturePath, startDelaySeconds: 0.5);

      Assert.IsTrue(rec.Ended.Wait(EndTimeout), "AcquisitionEnd never fired");
      Assert.IsTrue(clock.ElapsedMilliseconds >= 500, $"the run ended after {clock.ElapsedMilliseconds} ms, inside the hold");
      CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, rec.ScanNumbers);
    }
  }
}
