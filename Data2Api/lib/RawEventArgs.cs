using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace Data2Api.lib
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
