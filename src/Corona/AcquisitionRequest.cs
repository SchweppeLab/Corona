using System.Globalization;

namespace Corona
{
  /// <summary>
  /// The kinds of acquisition a client can start, as the IAPI names them.
  /// </summary>
  public enum AcquisitionKind
  {
    Permanent,
    LimitedByCount,
    LimitedByTime,
    MethodRun,
  }

  /// <summary>
  /// What the instrument does once a client's acquisition ends, as the IAPI names it.
  /// </summary>
  public enum AcquisitionContinuation
  {
    StayOn,
    Standby,
    Off,
  }

  /// <summary>
  /// A client's request to start an acquisition, pipe message 'S': the IAPI acquisition
  /// workflow as Helios serializes it. Corona honors the kind's limit, the raw file name, the
  /// start delay and the continuation; the rest is logged.
  /// </summary>
  public sealed class AcquisitionRequest
  {
    public AcquisitionKind Kind { get; init; }
    public string RawFileName { get; init; } = "";
    public string SampleName { get; init; } = "";
    public string Comment { get; init; } = "";
    public AcquisitionContinuation Continuation { get; init; } = AcquisitionContinuation.StayOn;
    /// <summary>Seconds to hold the first scan back, so the client can place a custom scan first.</summary>
    public double SingleProcessingDelay { get; init; }
    public bool WaitForContactClosure { get; init; }
    /// <summary>LimitedByCount: the number of scans to take.</summary>
    public int NumberOfScans { get; init; }
    /// <summary>LimitedByTime: how long to acquire; MethodRun: a duration in place of the method's own.</summary>
    public TimeSpan? Duration { get; init; }
    public string MethodName { get; init; } = "";

    /// <summary>
    /// Parses the Key=Value payload. Throws FormatException when Kind is missing or unknown, or a
    /// value does not read; keys it does not know are ignored.
    /// </summary>
    public static AcquisitionRequest Parse(string payload)
    {
      Dictionary<string, string> values = KeyValueLines.Parse(payload);

      if (!values.TryGetValue("Kind", out string? kindText) || !Enum.TryParse(kindText, true, out AcquisitionKind kind))
        throw new FormatException($"unknown acquisition kind '{kindText}'");

      AcquisitionContinuation continuation = AcquisitionContinuation.StayOn;
      if (values.TryGetValue("Continuation", out string? continuationText) && !Enum.TryParse(continuationText, true, out continuation))
        throw new FormatException($"unknown continuation '{continuationText}'");

      TimeSpan? duration = null;
      if (values.TryGetValue("Duration", out string? durationText))
      {
        if (!TimeSpan.TryParseExact(durationText, "c", CultureInfo.InvariantCulture, out TimeSpan span))
          throw new FormatException($"unreadable Duration '{durationText}'");
        duration = span;
      }

      return new AcquisitionRequest
      {
        Kind = kind,
        RawFileName = values.GetValueOrDefault("RawFileName", ""),
        SampleName = values.GetValueOrDefault("SampleName", ""),
        Comment = values.GetValueOrDefault("Comment", ""),
        Continuation = continuation,
        SingleProcessingDelay = Number(values, "SingleProcessingDelay"),
        WaitForContactClosure = Flag(values, "WaitForContactClosure"),
        NumberOfScans = Count(values, "NumberOfScans"),
        Duration = duration,
        MethodName = values.GetValueOrDefault("MethodName", ""),
      };
    }

    private static double Number(Dictionary<string, string> values, string key)
    {
      if (!values.TryGetValue(key, out string? text)) return 0;
      if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)) return number;
      throw new FormatException($"unreadable {key} '{text}'");
    }

    private static int Count(Dictionary<string, string> values, string key)
    {
      if (!values.TryGetValue(key, out string? text)) return 0;
      if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count)) return count;
      throw new FormatException($"unreadable {key} '{text}'");
    }

    private static bool Flag(Dictionary<string, string> values, string key)
    {
      if (!values.TryGetValue(key, out string? text)) return false;
      if (bool.TryParse(text, out bool flag)) return flag;
      throw new FormatException($"unreadable {key} '{text}'");
    }

    /// <summary>
    /// A one-line summary for the log.
    /// </summary>
    public override string ToString()
    {
      var parts = new List<string> { Kind.ToString() };
      if (Kind == AcquisitionKind.LimitedByCount) parts.Add($"{NumberOfScans} scans");
      if (Duration is TimeSpan d) parts.Add($"{d.TotalMinutes:0.##} min");
      if (Kind == AcquisitionKind.MethodRun) parts.Add($"method {MethodName}");
      if (RawFileName.Length > 0) parts.Add($"raw file {RawFileName}");
      if (SampleName.Length > 0) parts.Add($"sample {SampleName}");
      if (SingleProcessingDelay > 0) parts.Add($"{SingleProcessingDelay:0.##} s start delay");
      if (WaitForContactClosure) parts.Add("wait for contact closure");
      parts.Add($"then {Continuation}");
      return string.Join(", ", parts);
    }
  }

  /// <summary>
  /// A client's request to set the mode, pipe message 'M'.
  /// </summary>
  public static class ModeRequest
  {
    /// <summary>
    /// Reads Mode=&lt;name&gt;; true only for a mode the instrument can be put in.
    /// </summary>
    public static bool TryParse(string payload, out SystemMode mode)
    {
      mode = default;
      return KeyValueLines.Parse(payload).TryGetValue("Mode", out string? text)
        && Enum.TryParse(text, true, out mode)
        && Array.IndexOf(InstrumentStatus.SelectableModes, mode) >= 0;
    }
  }

  /// <summary>
  /// The protocol's text payloads: Key=Value lines. The first '=' splits a line, both sides are
  /// trimmed, a line without '=' is skipped, and keys match without regard to case.
  /// </summary>
  public static class KeyValueLines
  {
    public static Dictionary<string, string> Parse(string payload)
    {
      var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      foreach (string line in payload.Split('\n'))
      {
        int eq = line.IndexOf('=');
        if (eq <= 0) continue;
        values[line[..eq].Trim()] = line[(eq + 1)..].Trim();
      }
      return values;
    }
  }
}
