using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;

namespace MSim.lib
{
    public class State : IState
    {
        public SystemMode SystemMode { get; set; } = SystemMode.On;

        public InstrumentState SystemState { get; set; } = InstrumentState.Running;
    }
}
