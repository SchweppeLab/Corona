using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;

namespace Data2Api.lib
{
    //
    // Summary:
    //     This implementation of EventArgs carries information about further scans to be
    //     acquired.
    //
    // Remarks:
    //     An instance of this class will be created by Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.IAcquisition.AcquisitionStreamOpening.
    public class AcquisitionOpeningArgs : AcquisitionOpeningEventArgs
    {
        //
        // Summary:
        //     Create a new Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.AcquisitionOpeningEventArgs.
        public AcquisitionOpeningArgs()
        {
            StartingInformation = new Dictionary<string, string>()
            {
                {"Stream starting info", "Allo!!" }
            };
        }

    }

    public class StateChangedArgs : StateChangedEventArgs
    {

        public State State { get; set; } = new State();

        //
        // Summary:
        //     Create a new Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.AcquisitionOpeningEventArgs.
        public StateChangedArgs()
        {
        }

    }
}
