using Nova.Data;

namespace MStreamer
{
  /// <summary>
  /// Carries one acquired scan to MsScanArrived handlers.
  /// </summary>
  public class MStreamerEventArgs : EventArgs
  {

    /// <summary>
    /// Creates the arguments for one acquired scan.
    /// </summary>
    /// <param name="newScan">: the scan acquired.</param>
    /// <param name="isNative">: true when the scan data/format matches the source file.</param>
    /// <param name="isIAPI">: true when the scan is to be broadcast as IAPI.</param>
    public MStreamerEventArgs(SpectrumEx newScan, bool isNative, bool isIAPI)
    {
      Scan = newScan;
      IsIAPI = isIAPI;
      IsNative = isNative;
    }

    private SpectrumEx Scan;

    /// <summary>
    /// Indicates if the scan is to be broadcast as IAPI.
    /// </summary>
    public bool IsIAPI { get; }

    /// <summary>
    /// Indicates if the scan data/format matches the source file.
    /// </summary>
    public bool IsNative { get; }

    /// <summary>
    /// The scan acquired.
    /// </summary>
    /// <returns>Spectrum object</returns>
    public SpectrumEx GetScan()
    {
      return Scan;
    }

  }
}
