# CLAUDE.md

Context for Claude Code (or any future contributor) working in the Corona repository.

## What Corona is

A virtual mass spectrometer. `MStreamer` replays an MS data file (Thermo RAW, mzML, mzXML)
scan by scan at the original retention times; `Corona` wraps it in a WPF app and serves the
scans over Nova named pipes to Helios-based clients (Booster, for one) exactly as an
instrument would, including custom-scan requests coming back; `Blazar` is a console demo of
MStreamer. Published docs: https://schweppelab.github.io/Corona/ (built from the separate
`gh-pages` branch, unrelated to `docs/` here).

## Repo layout

```
Corona.sln                 x64 only; needs Visual Studio's MSBuild (see Build)
Directory.Build.props      the one Version (1.1.0) and shared metadata; vcxproj files import it too
Directory.Packages.props   every package version (central package management)
nuget.config / nuget/      vendored packages for private-repo dependencies (LandmineUI); no <clear/>
src/MStreamer/             net8.0 library, AnyCPU, packable as MStreamer. SimRunner is the whole API.
src/Corona/                net8.0-windows WPF app, x64, win-x64, builds into bin\x64\<Config>\app\
src/Corona.Launcher/       native Win32 stub -> the release root's Corona.exe
src/Blazar/                net8.0 console app, same layout and launcher pattern
src/Blazar.Launcher/       native console stub -> the release root's Blazar.exe
src/Launcher/              launcher.cpp, shared by both launchers via each one's launcher_config.h
tests/MStreamer.Tests/     MSTest; replays Files/NovaTestFixture.mzML (4 scans, RT 0.5-1.0 min)
tests/Corona.Tests/        MSTest; CustomScan parsing and VMSServer over a real named pipe
.github/workflows/         ci.yml, dev-build.yml, release.yml; scripts/Get-NovaFeed.ps1
docs/ROADMAP.md            the current plan of record and its status
```

## Build

```
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" `
  Corona.sln /restore /p:Configuration=Release /p:Platform=x64
```

- Only `Release|x64` is exercised. `Directory.Build.props` defaults `Platform` to x64, so a
  bare `dotnet build` on one project lands in the same `bin\x64\` tree.
- The full solution needs `MSBuild.exe` because the launchers are vcxproj files; the dotnet
  CLI cannot build those. The launcher `ProjectReference`s are conditioned on
  `$(MSBuildRuntimeType) != 'Core'` so `dotnet build`/`dotnet test` of a single project still
  works, just without producing the root exe.
- The build is warning-free. The one suppressed warning (NU1701, SkiaSharp.Views.WPF) has its
  reason in `Corona.csproj`; do not add another suppression without one.
- `MStreamer` generates XML docs, so every public member needs a `///` summary (CS1591).

## Tests

```
dotnet test tests\MStreamer.Tests\MStreamer.Tests.csproj -c Release --no-build
dotnet test tests\Corona.Tests\Corona.Tests.csproj -c Release --no-build
```

45 tests as of 2026-11-19, all passing. `--no-build` works right after a solution build because
`Platform` defaults to x64; without it the dotnet CLI rebuilds the test projects itself.
`Corona.Tests` links the MStreamer fixture for its acquisition tests, which replay it over a
real pipe.
`SimRunner.Run` is `async void`, so SimRunner tests wait on `AcquisitionEnd` with a timeout;
the fixture replays in about 0.6 s at 100x. The pipe test uses a GUID-derived pipe name so it
never collides with a running Corona.

## CI/CD

Three workflows, the same shape as Nova's (`D:\Software\Claude\Nova\.github\workflows`):

- `ci.yml`: build + test on PRs to `main`/`Dev` and pushes to `main`. No packaging.
- `dev-build.yml`: push to `Dev` or manual. Builds with `/p:Version=<Version>-dev.<run>`,
  tests, zips `src\Corona\bin\x64\Release` and `src\Blazar\bin\x64\Release` minus `.pdb`
  files as `Corona_v<v>.zip`/`Blazar_v<v>.zip`, packs `MStreamer.<v>.nupkg`, publishes a dated
  pre-release (`dev-<run>-<sha>`) and the rolling `dev-latest` (tag force-moved, assets replaced).
- `release.yml`: manual, refuses anything but `main`, real version, refuses to clobber a
  promoted release, publishes `v<version>` as a pre-release for manual promotion.
- All three run `.github/scripts/Get-NovaFeed.ps1` first: it reads the Nova version from
  `Directory.Packages.props` and downloads that release bundle from SchweppeLab/Nova (official
  `v<x.y.z>` or the dev release carrying `Nova.<version>.zip`), registering the bundle and its
  `ThermoRawFileReader/` folder as NuGet sources. If `nuget/Nova.<version>.nupkg` exists, it
  uses that instead.
