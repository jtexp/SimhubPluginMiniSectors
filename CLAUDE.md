# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SimHub plugin that creates mini sectors data for sim racing. Tracks timing per corner/section of a track and exposes data properties for use in SimHub dashboards. Initially supports ACC (Assetto Corsa Competizione).

## Build Commands

**Prerequisites:** Set the `SIMHUB_INSTALL_PATH` Windows user environment variable to your SimHub installation directory (e.g., `C:\Program Files (x86)\SimHub\`).

```bash
# Build from WSL (recommended)
./build.sh

# Build from WSL with Release config
./build.sh Release

# Build from Windows command line
msbuild User.PluginSdkDemo.csproj /p:Configuration=Debug
```

**Note:** Close SimHub before building - the post-build copy will fail if SimHub has the DLLs locked.

**WSL Build Details:** `build.sh` calls Windows MSBuild via `powershell.exe` since this is a .NET Framework 4.8 project that requires Windows tooling. The script reads `SIMHUB_INSTALL_PATH` from Windows user environment variables.

**Debug:** Set debug target to SimHub's main exe (`SimHubWPF.exe`).

## Architecture

### Core Plugin Class
`DataPluginMiniSectors.cs` - Main plugin implementing `IPlugin`, `IDataPlugin`, `IWPFSettingsV2`:
- `DataUpdate()` - Called each telemetry frame; updates turn/sector detection and timing
- `Init()` - Registers properties, actions, and events with SimHub

### Track Data
`TrackData.cs` - Static track corner definitions:
- `RangeLabel` struct - Defines a track section with start/end TrackPositionPercent and label name
- `TrackTurnMap` - Static dictionary mapping track IDs to corner ranges

### Key Concepts
- **Sector timing** - Uses corner end boundaries to define sector splits; accumulates time between boundaries
- **Lap wrap detection** - `IsLapWrap()` detects when TrackPositionPercent wraps from ~1.0 to ~0.0

### Exposed SimHub Properties
- `CurrentTurn` - Current corner/section name
- `CurrentSectorNumber`, `SectorCountForTrack` - Sector index and total count
- `CurrentSectorTime`, `LastCompletedSectorTime` - Timing values in seconds
- `SectorTime_01` through `SectorTime_60` - Per-sector times for current lap
- `LastLapSectorTime_01` through `LastLapSectorTime_60` - Per-sector times for previous lap

### Settings & UI
- `DataPluginDemoSettings.cs` - Serializable settings (JSON.NET compatible)
- `SettingsControlDemo.xaml/.cs` - WPF settings control displayed in SimHub
- `DemoWindow.xaml/.cs`, `DemoDialogWindow.xaml/.cs` - Example windows/dialogs

### Data Storage
SQLite database stored at `%USERPROFILE%\Documents\SimHub\PluginsData\User.PluginMiniSectors\MiniSectors.sqlite`.

## Dependencies

References loaded from `$(SIMHUB_INSTALL_PATH)`:
- `SimHub.Plugins.dll`, `GameReaderCommon.dll` - Core SimHub plugin SDK
- `MahApps.Metro.dll` - WPF styling framework
- `log4net.dll` - Logging via `SimHub.Logging.Current`

NuGet: `System.Data.SQLite` (Stub.System.Data.SQLite.Core.NetFramework 1.0.119.0)

## Testing

Test project: `User.PluginMiniSectors.Tests/` (MSTest, uses PackageReference)

```bash
# Build and run tests from WSL
powershell.exe -Command "& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' User.PluginMiniSectors.sln /p:Configuration=Release /p:PostBuildEvent= /restore"

powershell.exe -Command "& 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe' 'User.PluginMiniSectors.Tests\bin\Release\User.PluginMiniSectors.Tests.dll'"
```

Tests cover `RangeLabel` and `TrackTurnMap` functionality. Internal types are exposed via `InternalsVisibleTo` in `Properties/AssemblyInfo.cs`.

## CI/CD

GitHub Actions workflow: `.github/workflows/build.yml`

**SimHub SDK for CI:** The workflow downloads SimHub SDK DLLs from a separate repo since SimHub isn't installed on GitHub runners:
- Repo: `https://github.com/jtexp/simhub-sdk`
- Contains only the DLLs needed for compilation (not the full SimHub install)
- Update this repo when upgrading SimHub versions

## Adding New Tracks

Add entries to the `TrackTurnMap` dictionary in `TrackData.cs`. Each track needs:
1. A key matching the `TrackName` from game telemetry (case-insensitive)
2. An array of `RangeLabel` with TrackPositionPercent start/end values and corner names

## GitHub Actions

To check CI build status, use a polling loop instead of a single sleep:

```bash
# Poll every 5 seconds until build completes
for i in {1..60}; do
  status=$(gh run list --limit 1 --json status --jq '.[0].status')
  echo "Build status: $status"
  if [ "$status" = "completed" ]; then
    gh run list --limit 1
    break
  fi
  sleep 5
done
```

To view failed build logs:
```bash
gh run view <run-id> --log-failed
```
