# msim

msim is a simulator library to use scans from raw files as if they were real-time scans from an instrument accessed through and instrument API ([Thermo](https://github.com/thermofisherlsms/iapi)).

## Usage

The main functionality for msim can be access through the `SimRunner`.

Listerners for API applications can be assigned to EventHandlers within the `SimRunner` that mimic those from the API available through the the `IMSScan` interface.

### Running MSim

msim can be run using the following method:

    SimRunner simRunner = new SimRunner();

    _instAcq = simRunner.Acquisition;

    _scans = null;

    simRunner.Acquisition.AcquisitionStreamOpening += _instAcq_AcquisitionStreamOpening;
    simRunner.Acquisition.AcquisitionStreamClosing += _instAcq_AcquisitionStreamClosing;
    
    simRunner.MsScanArrived += _instMSScanContainer_MsScanArrived;
    
    simRunner.Run(RawFile, waitFor: 5);

`waitFor` corresponds to a Thread delay of `n` milliseconds. This is used because the `Run` method is `async`.
Depending on the downstream application this can be tuned to replicate instrument performance (e.g., scan delays due to actual acquisition).

Raw Files are read based on code from the `Monocle` project: https://github.com/gygilab/Monocle

Author: `Devin Schweppe`
Copywrite Schweppe Lab 2021


