# Corona

Corona is a virtual mass spectrometer with a graphical user interface. It simulates a mass spectrometer by replaying scan data in real-time. Corona connects to
instrument API ([Thermo](https://github.com/thermofisherlsms/iapi)) applications that are built using the [Helios API](https://github.com/SchweppeLab/Helios),
providing a means to develop and test real-time MS applications without a mass spectrometer.

Instructions for use are found [here](https://schweppelab.github.io/Corona/). Compiled releases are on the
[Releases](https://github.com/SchweppeLab/Corona/releases) page.

## Repository layout

```
Corona.sln              the whole repo; needs Visual Studio's MSBuild (see Building)
Directory.Build.props   the one version number and the metadata every project shares
Directory.Packages.props  every package version
src/MStreamer/          library: replays an MS data file as a live acquisition (NuGet package MStreamer)
src/Corona/             the Corona WPF application
src/Corona.Launcher/    native stub that is the release root's Corona.exe
src/Blazar/             console demonstration of MStreamer
src/Blazar.Launcher/    native stub that is the release root's Blazar.exe
src/Launcher/           the one launcher source both stubs compile
nuget/                  vendored packages (see nuget/README.md)
docs/                   repo-local documentation; the published site lives on the gh-pages branch
```

## Building

Windows, Visual Studio 2022 with the "Desktop development with C++" workload (v143 toolset)
and the .NET 8 SDK.

The library builds with the plain `dotnet` CLI:

```
dotnet build src\MStreamer\MStreamer.csproj -c Release
```

The full solution, and therefore the release folders, needs Visual Studio's `MSBuild.exe`,
because the two launchers are native C++:

```
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" `
  Corona.sln /restore /p:Configuration=Release /p:Platform=x64
```

### Packages

Nova and Nova.IO are Schweppe Lab libraries that are not on nuget.org (the `Nova` package
there is unrelated software). Download the bundle for the version pinned in
`Directory.Packages.props` from https://github.com/SchweppeLab/Nova/releases, extract it, and
register the extracted folder and its `ThermoRawFileReader` subfolder as NuGet sources:

```
dotnet nuget add source <extracted folder> -n Nova
dotnet nuget add source <extracted folder>\ThermoRawFileReader -n ThermoRawFileReader
```

Everything else comes from nuget.org or from the `nuget/` folder in this repo.

## Release layout

The build output is the release folder. `src\Corona\bin\x64\Release\` holds exactly
`Corona.exe` (a tiny native launcher) and `app\` (the application and every DLL in the flat
layout the .NET host expects). Zip that folder as-is; `src\Blazar\bin\x64\Release\` is the
same shape for Blazar. Both require the
[.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64); if it is
missing, starting the application shows a dialog with the download link.

## MStreamer

Capable of reading an MS spectrum file and streaming the spectra within at their designated retention times. Its job is to
read and broadcast scans from MS data as if they were real-time scans from an instrument.

### MStreamer Usage

The main functionality for MStreamer can be accessed through the `SimRunner`. Listeners assigned to EventHandlers are used to process spectra as they are read from the data file.
Below is an example of how to use it:

    SimRunner simRunner = new SimRunner();
    simRunner.AcquisitionStart += _instAcq_AcquisitionStreamOpening;
    simRunner.AcquisitionEnd += _instAcq_AcquisitionStreamClosing;
    simRunner.MsScanArrived += _instMSScanContainer_MsScanArrived;
    simRunner.Run(RawFile);

Data files are read in MStreamer using the [Nova](https://github.com/SchweppeLab/Nova) library.

## Blazar

A commandline implementation of a real-time raw file datastream, for demonstration of MStreamer use.

## Additional Information

Authors: `Devin Schweppe` and `Michael Hoopmann`

Copyright: Authors & Schweppe Lab 2021-2026

Cite:
Hoopmann, M. R.; McGann, C. D.; Canterbury, J. D.; Barshop, W. D.; Yu, Q.; Gadzuk-Shea, M.; Hogrebe, A.; Villén, J.; Schweppe, D. K. "Corona: A Virtual Mass Spectrometer for the Development of Real-Time Mass Spectrometry Software." Anal. Chem. 2026, ASAP. DOI: 10.1021/acs.analchem.6c01637. [Link](https://pubmed.ncbi.nlm.nih.gov/42308194/)
