using Nova.Data;
using System;

namespace MStreamer
{
  public class MStreamerEventArgs : EventArgs
  {

    public MStreamerEventArgs(SpectrumEx newScan, bool isNative, bool isIAPI)
    {
      Scan = newScan;
      IsIAPI = isIAPI;
      IsNative = isNative;
    }

    private SpectrumEx Scan;
    public bool IsIAPI { get; }  //Indicate if the scan is to be broadcast as IAPI
    public bool IsNative { get; } //Indicate if the scan data/format matches the source file.

    public SpectrumEx GetScan()
    {
      return Scan;
    }

  }
}