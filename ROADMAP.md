# MiniSectors Roadmap

Feature roadmap for the MiniSectors SimHub plugin. Focus: ACC (Assetto Corsa Competizione).

---

## Feature 1: Session Best Sector Times

**Goal**: Track the fastest time achieved in each sector during the current session.

**Terminology Note**:
- **Session** = A period of track activity (Practice, Qualifying, Race). Resets when you exit to menu or restart.
- **Stint** = Continuous driving between pit stops. Multiple stints can occur in one session.

We use "Session" here as it matches ACC's terminology and is more intuitive for general use.

**New Properties**:
- `SessionBestSectorTime_01` through `SessionBestSectorTime_60` - Best time per sector this session

**Implementation**:
- Add `_sessionBestSectorTimesSec` array alongside existing lap arrays
- On sector completion, compare against session best and update if faster
- Reset session bests when track changes or game restarts

**Use Case**: See how your current sector compares to your best effort this session.

---

## Feature 2: All-Time Best Sector Times (SQLite Persistence)

**Goal**: Persist best sector times to SQLite database, surviving across sessions.

**Database Schema**:
```sql
CREATE TABLE sector_bests (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    track_id TEXT NOT NULL,
    sector_number INTEGER NOT NULL,
    best_time_sec REAL NOT NULL,
    car_model TEXT,
    recorded_at TEXT NOT NULL,
    UNIQUE(track_id, sector_number, car_model)
);
```

**New Properties**:
- `AllTimeBestSectorTime_01` through `AllTimeBestSectorTime_60` - All-time best per sector

**Implementation**:
- Load all-time bests from SQLite on track load
- Update database when a new all-time best is set
- Store per track + car combination for accuracy

---

## Feature 3: Sector Delta Display

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

## Feature 4: Theoretical Best Lap Time

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

## Feature 5: Per-Car Best Times

**Goal**: Track separate best times for each car model driven.

**Database Schema Extension**:
```sql
-- sector_bests table already includes car_model column
-- Add index for fast lookups
CREATE INDEX idx_sector_bests_car ON sector_bests(track_id, car_model);
```

**New Properties**:
- `CurrentCarModel` - The car currently being driven
- `CarBestSectorTime_01` through `CarBestSectorTime_60` - Best for current car

**Implementation**:
- Extract car model from ACC telemetry (`data.NewData.CarModel` or similar)
- Filter SQLite queries by car model
- Expose both global and car-specific bests

**Use Case**: Compare performance across different cars on the same track.

---

## Implementation Priority

1. **Session Best** - Foundation, no database changes needed
2. **All-Time Best** - Builds on session best, adds persistence
3. **Delta Display** - Leverages both best types, high dashboard value
4. **Theoretical Best** - Aggregation of existing data
5. **Per-Car Bests** - Extension of existing schema

---

## Future Ideas (Beyond Initial 5)

- **Stint tracking** - Best times per stint (resets on pit stop); useful for endurance racing to compare tire degradation
- **Condition tracking** - Track temp, weather conditions stored with best times
- **Export/import personal bests** - Share or backup your data
- **Sector performance trends** - Visualize improvement over time
- **ACC native sector integration** - Compare mini sectors against ACC's built-in sector data
