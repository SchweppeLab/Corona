using Nova.IPC.Pipes;

namespace Corona.Tests
{
  // A Nova pipe client that records every message a VMSServer sends it, as a Helios client
  // receives them, and can wait for the record to satisfy a condition.
  internal sealed class PipeRecorder : IDisposable
  {
    private readonly List<(char Code, string Text)> messages = new();
    private readonly PipesClient client;
    private readonly object gate = new();

    public PipeRecorder(string pipe)
    {
      client = new PipesClient(pipe);
      client.ServerMessage += (_, message) =>
      {
        string text = message.MsgCode is '0' or '2' or '3' or '6' ? message.DecodeString() : "";
        lock (gate)
        {
          messages.Add((message.MsgCode, text));
          Monitor.PulseAll(gate);
        }
      };
      client.Start();
      client.WaitForConnection(5000);
    }

    public (char Code, string Text)[] Snapshot()
    {
      lock (gate) return messages.ToArray();
    }

    // The message codes received so far, in order, as one string.
    public string Codes() => string.Concat(Snapshot().Select(m => m.Code));

    public bool WaitUntil(Func<IReadOnlyList<(char Code, string Text)>, bool> condition, TimeSpan timeout)
    {
      DateTime deadline = DateTime.UtcNow + timeout;
      lock (gate)
      {
        while (!condition(messages))
        {
          TimeSpan left = deadline - DateTime.UtcNow;
          if (left <= TimeSpan.Zero || !Monitor.Wait(gate, left)) return false;
        }
        return true;
      }
    }

    public void Send(char code, string text)
    {
      var message = new PipeMessage();
      message.EncodeString(text);
      message.MsgCode = code;
      client.Send(message);
    }

    public void Dispose()
    {
      client.AutoReconnect = false;
      client.Stop();
    }
  }
}
