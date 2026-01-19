# MiniSectors - SimHub Plugin

A SimHub plugin that creates mini sectors (per-corner timing) for sim racing. Tracks your time through each corner/section of a track and exposes data properties for use in SimHub dashboards.

## Supported Simulators

- **Assetto Corsa Competizione (ACC)**

## Features

- **Per-corner tracking**: Identifies which corner/section you're in based on track position
- **Sector timing**: Tracks time spent in each sector (defined by corner boundaries)
- **Current lap times**: Per-sector times for the lap in progress
- **Last lap times**: Per-sector times for your previous completed lap
- **24 tracks mapped**: Comprehensive corner definitions for popular ACC tracks

## Supported Tracks

| Track | Corners |
|-------|---------|
| Barcelona | 11 |
| Brands Hatch | 16 |
| Circuit of the Americas (COTA) | 20 |
| Donington Park | 12 |
| Hungaroring | 16 |
| Imola | 7 |
| Kyalami | 12 |
| Laguna Seca | 10 |
| Misano | 7 |
| Monza | 8 |
| Mount Panorama (Bathurst) | 16 |
| Nurburgring GP | 11 |
| Nurburgring 24h (Nordschleife) | 43 |
| Oulton Park | 15 |
| Paul Ricard | 11 |
| Silverstone | 17 |
| Snetterton | 12 |
| Spa-Francorchamps | 15 |
| Suzuka | 13 |
| Watkins Glen | 9 |
| Zandvoort | 12 |
| Zolder | 12 |

## Exposed SimHub Properties

Use these properties in your SimHub dashboards with the prefix `[Minisectors]`:

### Track & Position
| Property | Type | Description |
|----------|------|-------------|
| `TrackId` | string | Current track identifier |
| `TrackPositionPercent` | double | Position on track (0.0 - 1.0) |
| `CurrentTurn` | string | Name of current corner/section |
| `CurrentSectorNumber` | int | Current sector index (1-based) |
| `SectorCountForTrack` | int | Total number of sectors for track |

### Timing
| Property | Type | Description |
|----------|------|-------------|
| `CurrentSectorTime` | double | Elapsed time in current sector (seconds) |
| `LastCompletedSectorTime` | double | Time of last completed sector (seconds) |
| `SectorTime_01` - `SectorTime_60` | double | Per-sector times for current lap |
| `LastLapSectorTime_01` - `LastLapSectorTime_60` | double | Per-sector times for previous lap |

## Installation

1. Build the project (requires SimHub SDK references)
2. Copy the DLL to your SimHub plugins folder
3. Enable the plugin in SimHub settings

## Building

Set the `SIMHUB_INSTALL_PATH` environment variable to your SimHub installation directory, then:

```bash
msbuild User.PluginSdkDemo.csproj /p:Configuration=Debug
```

The post-build event automatically copies the output to SimHub.

## Adding New Tracks

Edit `DataPluginMiniSectors.cs` and add entries to the `TrackTurnMap` dictionary:

```csharp
["track_name"] = new[]
{
    new RangeLabel(0.050, 0.100, "Turn 1"),
    new RangeLabel(0.150, 0.200, "Turn 2"),
    // ... more corners
},
```

- `track_name`: Must match `TrackName` from game telemetry (case-insensitive)
- Each `RangeLabel`: start position, end position, corner name
- Positions are `TrackPositionPercent` values (0.0 - 1.0)

## Roadmap

See [ROADMAP.md](ROADMAP.md) for planned features.

## License

MIT
