using Nova.IPC.Pipes;

namespace Corona.Tests
{
  // Drives VMSServer over a real named pipe with Nova's own PipesClient, the way a Helios
  // client does. The pipe name is GUID-derived: named pipes are a process-wide namespace, so a
  // fixed name would collide with a running Corona or a parallel test run.
  [TestClass]
  public class VMSServerTests
  {
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    [TestMethod]
    public void MalformedCustomScanRequest_IsDroppedAndTheServerKeepsServing()
    {
      string pipe = "CoronaTestPipe_" + Guid.NewGuid().ToString("N");
      var server = new VMSServer(pipe);
      var connected = new ManualResetEventSlim();
      var requested = new ManualResetEventSlim();
      var requests = new List<CustomScan>();
      server.ClientConnected += _ => connected.Set();
      server.CustomScanRequest += (_, scan) => { requests.Add(scan); requested.Set(); };

      var client = new PipesClient(pipe);
      try
      {
        client.Start();
        Assert.IsTrue(connected.Wait(Timeout), "server never saw the client connect");
        client.WaitForConnection((int)Timeout.TotalMilliseconds);

        // A pair count of 5 followed by one byte: Deserialize throws on the first string.
        var malformed = new PipeMessage { MsgCode = 'A', MsgData = new byte[] { 0x05, 0x00, 0x00, 0x00, 0x41 } };
        client.Send(malformed);

        var wellFormed = new PipeMessage { MsgCode = 'A', MsgData = CustomScanTests.Encode(("ScanType", "Full")) };
        client.Send(wellFormed);

        Assert.IsTrue(requested.Wait(Timeout), "server stopped serving after the malformed request");
        Assert.AreEqual(1, requests.Count, "the malformed request must not surface as a request");
        Assert.AreEqual("Full", requests[0].Values["ScanType"]);
        Assert.AreEqual("yes", server.IsRunning());
      }
      finally
      {
        client.AutoReconnect = false;
        client.Stop();
        server.Stop();
      }
    }
  }
}
