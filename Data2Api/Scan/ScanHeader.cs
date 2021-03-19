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
            Header = new Dictionary<string, string>();
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
