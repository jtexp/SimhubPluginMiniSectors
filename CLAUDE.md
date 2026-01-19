# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SimHub plugin that creates mini sectors data for sim racing. Tracks timing per corner/section of a track and exposes data properties for use in SimHub dashboards. Initially supports ACC (Assetto Corsa Competizione).

## Build Commands

**Prerequisites:** Set the `SIMHUB_INSTALL_PATH` environment variable to your SimHub installation directory (e.g., `C:\Program Files (x86)\SimHub\`).

```bash
# Build (uses MSBuild, targets .NET Framework 4.8, x86)
msbuild User.PluginSdkDemo.csproj /p:Configuration=Debug

# The post-build event automatically copies output to SimHub install directory
```

**Debug:** Set debug target to SimHub's main exe (`SimHubWPF.exe`).

## Architecture

### Core Plugin Class
`DataPluginMiniSectors.cs` - Main plugin implementing `IPlugin`, `IDataPlugin`, `IWPFSettingsV2`:
- `DataUpdate()` - Called each telemetry frame; updates turn/sector detection and timing
- `Init()` - Registers properties, actions, and events with SimHub
- `TrackTurnMap` - Static dictionary mapping track IDs to corner ranges (TrackPositionPercent boundaries)

### Key Concepts
- **RangeLabel struct** - Defines a track section with start/end TrackPositionPercent and label name
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

## Adding New Tracks

Add entries to the `TrackTurnMap` dictionary in `DataPluginMiniSectors.cs`. Each track needs:
1. A key matching the `TrackName` from game telemetry (case-insensitive)
2. An array of `RangeLabel` with TrackPositionPercent start/end values and corner names
