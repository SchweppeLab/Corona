using Data2Api.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace Data2Api
{
    public class d2a
    {
        public string DetectorClass => throw new NotImplementedException();

        public event EventHandler<RawEventArgs> MsScanArrived;

        private Scan LastMsScan { get; set; }

        public IMsScan GetLastMsScan()
        {
            return LastMsScan;
        }

        protected virtual void SendMsScanArrived(Scan scan)
        {
            MsScanArrived?.Invoke(this, new RawEventArgs(scan));
        }

        public void Run(string path, int maxNumScans = 100)
        {
            RawReader raw = new RawReader();
            raw.Open(path);
            int scanCount = 0;
            foreach(Scan scan in raw)
            {
                LastMsScan = scan;
                Console.WriteLine("scan: " + scan.ScanNumber);
                SendMsScanArrived(scan);
                scanCount++;
                if(scanCount >= maxNumScans) { return; }
            }
        }
    }
}
