using MSim;
using MSim.lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace d2a_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string rawPath = "";

            if (args.Length >= 1)
            {
                rawPath = args[0];
            }
            else
            {
                rawPath = "C:/Users/Mintaka/Documents/SchweppeLab/Informatics/eca00838.raw";
            }
            Console.WriteLine("Starting.");
            Console.WriteLine("Extention: " + Path.GetExtension(rawPath));
            if (File.Exists(rawPath) && Path.GetExtension(rawPath) == ".raw")
            {
                SimRunner runner = new SimRunner();
                Console.WriteLine("Bind Listeners.");
                runner.MsScanArrived += DCont_MsScanArrived;
                runner.Acquisition.AcquisitionStreamOpening += Acquisition_AcquisitionStreamOpening;
                runner.Acquisition.AcquisitionStreamClosing += Acquisition_AcquisitionStreamClosing;
                runner.Run(rawPath);
            }
            Console.ReadLine();
        }

        private static void Acquisition_AcquisitionStreamClosing(object sender, EventArgs e)
        {
            Console.WriteLine("Stream closing");
        }

        private static void Acquisition_AcquisitionStreamOpening(object sender, Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.AcquisitionOpeningEventArgs e)
        {
            Console.WriteLine("Stream opening");
            Console.WriteLine(e.StartingInformation.First().Key);
        }

        private static void DCont_MsScanArrived(object sender, MsScanEventArgs e)
        {
            Console.WriteLine("Scan Arrived.");
            Scan testScan = (Scan)e.GetScan();
            IMsScan testScan_I = e.GetScan();
            Console.WriteLine(testScan.ScanNumber + " : " + testScan_I.Header["ScanNumber"]);
            Console.WriteLine(">>>>######################");
            foreach(KeyValuePair<string,string> kvp in testScan_I.Header)
            {
                Console.WriteLine(kvp.Key + ": " + kvp.Value);
            }
            Console.WriteLine("######################");
            foreach (string item in testScan_I.Trailer.ItemNames.ToList())
            {
                testScan_I.Trailer.TryGetValue(item, out string value);
                Console.WriteLine(item + ": " + value);
            }
            Console.WriteLine("######################<<<<");
            Console.WriteLine(testScan.Centroids.First().Mz + " - " + testScan.Centroids.Last().Mz);
            Console.WriteLine(testScan_I.Centroids.First().Mz + " - " + testScan_I.Centroids.Last().Mz);
            testScan.Meta.Trailer.TryGetValue("Ion Injection Time (ms):", out string valueTest);
            testScan_I.Trailer.TryGetValue("Ion Injection Time (ms):", out string valueTest1);
            Console.WriteLine(valueTest + " ms - " + valueTest1 + " ms");
        }
    }
}
