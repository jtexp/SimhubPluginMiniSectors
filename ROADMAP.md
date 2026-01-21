# MiniSectors Roadmap

Feature roadmap for the MiniSectors SimHub plugin. Focus: ACC (Assetto Corsa Competizione).

---

## Feature 1: Sector Delta Display

**Goal**: Show the time difference between current sector and session/all-time best.

**New Properties**:
- `CurrentSectorDeltaSession` - Delta vs session best (positive = slower)
- `CurrentSectorDeltaAllTime` - Delta vs all-time best
- `LastSectorDeltaSession` - Completed sector delta vs session best
- `LastSectorDeltaAllTime` - Completed sector delta vs all-time best

**Implementation**:
- Calculate delta as `current_time - best_time`
- Update in real-time during sector traversal
- Finalize on sector completion

**Use Case**: Dashboard can show green/red indicator if you're ahead/behind best pace.

---

## Feature 2: Theoretical Best Lap Time

**Goal**: Calculate what your lap time would be if you matched your best in every sector.

**New Properties**:
- `TheoreticalBestLapSession` - Sum of session best sectors
- `TheoreticalBestLapAllTime` - Sum of all-time best sectors
- `CurrentLapVsTheoreticalSession` - Current cumulative time vs theoretical
- `CurrentLapVsTheoreticalAllTime` - Current cumulative time vs all-time theoretical

**Implementation**:
- Sum all `_sessionBestSectorTimesSec` for theoretical session best
- Sum all-time bests from database for theoretical all-time
- Track cumulative lap time to compare against theoretical

**Use Case**: Know your true potential lap time based on your best individual sectors.

---

## Feature 3: Per-Car All-Time Best Filtering

**Status**: Partially implemented - infrastructure complete, filtering not yet applied.

**Goal**: Filter all-time best comparisons by car model so dashboard deltas are meaningful.

**Problem Solved**:
Comparing sector times against an all-time best set in a GT3 car while driving a GT4 car isn't useful - you'll always be slower. The all-time best should default to comparing against your best in the *same* car.

**Already Implemented**:
- `CurrentCarModel` property exposed to SimHub
- Database schema includes `car_model` column with proper indexing
- Car model extracted from telemetry and stored with sector bests

**Remaining Work**:
- Add plugin setting: "Compare all-time best against" with options:
  - "Same car only" (default)
  - "All cars"
- Modify `LoadAllTimeBestsForTrack()` to filter by car model when setting is enabled
- Reload all-time bests when car changes (if filtering by car)

**Use Case**: All-time best comparisons are meaningful regardless of which car you're driving.

---

## Feature 4: Last Reported Sector Times

**Goal**: Provide persistent sector times that don't reset at lap boundaries.

**Problem Solved**:
When a lap completes, current lap sector times are cleared and copied to "last lap". This means at the start of a new lap, `SectorTime_12` immediately shows 0 even though you just completed it. Dashboards lose visibility into the most recent sector performance.

**New Properties**:
- `LastReportedSectorTime_01` through `LastReportedSectorTime_15` - Most recent completed time per sector

**Behavior**:
| Event | SectorTime_XX | LastLapSectorTime_XX | LastReportedSectorTime_XX |
|-------|---------------|----------------------|---------------------------|
| Complete sector 3 | [3] = 12.5s | unchanged | [3] = 12.5s |
| Complete sector 4 | [4] = 8.2s | unchanged | [4] = 8.2s |
| Lap wraps | cleared to 0 | = old current | unchanged |
| Next lap sector 1 | [1] = 9.1s | unchanged | [1] = 9.1s |

**Key Difference from Other Arrays**:
- `SectorTime_XX` - Current lap only, cleared at lap wrap
- `LastLapSectorTime_XX` - Previous lap only, bulk-overwritten at lap wrap
- `LastReportedSectorTime_XX` - Most recent completion, individual values persist until that sector is completed again

**Implementation**:
- Add `_lastReportedSectorTimesSec` array in `SectorTimingEngine.cs`
- Initialize to -1.0 (unset indicator, matches session best pattern)
- Update on sector completion (both mid-lap transitions and final sector at lap wrap)
- NOT cleared or bulk-overwritten at lap wrap
- Expose via `GetLastReportedSectorTime(int sector)` and SimHub properties

**Use Case**: Dashboard always shows your most recent time for each sector, even at the start of a new lap.

---

## Feature 5: Bundled Overlay Dashboard

**Goal**: Provide a ready-to-use SimHub overlay that displays sector times while racing.