- The full solution builds with `msbuild` (microsoft/setup-msbuild) because of the launchers;
  tests run with `dotnet test --no-build` afterwards.
- To reproduce CI locally, extract a Nova bundle and pass
  `/p:RestoreSources="https://api.nuget.org/v3/index.json;<bundle>;<bundle>\ThermoRawFileReader;<repo>\nuget"`
  and `/p:RestorePackagesPath=<empty folder>` to the msbuild restore.

## Theming (LandmineUI)

`App.OnStartup` merges `ThemeResources.Build(tokens, mode)` first, then LandmineUI.Wpf's
`Controls/<Name>.xaml` dictionaries for every stock control Corona uses (list in
`App.xaml.cs`; a control missing there keeps WPF's own look, which fails in dark themes).
`MainWindow` takes `SharpWindowStyle` (custom chrome) and `BackgroundBrush`. `App.ApplyTheme`
swaps the theme dictionary and raises `App.ThemeChanged`; the only listener is
`MainWindow.StylePlots`, because ScottPlot paints its own surface and reads colors through
`ChartPalette`. The theme picker lives outside the ToolBar on purpose (inside one, WPF's own
toolbar styles win). `AppSettings` persists the choice under `%AppData%\Corona`.

LandmineUI's repo is private; its packages are vendored under `nuget/` at the version in
`Directory.Packages.props` (1.4.0 as of 2026-11-03, twelve themes; the owner drops
new builds into `D:\Software\NuGet\LandmineUI`). LandmineUI buttons default to 32 px tall
with 16 px side padding; small utility buttons (the custom-scan stepper) set explicit
`Height`/`Padding` rather than inheriting that. Anything Corona needs that LandmineUI lacks is a wish file in
`D:\Software\Claude\Wishes\LandmineUI\` (addressed with honorifics), never an edit to
LandmineUI.

The toolbar glyphs are Material Symbols geometries in `src/Corona/Icons.xaml` (the SVG
sources sit beside them in `gfx/`), drawn by the `ToolBarIcon` template in the button's
Foreground, so they follow every theme and the disabled state. A new toolbar button gets a
geometry there, not a bitmap. Two LandmineUI style facts that bit once: its TextBox is a
single 32 px line unless `AcceptsReturn` is set (the Message Log sets it), and its TabControl
honors `TabStripPlacement` only from 1.4.0 on (granted as a Corona wish; the lower
TabControl's `Bottom` relies on it). The status bar holds the instrument's mode selector and
its mode and state (see Instrument state).

## Instrument state (the Helios protocol)

Corona replicates an instrument. Helios's VMS backend relays what Corona reports and never
fabricates state, so whatever Corona reports is what every Helios client sees. The wire
contract is `docs/corona-pipe-protocol.md` in the Helios repo (`D:\Software\Claude\Helios`);
both sides change together.

- `InstrumentStatus` (`InstrumentState.cs`) holds the IAPI mode and state and whether a pause
  or resume is possible, and allows only the transitions an instrument makes. Report only a
  status Corona is in: when Corona cannot do what an instrument does (contact closure,
  methods), leave the status alone and log it rather than fake a transition.
- `AcquisitionControl` is the one path for every start, pause, cancel and mode change, whether
  a Corona button or a client asked. It sends message `6` to a client on connect and to all
  clients on every change, ordered around the stream messages: a start is `6` Running then
  `2`, an end is `3` then `6` idle. It subscribes to `SimRunner` before the view model does so
  that order holds.
- A client's start (`S`) replays the file at the head of the queue, one acquisition per start,
  under `DirectControl`, honoring the scan-count or duration limit, the start delay (which a
  custom scan ends early), the raw file name (message `2`) and the continuation. It is
  refused, with the status unchanged and a log line, unless the instrument is On and idle and
  a file is queued. The Run button plays the whole queue, in mode On.
- A mode other than On (`M`, or the status bar's selector) ends a running replay first and is
  reported after the stream closes. Run is disabled unless the mode is On.
- `PipeMessage.EncodeString` resets `MsgCode` to `'0'`; `VMSServer.Text` sets the code after.
  Unknown message codes are ignored and malformed payloads dropped, never crashing dispatch.
- `AcquisitionControlTests` watch the whole sequence from a Nova pipe client; the Helios
  in-process probe in `D:\Software\Claude\Notes\Corona-State-Protocol-Instructions.md` is the
  end-to-end check.

## Release layout: do not flatten it

The build output **is** the release folder: `src\Corona\bin\x64\Release\` holds exactly
`Corona.exe` plus `app\` (the app's own `Corona.exe`, every DLL, only win-x64 natives). Same
for Blazar.
Zip as-is. The launcher exists precisely so nothing about assembly loading had to change; do
not move DLLs out of `app\` or add a custom assembly resolver. The .NET 8 Desktop Runtime is a
prerequisite by design (no self-contained publish, no installer). This follows Scops
(`D:\Software\Claude\Scops`, branch Dev), where the alternatives were weighed and rejected.

- Corona's launcher starts the app and exits; Blazar's shares the console, waits, and returns
  the app's exit code, and keeps the caller's working directory.
- **The app's process name is a contract.** Helios finds Corona by a running process named
  exactly `Corona`, so the managed app is `app\Corona.exe` (`AssemblyName` `Corona`), the same
  name as the root launcher. Do not rename it the way Scops names `Scops.App`: a `Corona.App`
  build left every Helios client unable to find Corona.
- `app\Corona.exe` and the root `Corona.exe` set the same AppUserModelID (`SchweppeLab.Corona`, in
  `src\Corona\AppUserModelId.cs` and `src\Corona.Launcher\launcher_config.h`); keep them in step.
- The launcher VERSIONINFO is generated at build time from `$(Version)`, so there is no
  second copy of the version anywhere. Bump `Directory.Build.props` only.

## Packages

- Nova and Nova.IO are not on nuget.org; the `Nova` package there is **unrelated software by
  someone else**. Never compare against it or suggest it as a source.
- CI downloads the bundle matching `Directory.Packages.props` from
  https://github.com/SchweppeLab/Nova/releases (it includes the pinned Thermo RawFileReader
  packages). Locally the machine-level NuGet.Config feeds serve them.
- **A local pass can mask a CI failure.** This machine's global package cache and local feeds
  can satisfy a restore that CI cannot (it did for Thermo 8.0.37, which the local feed does not
  even hold). Before trusting a packaging change, restore into a fresh packages folder with
  only nuget.org and the Nova bundle as sources.
- Never add a `using ThermoFisher.*` to this repo. Three such usings compiled only through
  Nova.IO's transitive packages and were removed in the 2026-10 restructure; the same pattern
  broke Nova's CI for two months when the transitive tree changed.

## Versioning and branches

- One version for everything, in `Directory.Build.props`. CI passes `/p:Version=<it>-dev.<run>`
  for Dev builds. Release tags are `v<version>`; release assets keep the historical
  `Corona_v1_1_0.zip` naming because the gh-pages download page links to that pattern.
- `Dev` is the permanent development branch; `main` only receives merges at stable points and
  `Dev` lives on afterwards.

## Working conventions

- 2-space indentation in every language (C#, XAML, C++, XML, YAML). `.editorconfig` enforces it.
- Nullable and implicit usings are on everywhere. Fix nullable warnings at the source; do not
  suppress them.
- Comments say what the code does not already say, in American English, with no history in
  them (history goes in commit messages and `docs/`).
- Doc comments on public members use the existing house style: one terse summary sentence,
  `<param name="x">: description.</param>`.

## Git and verification

- Never `git push`. The owner pushes. Committing locally on `Dev` is fine; one commit per
  concern.
- The owner does all visual and interactive testing of Corona. After a build, launching
  `Corona.exe` for about three seconds to confirm it starts (and closing it) is the only
  automated UI check allowed. No screenshots.
- Always stop any Corona you started, and only that one: it opens the named pipe server that
  Booster connects to, and a lingering instance breaks the owner's instrument testing. Match it
  by process ID, since the owner's own instance has the same `Corona` process name.

## Gotchas

- Helios/Booster and Corona must run the same Nova version. Nova 1.1.0 changed the spectrum
  wire format (ScanDescription is serialized) relative to 1.1.0-dev.8 and earlier; mixed
  versions misparse every spectrum over the pipe. Corona pins 1.1.0.
- Nova.IO 1.1.0 throws `SpectrumFileOpenException` for an existing file it cannot open or
  index, and `FileReader(string)` throws for any file it cannot open; a missing file still
  reads as empty scans. `SimRunner.Run` catches everything from the replay and raises
  `RunError` (then `AcquisitionEnd` as usual); `CoronaViewModel.Load`/`FindScan` catch per
  file and log; `App` handles `DispatcherUnhandledException`. Keep those guards.
- `SimRunner.Run` is `async void`; nothing inside it reaches the caller. Tests must wait on
  `AcquisitionEnd`.
- Nova's `FileReader.ReadSpectrum*` take `""` as "the file already open"; never pass null.
