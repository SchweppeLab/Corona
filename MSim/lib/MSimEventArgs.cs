using Nova.Data.Spectrum;
using System;

namespace MSim.lib
{
  public class MSimEventArgs : EventArgs
  {

    public MSimEventArgs(Spectrum newScan)
    {
      Scan = newScan;
    }

    private Spectrum Scan;

    public Spectrum GetScan()
    {
      return Scan;
    }

  }
}