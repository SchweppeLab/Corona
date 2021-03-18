using Data2Api;
using Data2Api.lib;
using System;
using System.IO;
using System.Linq;
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
            Scan testScan = e.GetFullScan();
            IMsScan testScan_I = e.GetScan();
            Console.WriteLine(testScan.ScanNumber + " : " + testScan_I.Header["Scan"]);
            Console.WriteLine(testScan.Centroids.First().Mz + " - " + testScan.Centroids.Last().Mz);
            Console.WriteLine(testScan_I.Centroids.First().Mz + " - " + testScan_I.Centroids.Last().Mz);
            testScan.MetaInformation.Trailer.TryGetValue("Ion Injection Time (ms):", out string valueTest);
            testScan_I.Trailer.TryGetValue("Ion Injection Time (ms):", out string valueTest1);
            Console.WriteLine(valueTest + " ms - " + valueTest1 + " ms");
        }

        private static void DCont_MsScanArrived(object sender, MsScanEventArgs e)
        {
            Console.WriteLine("Scan Arrived..");
            Scan testScan = (Scan)e.GetScan();
            Console.WriteLine(testScan.ScanNumber);
        }
    }
}
