using MSim.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace MSim
{
    public class SimRunner
    {
        public string DetectorClass => throw new NotImplementedException();

        public event EventHandler<RawEventArgs> MsScanArrived;

        private Scan LastMsScan { get; set; }

        public IMsScan GetLastMsScan()
        {
            return LastMsScan;
        }

        public Acquisition Acquisition { get; set; } = new Acquisition();

        protected virtual void SendMsScanArrived(Scan scan)
        {
            MsScanArrived?.Invoke(this, new RawEventArgs(scan));
        }

        private CancellationTokenSource CancellationSource { get; set; } = new CancellationTokenSource();


        public void Stop()
        {
            if (CancellationSource != null && !CancellationSource.IsCancellationRequested)
            {
                CancellationSource.Cancel();
                Thread.Sleep(500);
                CancellationSource.Dispose();
                ResetCancellationTokenSource();
            }
        }

        public void ResetCancellationTokenSource()
        {
            CancellationSource = new CancellationTokenSource();
        }

        public async void Run(string path, int maxNumScans = 100000, int waitFor = 0, int? FirstScan = null, int? LastScan = null)
        {
            await Task.Run(() =>
            {
                Acquisition.SendStreamOpen();
                Acquisition.SendStateChange();
                RawReader raw = new RawReader
                {
                    FirstScanManual = FirstScan,
                    LastScanManual = LastScan
                };
                raw.Open(path);
                int scanCount = 0;
                // needs cancellation tolken!
                foreach (Scan scan in raw)
                {
                    if (CancellationSource.IsCancellationRequested)
                    {
                        return;
                    }
                    LastMsScan = scan;
                    Console.WriteLine("scan: " + scan.ScanNumber);
                    SendMsScanArrived(scan);
                    scanCount++;
                    Thread.Sleep(waitFor);
                    if (scanCount >= maxNumScans)
                    {
                        Acquisition.SendStreamClose();
                        return;
                    }
                }
            });

        }
    }
}
