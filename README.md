# Corona

Corona is a virtual mass spectrometer with a graphical user interface. It simulates a mass spectrometer by replaying scan data in real-time. Corona connects to 
instrument API ([Thermo](https://github.com/thermofisherlsms/iapi)) applications that are built using the [Helios API](https://github.com/SchweppeLab/Helios), 
providing a means to develop and test real-time MS applications without a mass spectrometer.

The Corona code repository contains three principal code structures, described below.

## VirtualMS

This component contains the Corona software project to produce the Corona virtual mass spectrometer, and depends on the following libraries:

1. msim - contained in this repository, and described below.
2. [Nova](https://github.com/SchweppeLab/Nova) - a library for spectral data management and interprocess communication.
3. [ScottPlot](https://github.com/scottplot/scottplot/) - for spectral visualization.

The libraries required are easily obtained from NuGet (public) or the links above. The binary produced from compilation of this project is
the Corona virtual mass spectrometer. Instructions for use are found [here](https://schweppelab.github.io/Corona/).


## MSim

This source code set contains the mass spectrometer simulator library, named msim. It provides a foundation class for putting an MS simulator into any software. Its job is to 
read and broadcast scans from MS data as if they were real-time scans from an instrument.

### MSim Usage

The main functionality for msim can be access through the `SimRunner`. Listeners assigned to EventHandlers are used to process spectra as they are read from the data file.
Below is an example of how to use msim:

    SimRunner simRunner = new SimRunner();
    simRunner.AcquisitionStart += _instAcq_AcquisitionStreamOpening;
    simRunner.AcquisitionEnd += _instAcq_AcquisitionStreamClosing;
    simRunner.MsScanArrived += _instMSScanContainer_MsScanArrived;
    simRunner.Run(RawFile);

Data files are read in msim using the [Nova](https://github.com/SchweppeLab/Nova) library.


## Blazar

A commandline implementation of a real-time raw file datastream, for demonstration of MSim use.

## Additional Information

Authors: `Devin Schweppe` and `Michael Hoopmann`

Copywrite: Authors & Schweppe Lab 2021-2025


