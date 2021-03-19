using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Modes;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Workflow;

namespace MSim.lib
{
    public class Acquisition : IAcquisition
    {
        public IState State { get; set; } = new State();
        public bool CanPause => throw new NotImplementedException();

        public bool CanResume => throw new NotImplementedException();

        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event EventHandler<AcquisitionOpeningEventArgs> AcquisitionStreamOpening;
        public event EventHandler AcquisitionStreamClosing;

        public virtual void SendStreamOpen()
        {
            AcquisitionStreamOpening?.Invoke(this, new AcquisitionOpeningArgs());
        }

        public virtual void SendStreamClose()
        {
            AcquisitionStreamClosing?.Invoke(this, new EventArgs());
        }

        public virtual void SendStateChange()
        {
            StateChanged?.Invoke(this, new StateChangedArgs());
        }

        public void CancelAcquisition()
        {
            throw new NotImplementedException();
        }

        public IAcquisitionLimitedByCount CreateAcquisitionLimitedByCount(int count)
        {
            throw new NotImplementedException();
        }

        public IAcquisitionLimitedByTime CreateAcquisitionLimitedByDuration(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public IForcedOffMode CreateForcedOffMode()
        {
            throw new NotImplementedException();
        }

        public IForcedStandbyMode CreateForcedStandbyMode()
        {
            throw new NotImplementedException();
        }

        public IAcquisitionMethodRun CreateMethodAcquisition(string methodFileName)
        {
            throw new NotImplementedException();
        }

        public IAcquisitionMethodRun CreateMethodAcquisition(string methodFileName, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public IOffMode CreateOffMode()
        {
            throw new NotImplementedException();
        }

        public IOnMode CreateOnMode()
        {
            throw new NotImplementedException();
        }

        public IAcquisitionWorkflow CreatePermanentAcquisition()
        {
            throw new NotImplementedException();
        }

        public IStandbyMode CreateStandbyMode()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void SetMode(IMode newMode)
        {
            throw new NotImplementedException();
        }

        public void StartAcquisition(IAcquisitionWorkflow acquisition)
        {
            throw new NotImplementedException();
        }

        public bool WaitFor(TimeSpan duration, Func<InstrumentState, SystemMode, bool> continuation)
        {
            throw new NotImplementedException();
        }
    }
}
