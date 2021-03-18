using ThermoBiz = ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;
using System.Text.RegularExpressions;

namespace Data2Api.lib
{
    public class ScanMeta
    {
        public ScanHeader Header { get; set; } = new ScanHeader();

        public ScanTrailer Trailer { get; set; } = new ScanTrailer();

    }
}
