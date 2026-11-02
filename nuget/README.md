# Vendored NuGet packages

This folder is a local NuGet source, registered as `Corona-Vendored` in the root
`nuget.config`. It holds `.nupkg` files for dependencies that cannot be restored from a
public feed: LandmineUI (Core, Themes, Wpf), whose repository is private. The packages here
come from the LandmineUI feed on the owner's machine (`D:\Software\NuGet\LandmineUI`); the
version is the one pinned in `Directory.Packages.props`.

Nova and Nova.IO are **not** vendored here. CI downloads them, together with the pinned
Thermo RawFileReader packages, from the matching release at
https://github.com/SchweppeLab/Nova/releases, using the version in
`Directory.Packages.props`.

To update a vendored package: drop the new `.nupkg` files here, bump the versions in
`Directory.Packages.props`, and delete the old `.nupkg` files in the same commit.
