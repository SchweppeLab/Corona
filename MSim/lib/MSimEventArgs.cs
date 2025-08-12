using Nova.Data;
using System;

namespace MSim.lib
{
  public class MSimEventArgs : EventArgs
  {

    public MSimEventArgs(SpectrumEx newScan)
    {
      Scan = newScan;
    }

    private SpectrumEx Scan;

    public SpectrumEx GetScan()
    {
      return Scan;
    }

  }
}