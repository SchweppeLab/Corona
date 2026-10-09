# Vendored NuGet packages

This folder is a local NuGet source, registered as `Corona-Vendored` in the root
`nuget.config`. It holds `.nupkg` files for dependencies that cannot be restored from a
public feed: today that means LandmineUI, whose repository is private.

Nova and Nova.IO are **not** vendored here. CI downloads them, together with the pinned
Thermo RawFileReader packages, from the matching release at
https://github.com/SchweppeLab/Nova/releases, using the version in
`Directory.Packages.props`.

To update a vendored package: drop the new `.nupkg` here, bump the version in
`Directory.Packages.props`, and delete the old `.nupkg` in the same commit.
