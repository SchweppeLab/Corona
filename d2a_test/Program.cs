using System;
using System.IO;
using Data2Api;
using Data2Api.lib;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace d2a_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string rawPath = args[0];
            Console.WriteLine("Starting.");
            Console.WriteLine("Extention: " + Path.GetExtension(rawPath));
            if (File.Exists(rawPath) && Path.GetExtension(rawPath) == ".raw")
            {
                d2a dCont = new d2a();
                Console.WriteLine("Bind Listener.");
                dCont.MsScanArrived += DCont_MsScanArrived;
                dCont.Run(rawPath);
            }
        }

        private static void DCont_MsScanArrived(object sender, RawEventArgs e)
        {
            Console.WriteLine("Scan Arrived.");
            Scan testScan = (Scan)e.GetScan();
            Console.WriteLine(testScan == null);
            Console.WriteLine(testScan.Header.Count);
        }

        private static void DCont_MsScanArrived(object sender, MsScanEventArgs e)
        {
            Console.WriteLine("Scan Arrived..");
            Scan testScan = (Scan)e.GetScan();
            Console.WriteLine(testScan == null);
            Console.WriteLine(testScan.ScanNumber);
        }
    }
}
