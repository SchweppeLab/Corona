using System;
using System.Collections.Generic;
using Thermo.Interfaces.SpectrumFormat_V1;
using ThermoBiz = ThermoFisher.CommonCore.Data.Business;

namespace MSim.lib
{
    public class ScanTrailer : IInformationSourceAccess
    {

        private Dictionary<string, string> Trailer;

        public IEnumerable<string> ItemNames {
            get
            {
                return Trailer.Keys;
            }
        }

        public bool Available { get; set; } = true;

        public bool Valid { get; set; } = true;

        public ScanTrailer()
        {
            Trailer = new Dictionary<string, string>()
            {
                { "Access ID:", "" },
                { "Scan Description:", "" },
                { "Ion Injection Time (ms):", "" },
                { "Elapsed Scan Time (sec):", "" },
                { "Charge State:", "" },
                { "Master Scan Number:", "" },
                { "Master Index:", "" },
                { "Monoisotopic M/Z:", "" },
                { "FAIMS CV:", "" },
                { "FAIMS Voltage On:", "" },
                { "SPS Masses:", "" },
            };
        }

        public bool TryGetValue(string key, out string value)
        {
            return Trailer.TryGetValue(key, out value);
        }

        public bool TryGetRawValue(string name, out object value)
        {
            throw new NotImplementedException();
        }

        public void ConsumeLogEntry(ThermoBiz.LogEntry trailer)
        {
            for (int i = 0; i < trailer.Length; i++)
            {
                string value = trailer.Values[i];
                if (value == null)
                {
                    value = "";
                }
                if(Trailer.TryGetValue(trailer.Labels[i],out string outer))
                {
                    Trailer[trailer.Labels[i]] = value.Trim();
                    //Console.WriteLine(trailer.Labels[i] + " " + Trailer[trailer.Labels[i]] + " : " + value);
                }
            }
        }
    }
}
