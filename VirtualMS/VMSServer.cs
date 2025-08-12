using MSim;
using Nova.IPC.Pipes;
using Nova.Data;
using ThermoFisher.CommonCore.Data.Business;

namespace VirtualMS
{

  public class VMSServer
  {

    private PipesServer server;
    public event PipeConnectionEvent? ClientConnected;
    public event PipeConnectionEvent? ClientDisconnected;
    public event EventHandler<CustomScan>? CustomScanRequest;

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
        default:
          Console.WriteLine("Server received unrecognized message code from {0}: {1}", connection.ID, message.MsgCode);
          break;
      }
    }

    private void OnCustomScanRequest(PipeMessage message)
    {
      CustomScan cs = new CustomScan();
      cs.Deserialize(message.MsgData);
      CustomScanRequest?.Invoke(this,cs);
    }

    private void OnError(Exception exception)
    {
      Console.Error.WriteLine("ERROR: {0}", exception);
    }

    public void SendAcquisitionEnd(RunInfo info)
    {
      PipeMessage pm = new PipeMessage();
      pm.EncodeString(info.Name);
      pm.MsgCode = '3';
      server.Send(pm);
    }

    public void SendAcquisitionStart(RunInfo info)
    {
      PipeMessage pm = new PipeMessage();
      pm.EncodeString(info.Name);
      pm.MsgCode = '2';
      server.Send(pm);
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

  }

}
