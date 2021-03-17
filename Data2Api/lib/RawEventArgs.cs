using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override IMsScan GetScan()
        {
            return Scan;
        }
    }
}
