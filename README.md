# MiniSectors - SimHub Plugin

A SimHub plugin that tracks per-corner timing for sim racing. Know exactly how you're performing through each section of the track.

## Features

- Per-corner timing for 24+ ACC tracks
- Current lap and last lap sector times
- Session best and all-time best tracking (stored in SQLite)
- Properties exposed for use in SimHub dashboards

## Installation

1. Download `User.PluginMiniSectors.zip` from the [latest nightly release](https://github.com/jtexp/SimhubPluginMiniSectors/releases/tag/nightly)
2. Extract the contents to your SimHub installation folder (e.g., `C:\Program Files (x86)\SimHub\`)
3. Restart SimHub
4. Enable "Minisectors" in SimHub's Plugins settings

## Usage

Once installed, access sector timing properties in your dashboards with the `[Minisectors]` prefix:

- `[Minisectors].CurrentTurn` - Current corner name
- `[Minisectors].CurrentSectorTime` - Time in current sector
- `[Minisectors].SectorTime_01` through `SectorTime_60` - Per-sector times

View your stored records in the plugin settings under the **Records** tab.

## Supported Games

- Assetto Corsa Competizione (ACC)

## Contributing

See [CLAUDE.md](CLAUDE.md) for development setup and architecture details.
