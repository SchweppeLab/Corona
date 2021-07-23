using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace MSim.lib
{
    public class RawEventArgs : MsScanEventArgs
    {

        public RawEventArgs(Scan newScan)
        {
            Scan = newScan;
        }

        private Scan Scan;

        public Scan GetFullScan()
        {
            return Scan;
        }

        public override IMsScan GetScan()
        {
            return Scan;
        }
    }
}
