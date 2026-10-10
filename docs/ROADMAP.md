# Roadmap

The plan of record for the 2026-10 modernization, in the order the owner set. Update the
status lines as items land; the detail behind each decision is in the commit messages.

| # | Item | Status |
|---|------|--------|
| 1 | Permanent `Dev` branch; `main` only receives stable merges | Done locally 2026-10-09 (`wpf-ui` renamed; owner pushes `Dev`, deletes `origin/wpf-ui` and `origin/CleanUp`) |
| 2 | Professional repo layout: library + two apps, launchers, shared props, docs | Done 2026-10-09 |
| 3 | GitHub Actions: build, test, Dev pre-releases, manual official releases | Done locally 2026-10-09 (12 tests; workflows verified by a CI-equivalent local restore/build/test/pack); first real run after the owner pushes `Dev` |
| 4 | Finish the WPF visual punch list, then adopt LandmineUI.Wpf | Next; starts with the owner's list of visual nags |
| 5 | Upgrade to the official Nova release with file-open guards | Done 2026-10-09, pulled ahead of item 4 so CI can restore Nova (see below). Helios/Booster must run Nova 1.1.0 to talk to these builds. |

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
- **Item 4 LandmineUI**: latest official LandmineUI (1.2.0 at planning time) from the local
  feed; theme merged in `App.OnStartup` the way LandmineUI's Gallery does it; a light theme
  by default since Corona is light today; anything LandmineUI lacks becomes a wish file in
  `D:\Software\Claude\Wishes\LandmineUI\`, never an edit to LandmineUI.
- **Item 5 guards** (done): `SimRunner.Run` raises `RunError` and still fires
  `AcquisitionEnd`; `CoronaViewModel.Load` and `FindScan` catch per file and log; `App`
  handles `DispatcherUnhandledException` by logging and showing the message instead of
  exiting, which would also take the pipe server down.
