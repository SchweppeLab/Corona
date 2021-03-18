using System;
using System.Collections.Generic;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoBiz = ThermoFisher.CommonCore.Data.Business;

namespace Data2Api.lib
{
    public class ScanHeader
    {

        private Dictionary<string, string> Header;

        public ScanHeader()
        {
            Header = new Dictionary<string, string>()
            {
                { "MassAnalyzer", ""},
                { "IonizationMode", "" },
                { "ScanRate", "" },
                { "ScanMode", "" },
                { "StartTime", "" },
                { "Scan", "" },
                { "TIC", "" },
                { "BasePeakIntensity", "" },
                { "BasePeakMass", "" },
                { "CycleNumber", "" },
                { "Polarity", "" },
                { "Microscans", "" },
                { "InjectTime", "" },
                { "ScanData", "" },
                { "Segments", "" },
                { "Monoisotopic", "" },
                { "MasterScan", "" },
                { "FirstMass", "" },
                { "LastMass", "" },
                { "Checksum", "" },
                { "MSOrder", "" },
                { "Average", "" },
                { "Dependent", "" },
                { "MSX", "" },
                { "SourceFragmentaiton", "" },
                { "SourceFragmentationEnergy", "" },
                { "Resolution", "" },
                { "Access ID", "" },
                { "Temperature Comp. (ppm)", "" },
                { "RF Comp. (ppm)", "" },
                { "Space Charge Comp. (ppm)", "" },
                { "Resolution Comp. (ppm)", "" },
                { "Number of LM Found", "" },
                { "LM Correction (ppm)", "" },
                { "RawOvFtT", "" },
                { "Injection t0", "" },
                { "FAIMS Voltage On", "" },
                { "FAIMS CV", "" },
                { "Scan Description", "" },
                { "AGC", "" },
                { "Micro Scan Count", "" },
                { "Ion Injection Time (ms)", "" },
                { "Elapsed Scan Time (sec)", "" },
                { "Average Scan by Inst", "" },
                { "Orbitrap Resolution", "" },
                { "API Process Delay", "" },
                { "Dependency Type", "" },
                { "Master Scan Number", "" },
                { "API PAGC Scan", "" },
                { "API PAGC Scan Group Index", "" },
                { "Multi Inject Info", "" },
                { "Monoisotopic M/Z", "" },
                { "Charge State", "" },
                { "Error in isotopic envelope fit", "" },
                { "HCD Energy", "" },
                { "HCD Energy V", "" },
                { "MS2 Isolation Width", "" },
                { "SPS Masses", "" },
                { "SPS Masses Continued", "" },
            };
        }

        public Dictionary<string,string> GetHeader()
        {
            return Header;
        }

        public bool TrySetValue(string key, string value)
        {
            if (Header.TryGetValue(key, out string outer))
            {
                Header[key] = value;
                return true;
            }
            return false;
        }
    }
}
