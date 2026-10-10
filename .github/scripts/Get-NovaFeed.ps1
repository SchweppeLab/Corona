# Makes Nova and Nova.IO restorable on a CI runner, where a developer machine's local feeds
# do not exist. Nova is not on nuget.org (the "Nova" package there is unrelated software).
#
# Order of preference:
#   1. nuget/ already holds Nova.<version>.nupkg (a temporarily vendored build): nothing to
#      do, the root nuget.config lists that folder. The Thermo packages must be there too.
#   2. Otherwise download the release bundle for exactly the version pinned in
#      Directory.Packages.props from https://github.com/SchweppeLab/Nova/releases:
#        official  x.y.z        tag v<x.y.z>, asset Nova-<x_y_z>-nupkg.zip
#        dev       x.y.z-dev.N  whichever release carries the asset Nova.<x.y.z-dev.N>.zip
#      A bundle holds Nova.*.nupkg, Nova.IO.*.nupkg, and ThermoRawFileReader/ with the exact
#      Thermo RawFileReader packages Nova.IO was built against, so no Thermo feed is needed
#      either. Both folders are registered as NuGet sources on this runner.
#
# Needs the gh CLI (preinstalled on GitHub runners) with GH_TOKEN set; Nova is public, so
# the workflow's own GITHUB_TOKEN is enough.

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$props = Get-Content (Join-Path $repoRoot 'Directory.Packages.props') -Raw
if ($props -notmatch '<PackageVersion Include="Nova" Version="([^"]+)"') {
  throw 'Could not find the Nova PackageVersion in Directory.Packages.props'
}
$version = $Matches[1]

$vendored = Join-Path $repoRoot "nuget\Nova.$version.nupkg"
if (Test-Path $vendored) {
  Write-Host "Nova $version is vendored under nuget/; using it."
  exit 0
}

$feed = Join-Path $repoRoot 'nova-feed'
New-Item -ItemType Directory -Force $feed | Out-Null

if ($version -match '-dev\.') {
  $asset = "Nova.$version.zip"
  $tag = gh api repos/SchweppeLab/Nova/releases --paginate --jq ".[] | select(any(.assets[]; .name == `"$asset`")) | .tag_name" |
    Select-Object -First 1
  if (-not $tag) {
    throw "No SchweppeLab/Nova release carries $asset. Vendor that build under nuget/ or pin a version Nova still publishes."
  }
} else {
  $tag = "v$version"
  $asset = "Nova-$($version -replace '\.', '_')-nupkg.zip"
}

Write-Host "Downloading $asset from SchweppeLab/Nova release $tag"
gh release download $tag --repo SchweppeLab/Nova --pattern $asset --dir $feed --clobber
if ($LASTEXITCODE -ne 0) { throw "gh release download failed for release $tag, asset $asset" }

Expand-Archive -Path (Join-Path $feed $asset) -DestinationPath $feed -Force
Get-ChildItem $feed -Recurse -Filter *.nupkg | ForEach-Object { Write-Host "  $($_.FullName.Substring($feed.Length + 1))" }

dotnet nuget add source $feed --name NovaRelease
dotnet nuget add source (Join-Path $feed 'ThermoRawFileReader') --name ThermoRawFileReaderPinned