**Current State**: Initial version exists at `DashTemplates/John - Mini Sectors/` with JavaScript extensions for formatting sector times.

**Planned Features**:
- Bundle overlay dashboard with the plugin repository
- Add "Install Overlay" button in plugin settings to copy dashboard to SimHub
- Display current sector, last completed sector time, and delta vs best
- Show session best and all-time best comparison
- Compact overlay suitable for racing without obscuring view

**Implementation**:
- Store dashboard template in repo under `Dashboard/` folder
- Add button in SettingsControl.xaml to install/update the dashboard
- Copy dashboard files to `SimHub\DashTemplates\MiniSectors\` on install
- Detect if dashboard already exists and offer to update

**Use Case**: Users get a working overlay out of the box without needing to build their own dashboard.

---

## Feature 6: Custom Sector Definition via Hotkey

**Goal**: Allow users to define their own sector boundaries by pressing a hotkey while driving.

**How It Works**:
- User starts a "recording" mode from plugin settings or via hotkey
- While driving, press the hotkey at each point where a sector should end
- Plugin captures current `TrackPositionPercent` as sector boundary
- Boundaries saved to database for that track
- Custom sectors override the default turn-based sectors

**Implementation**:
- Add `custom_sectors` table to SQLite: `(track_id, sector_number, end_position)`
- Add SimHub action `StartSectorRecording` / `EndSectorRecording`
- Add SimHub action `MarkSectorBoundary` - captures current position
- Plugin setting to choose: "Use default sectors" / "Use custom sectors"
- UI in settings to view/edit/delete custom sector definitions

**Use Case**: Similar to iRacing's sector customization - drivers can define sectors that match their analysis needs (e.g., sector per straight, per complex, etc.).

---

## Feature 7: Even Split Sectors

**Goal**: Automatically divide the track into N equal sectors based on track position.

**How It Works**:
- User specifies number of sectors (e.g., 10)
- Plugin divides track evenly: sector 1 = 0-10%, sector 2 = 10-20%, etc.
- No need for per-track corner definitions
- Works on any track immediately

**Implementation**:
- Add plugin setting: "Sector mode" with options:
  - "Turn-based" (default) - use `TrackTurnMap` definitions
  - "Even split" - divide by count
  - "Custom" - use user-defined boundaries
- Add setting: "Number of sectors" (default: 10, range: 3-30)
- Modify `SectorTimingEngine` to calculate sector from position when in even-split mode
- Sector boundaries: `position * sectorCount` rounded down + 1

**Use Case**: Quick setup for tracks without corner definitions, or for users who prefer consistent sector counts across all tracks.

---

## Implementation Priority

1. **Delta Display** - Leverages existing best times, high dashboard value
2. **Theoretical Best** - Aggregation of existing data
3. **Per-Car All-Time Best Filtering** - Add setting to filter all-time bests by car (default) or all cars
4. **Last Reported Sector Times** - Simple array addition, high dashboard value
5. **Bundled Overlay** - Complete user experience with ready-to-use dashboard
6. **Even Split Sectors** - Simple calculation, works on any track
7. **Custom Sector Definition** - More complex, requires recording UI and hotkey handling

---

## Completed Features

- **Session Best Sector Times** - `SessionBestSectorTime_01` through `_60` properties
- **All-Time Best Sector Times (SQLite)** - `AllTimeBestSectorTime_01` through `_60` with database persistence
- **Condition tracking** - Track temp, air temp, weather, and grip level stored with each sector best
- **Auto-update** - Check for and install nightly updates from plugin settings
- **Records tab** - View historical sector bests in a sortable DataGrid

---

## Future Ideas

- **Stint tracking** - Best times per stint (resets on pit stop); useful for endurance racing to compare tire degradation
- **Export/import personal bests** - Share or backup your data
- **Sector performance trends** - Visualize improvement over time
- **ACC native sector integration** - Compare mini sectors against ACC's built-in sector data

---

## SDLC / Project Management

This roadmap is currently maintained as a markdown file. Consider migrating to GitHub Issues and Projects for better tracking:

**Benefits of GitHub Projects:**
- Issues link to PRs and auto-close on merge
- Kanban board view for visual progress tracking
- Discussion threads on each feature
- Milestones and due dates
- Better visibility for contributors

**Migration path:**
1. Create a GitHub Issue for each feature above
2. Create a GitHub Project board with columns: Backlog, In Progress, Done
3. Add issues to the project
4. Archive this ROADMAP.md or keep it as a high-level summary
