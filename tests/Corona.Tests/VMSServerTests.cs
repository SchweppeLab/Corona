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

    private static string NewPipeName() => "CoronaTestPipe_" + Guid.NewGuid().ToString("N");

    [TestMethod]
    public void MalformedCustomScanRequest_IsDroppedAndTheServerKeepsServing()
    {
      string pipe = NewPipeName();
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

    [TestMethod]
    public void Commands_ArriveWithTheirCodeAndPayload()
    {
      string pipe = NewPipeName();
      var server = new VMSServer(pipe);
      var commands = new List<(char Code, string Payload)>();
      var third = new ManualResetEventSlim();
      server.CommandReceived += (_, code, payload) => { commands.Add((code, payload)); if (commands.Count == 3) third.Set(); };
      using var client = new PipeRecorder(pipe);
      try
      {
        // The start payload as Helios serializes it; the comment is the one its probe sends.
        string start = "Kind=Permanent\nRawFileName=probe.raw\nSampleName=\nComment=a comment; with = signs\nContinuation=StayOn\nSingleProcessingDelay=0\nWaitForContactClosure=False\n";
        client.Send('M', "Mode=On");
        client.Send('S', start);
        client.Send('C', "");

        Assert.IsTrue(third.Wait(Timeout), "received only " + string.Concat(commands.Select(c => c.Code)));
        CollectionAssert.AreEqual(new[] { ('M', "Mode=On"), ('S', start), ('C', "") }, commands);
      }
      finally
      {
        server.Stop();
      }
    }

    [TestMethod]
    public void UnknownCode_IsIgnoredAndTheNextCommandStillArrives()
    {
      string pipe = NewPipeName();
      var server = new VMSServer(pipe);
      var commands = new List<char>();
      var received = new ManualResetEventSlim();
      server.CommandReceived += (_, code, _) => { commands.Add(code); received.Set(); };
      using var client = new PipeRecorder(pipe);
      try
      {
        client.Send('Z', "not a message Corona knows");
        client.Send('C', "");

        Assert.IsTrue(received.Wait(Timeout), "the command after the unknown code never arrived");
        CollectionAssert.AreEqual(new[] { 'C' }, commands);
        Assert.AreEqual("yes", server.IsRunning());
      }
      finally
      {
        server.Stop();
      }
    }
  }
}
