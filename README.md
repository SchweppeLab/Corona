# msim

msim is a simulator library to use scans from raw files as if they were real-time scans from an instrument accessed through an instrument API ([Thermo](https://github.com/thermofisherlsms/iapi)).

## Usage

The main functionality for msim can be access through the `SimRunner`.

Listerners for API applications can be assigned to EventHandlers within the `SimRunner` that mimic those from the API available through the the `IMSScan` interface.

### Running MSim

msim can be run using the following method:

    SimRunner simRunner = new SimRunner();

    simRunner.AcquisitionStart += _instAcq_AcquisitionStreamOpening;
    simRunner.AcquisitionEnd += _instAcq_AcquisitionStreamClosing;
    simRunner.MsScanArrived += _instMSScanContainer_MsScanArrived;
    
    simRunner.Run(RawFile);


Raw Files are read based on the `Nova` project: https://github.com/SchweppeLab/Nova

## Additional Tools

### VirtualMS

A virtual mass spectrometer application that uses MSim for real-time data broadcast. The VirtualMS accepts a sequence of data files (raw or mzML),
and broadcasts them in real-time, or faster using the data stream speed toggles. Applications developed with `Helios` (https://github.com/SchweppeLab/Helios)
can receive the data from the VirtualMS using an IAPI compatible interface. In this manner, the VirtualMS can be used in place of an instrument
for application development.

### Blazar

A commandline implementation of a real-time raw file datastream, for demonstration of MSim use.

## Additional Information

Authors: `Devin Schweppe` and `Michael Hoopmann`

Copywrite: Authors & Schweppe Lab 2021-2025


