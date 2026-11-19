# Roadmap

The plan of record for the 2026-10 modernization, in the order the owner set. Update the
status lines as items land; the detail behind each decision is in the commit messages.

| # | Item | Status |
|---|------|--------|
| 1 | Permanent `Dev` branch; `main` only receives stable merges | Done locally 2026-10-09 (`wpf-ui` renamed; owner pushes `Dev`, deletes `origin/wpf-ui` and `origin/CleanUp`) |
| 2 | Professional repo layout: library + two apps, launchers, shared props, docs | Done 2026-10-09 |
| 3 | GitHub Actions: build, test, Dev pre-releases, manual official releases | Done locally 2026-10-09 (12 tests; workflows verified by a CI-equivalent local restore/build/test/pack); first real run after the owner pushes `Dev` |
| 4 | Finish the WPF visual punch list, then adopt LandmineUI.Wpf | 2026-11-02: activity meter fixed (fills its column, 14 px tall); LandmineUI.Wpf adopted with a theme picker (12 themes, light/dark, persisted), custom-scan controls sized down from LandmineUI's 32 px button default. Second review round: toolbar icons are themed vector glyphs (Material Symbols geometries in `Icons.xaml`), the Message Log fills its tab, the spectrum plot has its m/z axis label, the empty status bar is back. 2026-11-03: LandmineUI 1.4.0, which honors `TabStripPlacement` (a Corona wish, granted the same day), so the lower tabs sit at the bottom again. 2026-11-04: the custom-scan count and scan-number box widen with their digits instead of clipping, and that row wraps in a narrow window. Further visual nags come from the owner's review. |
| 5 | Upgrade to the official Nova release with file-open guards | Done 2026-10-09, pulled ahead of item 4 so CI can restore Nova (see below). Helios/Booster must run Nova 1.1.0 to talk to these builds. |
| 6 | Instrument state for Helios: report the IAPI mode and state, act on a client's mode, start and cancel commands (added 2026-11-19, from the owner's protocol instructions) | Done locally 2026-11-19: `InstrumentStatus` and `AcquisitionControl`, pipe messages `6`/`M`/`S`/`C`, the status bar's mode selector, a replay duration limit and start delay; 45 tests, and the Helios in-process probe saw Corona's connect-time report (Standby), its answer to a mode command, and the disconnect sequence. A client-started replay through Helios and the status bar's look await the owner's review. |

## Decisions

- **Versioning**: one version in `Directory.Build.props` for MStreamer, Corona, and Blazar.
  Official tags `v1.1.0`; Dev builds `1.1.0-dev.N` under `dev-N-<sha>` tags plus a rolling
  `dev-latest`. Release assets stay `Corona_v1_1_0.zip` / `Blazar_v1_1_0.zip` for the
  download page, plus `MStreamer.1.1.0.nupkg`.
- **Packaging**: framework-dependent win-x64, the Scops launcher layout (root exe + `app\`).
  No self-contained publish, no installer.
- **CI packages**: Nova, Nova.IO, and the pinned Thermo packages come from the Nova GitHub
  release bundle matching `Directory.Packages.props`; LandmineUI (private repo) is vendored
  under `nuget/`.
- **Item 5 moved ahead of item 4** (owner's call, 2026-10-09): Nova's GitHub releases no
  longer carried the `1.1.0-dev.8` build Corona pinned, so CI could not have restored it.
  Corona now pins the `1.1.0` release, which CI downloads. Consequence: Dev builds speak the
  1.1.0 pipe wire format, so Helios/Booster need Nova 1.1.0 to receive scans from them.
- **Item 4 LandmineUI** (done): LandmineUI v1.2.0 (the build tagged in the LandmineUI repo)
  vendored under `nuget/`; theme merged in `App.OnStartup` the way LandmineUI's Gallery does
  it; Fjord Light by default since Corona was light before; a toolbar picker covers all five
  themes in light and dark and saves the choice; plots follow the theme through
  `ChartPalette`. Anything LandmineUI lacks becomes a wish file in
  `D:\Software\Claude\Wishes\LandmineUI\`, never an edit to LandmineUI.
- **Item 6 rules** (done): Corona replicates an instrument and never reports a state it is
  not in; a client's start is one acquisition under `DirectControl`, as Thermo's IAPI
  documentation describes every started acquisition, and is refused unless the instrument is
  On and idle; a mode other than On ends a replay first and is reported after the stream
  closes. What Corona cannot do (contact closure, methods) is logged, not faked. The contract
  is `docs/corona-pipe-protocol.md` in the Helios repo; CLAUDE.md has the Corona side.
- **Item 5 guards** (done): `SimRunner.Run` raises `RunError` and still fires
  `AcquisitionEnd`; `CoronaViewModel.Load` and `FindScan` catch per file and log; `App`
  handles `DispatcherUnhandledException` by logging and showing the message instead of
  exiting, which would also take the pipe server down.
