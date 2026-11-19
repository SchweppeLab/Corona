using Nova.IPC.Pipes;
using Nova.Data;

namespace Corona
{
  /// <summary>
  /// Handler for a client's mode, start or cancel command (pipe messages 'M', 'S' and 'C').
  /// </summary>
  /// <param name="connection">: the client.</param>
  /// <param name="code">: the message code.</param>
  /// <param name="payload">: the message text, Key=Value lines or empty.</param>
  public delegate void ClientCommandEvent(PipesConnection connection, char code, string payload);

  public class VMSServer
  {

    private PipesServer server;
    public event PipeConnectionEvent? ClientConnected;
    public event PipeConnectionEvent? ClientDisconnected;
    public event EventHandler<CustomScan>? CustomScanRequest;
    public event ClientCommandEvent? CommandReceived;

    public VMSServer(string pipeName)
    {
      server = new PipesServer(pipeName);
      server.ClientConnected += OnClientConnected;
      server.ClientDisconnected += OnClientDisconnected;
      server.ClientMessage += OnClientMessage;
      server.Error += OnError;
      server.Start();
    }

    public string IsRunning()
    {
      return server.IsRunning() ? "yes" : "no";
    }

    private void OnClientConnected(PipesConnection connection)
    {
      Console.WriteLine("Client Connected: " + connection.ID);
      ClientConnected?.Invoke(connection);
    }

    private void OnClientDisconnected(PipesConnection connection)
    {
      Console.WriteLine("Client {0} disconnected", connection.ID);
      ClientDisconnected?.Invoke(connection);
    }

    private void OnClientMessage(PipesConnection connection, PipeMessage message)
    {
      //Console.WriteLine("Client {0} says: {1}", connection.ID, message);
      switch (message.MsgCode)
      {
        case '0':
          Console.WriteLine("Client {0} says: {1}", connection.ID, message.DecodeString());
          break;
        case 'A':
          OnCustomScanRequest(message);
          break;
        case 'M':
        case 'S':
        case 'C':
          OnCommand(connection, message);
          break;
        default:
          Console.WriteLine("Server received unrecognized message code from {0}: {1}", connection.ID, message.MsgCode);
          break;
      }
    }

    private void OnCustomScanRequest(PipeMessage message)
    {
      CustomScan cs = new CustomScan();
      try
      {
        cs.Deserialize(message.MsgData);
      }
      catch (Exception ex)
      {
        //Malformed/truncated custom-scan message from a client. Drop it rather than
        //let a bad message from one client take down the server for everyone.
        OnError(ex);
        return;
      }
      CustomScanRequest?.Invoke(this,cs);
    }

    private void OnCommand(PipesConnection connection, PipeMessage message)
    {
      string payload;
      try
      {
        payload = message.MsgData is { Length: > 0 } ? message.DecodeString() : "";
      }
      catch (Exception ex)
      {
        //A text payload that does not decode is dropped, like a malformed custom scan.
        OnError(ex);
        return;
      }
      CommandReceived?.Invoke(connection, message.MsgCode, payload);
    }

    private void OnError(Exception exception)
    {
      Console.Error.WriteLine("ERROR: {0}", exception);
    }

    /// <summary>
    /// Tells every client the instrument's state (message '6').
    /// </summary>
    public void SendStateReport(StateReport report)
    {
      server.Send(Text('6', report.ToPayload()));
    }

    /// <summary>
    /// Tells one client, the one that just connected, the instrument's state.
    /// </summary>
    public void SendStateReport(StateReport report, PipesConnection client)
    {
      client.Send(Text('6', report.ToPayload()));
    }

    public void SendAcquisitionEnd(string name)
    {
      server.Send(Text('3', name));
    }

    public void SendAcquisitionStart(string name)
    {
      server.Send(Text('2', name));
    }

    public void SendCanAcceptNextCustomScan()
    {
      server.Send(Text('5', "a")); //dummy string
    }

    public void SendSpectrum(Spectrum spec)
    {
      PipeMessage pm = new PipeMessage();
      pm.MsgCode = '1';
      pm.MsgData = spec.Serialize();
      server.Send(pm);
    }

    public void SendSpectrum(SpectrumEx spec)
    {
      PipeMessage pm = new PipeMessage();
      pm.MsgCode = '4';
      pm.MsgData = spec.Serialize();
      server.Send(pm);
    }

    public void Stop()
    {
      server.Stop();
    }

    //EncodeString resets the code to '0', so the code goes on afterwards.
    private static PipeMessage Text(char code, string text)
    {
      PipeMessage pm = new PipeMessage();
      pm.EncodeString(text);
      pm.MsgCode = code;
      return pm;
    }

  }

}
