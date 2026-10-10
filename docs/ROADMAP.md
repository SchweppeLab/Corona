# Roadmap

The plan of record for the 2026-10 modernization, in the order the owner set. Update the
status lines as items land; the detail behind each decision is in the commit messages.

| # | Item | Status |
|---|------|--------|
| 1 | Permanent `Dev` branch; `main` only receives stable merges | Done locally 2026-10-09 (`wpf-ui` renamed; owner pushes `Dev`, deletes `origin/wpf-ui` and `origin/CleanUp`) |
| 2 | Professional repo layout: library + two apps, launchers, shared props, docs | Done 2026-10-09 |
| 3 | GitHub Actions: build, test, Dev pre-releases, manual official releases | Next |
| 4 | Finish the WPF visual punch list, then adopt LandmineUI.Wpf | Pending; starts with the owner's list of visual nags |
| 5 | Upgrade to the official Nova release with file-open guards | Pending; gated on Helios/Booster moving to the same Nova |

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
- **Known gap for item 3**: Nova's GitHub releases no longer carry the `1.1.0-dev.8` build
  Corona pins, so CI cannot restore it. Resolve before the first workflow run: either vendor
  dev.8 and Thermo 8.0.37 temporarily under `nuget/` or move item 5 (the Nova bump) ahead.
- **Item 4 LandmineUI**: latest official LandmineUI (1.2.0 at planning time) from the local
  feed; theme merged in `App.OnStartup` the way LandmineUI's Gallery does it; a light theme
  by default since Corona is light today; anything LandmineUI lacks becomes a wish file in
  `D:\Software\Claude\Wishes\LandmineUI\`, never an edit to LandmineUI.
- **Item 5 guards**: `SimRunner.Run` gets a `RunError` event and still fires `AcquisitionEnd`;
  `CoronaViewModel.Load` and `FindScan` catch per file; `App` gets a
  `DispatcherUnhandledException` handler that logs instead of crashing.
