using System;
using System.Collections.Generic;
using System.Linq;
using ThermoBiz = ThermoFisher.CommonCore.Data.Business;

namespace Data2Api.lib
{
    public class ScanTrailer
    {

        private Dictionary<string, string> Trailer;

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

        public void FromRaw(ThermoBiz.LogEntry trailer)
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
                }
            }
        }
    }
}
