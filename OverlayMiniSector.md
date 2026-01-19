# MiniSectors Overlay Dashboard Guide

A guide for creating a SimHub overlay dashboard to display sector timing data.

## Dashboard Goal

Display a row for each sector showing:
- Sector label (S1, S2, S3...)
- Last completed lap time for that sector
- Delta to session best
- Delta to all-time best

---

## Available Plugin Properties

All properties are prefixed with `MiniSectors.`

| Property | Type | Description |
|----------|------|-------------|
| `SectorCountForTrack` | int | Number of sectors on current track |
| `CurrentSectorNumber` | int | Sector currently being driven |
| `LastCompletedSectorNumber` | int | Most recently completed sector (1-based) |
| `LastCompletedSectorTime` | double | Time of the last completed sector |
| `SectorTime_01` through `_60` | double | Current lap sector times (live) |
| `LastLapSectorTime_01` through `_60` | double | Sector times from last completed lap |
| `SessionBestSectorTime_01` through `_60` | double | Session best per sector |
| `AllTimeBestSectorTime_01` through `_60` | double | All-time best per sector |
| `CurrentTurn` | string | Current corner/section name |
| `TrackId` | string | Current track identifier |

**Important:** All time values return `-1.0` when there is no data.

---

## JavaScript Snippets

### Row Visibility (IsVisible binding)

Use this to show/hide rows based on how many sectors the track has.

**Row 1:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 1;
```

**Row 2:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 2;
```

**Row 3:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 3;
```

**Row 4:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 4;
```

**Row 5:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 5;
```

**Row 6:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 6;
```

**Row 7:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 7;
```

**Row 8:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 8;
```

**Row 9:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 9;
```

**Row 10:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 10;
```

**Row 11:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 11;
```

**Row 12:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 12;
```

**Row 13:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 13;
```

**Row 14:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 14;
```

**Row 15:**
```javascript
return $prop('MiniSectors.SectorCountForTrack') >= 15;
```

---

### Sector Labels (Text binding)

Static text: just use `S1`, `S2`, `S3`, etc. No JavaScript needed.

---

## Calculating Most Recent Completed Sector Time in JavaScript

Instead of adding extra plugin properties, you can calculate the "most recent completed time" for each sector directly in the dashboard using JavaScript. This approach:

- Shows current lap time if the sector was completed this lap
- Falls back to last lap time if the sector hasn't been completed yet this lap
- Keeps the plugin simple while giving you flexible display options

### How It Works

The logic compares the sector number to `LastCompletedSectorNumber`:

```
If sector N <= LastCompletedSectorNumber:
    → Use SectorTime_N (completed this lap)
Else:
    → Use LastLapSectorTime_N (from previous lap)
```

**Example:** You're currently in sector 5 (so sectors 1-4 are complete this lap)
- S1-S4: Show `SectorTime_01` through `_04` (current lap)
- S5-S15: Show `LastLapSectorTime_05` through `_15` (previous lap)

---

### Most Recent Sector Time (Text binding)

Use these for the time column in your overlay. Shows the most recently completed time for each sector.

**Sector 1:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 1) {
    time = $prop('MiniSectors.SectorTime_01');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_01');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 2:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 2) {
    time = $prop('MiniSectors.SectorTime_02');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_02');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 3:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 3) {
    time = $prop('MiniSectors.SectorTime_03');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_03');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 4:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 4) {
    time = $prop('MiniSectors.SectorTime_04');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_04');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 5:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 5) {
    time = $prop('MiniSectors.SectorTime_05');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_05');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 6:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 6) {
    time = $prop('MiniSectors.SectorTime_06');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_06');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 7:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 7) {
    time = $prop('MiniSectors.SectorTime_07');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_07');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 8:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 8) {
    time = $prop('MiniSectors.SectorTime_08');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_08');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 9:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 9) {
    time = $prop('MiniSectors.SectorTime_09');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_09');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 10:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 10) {
    time = $prop('MiniSectors.SectorTime_10');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_10');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 11:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 11) {
    time = $prop('MiniSectors.SectorTime_11');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_11');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 12:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 12) {
    time = $prop('MiniSectors.SectorTime_12');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_12');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 13:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 13) {
    time = $prop('MiniSectors.SectorTime_13');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_13');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 14:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 14) {
    time = $prop('MiniSectors.SectorTime_14');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_14');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 15:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time;
if (lastCompleted >= 15) {
    time = $prop('MiniSectors.SectorTime_15');
} else {
    time = $prop('MiniSectors.LastLapSectorTime_15');
}
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

---

### Delta to Session Best - Using Most Recent Time (Text binding)

These calculate delta using the most recent completed time for each sector.

**Sector 1:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 1)
    ? $prop('MiniSectors.SectorTime_01')
    : $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.SessionBestSectorTime_01');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 2:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 2)
    ? $prop('MiniSectors.SectorTime_02')
    : $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.SessionBestSectorTime_02');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 3:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 3)
    ? $prop('MiniSectors.SectorTime_03')
    : $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.SessionBestSectorTime_03');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 4:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 4)
    ? $prop('MiniSectors.SectorTime_04')
    : $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.SessionBestSectorTime_04');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 5:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 5)
    ? $prop('MiniSectors.SectorTime_05')
    : $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.SessionBestSectorTime_05');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 6:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 6)
    ? $prop('MiniSectors.SectorTime_06')
    : $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.SessionBestSectorTime_06');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 7:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 7)
    ? $prop('MiniSectors.SectorTime_07')
    : $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.SessionBestSectorTime_07');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 8:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 8)
    ? $prop('MiniSectors.SectorTime_08')
    : $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.SessionBestSectorTime_08');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 9:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 9)
    ? $prop('MiniSectors.SectorTime_09')
    : $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.SessionBestSectorTime_09');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 10:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 10)
    ? $prop('MiniSectors.SectorTime_10')
    : $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.SessionBestSectorTime_10');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 11:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 11)
    ? $prop('MiniSectors.SectorTime_11')
    : $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.SessionBestSectorTime_11');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 12:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 12)
    ? $prop('MiniSectors.SectorTime_12')
    : $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.SessionBestSectorTime_12');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 13:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 13)
    ? $prop('MiniSectors.SectorTime_13')
    : $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.SessionBestSectorTime_13');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 14:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 14)
    ? $prop('MiniSectors.SectorTime_14')
    : $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.SessionBestSectorTime_14');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 15:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 15)
    ? $prop('MiniSectors.SectorTime_15')
    : $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.SessionBestSectorTime_15');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

---

### Delta to All-Time Best - Using Most Recent Time (Text binding)

**Sector 1:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 1)
    ? $prop('MiniSectors.SectorTime_01')
    : $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.AllTimeBestSectorTime_01');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 2:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 2)
    ? $prop('MiniSectors.SectorTime_02')
    : $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.AllTimeBestSectorTime_02');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 3:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 3)
    ? $prop('MiniSectors.SectorTime_03')
    : $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.AllTimeBestSectorTime_03');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 4:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 4)
    ? $prop('MiniSectors.SectorTime_04')
    : $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.AllTimeBestSectorTime_04');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 5:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 5)
    ? $prop('MiniSectors.SectorTime_05')
    : $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.AllTimeBestSectorTime_05');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 6:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 6)
    ? $prop('MiniSectors.SectorTime_06')
    : $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.AllTimeBestSectorTime_06');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 7:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 7)
    ? $prop('MiniSectors.SectorTime_07')
    : $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.AllTimeBestSectorTime_07');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 8:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 8)
    ? $prop('MiniSectors.SectorTime_08')
    : $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.AllTimeBestSectorTime_08');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 9:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 9)
    ? $prop('MiniSectors.SectorTime_09')
    : $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.AllTimeBestSectorTime_09');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 10:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 10)
    ? $prop('MiniSectors.SectorTime_10')
    : $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.AllTimeBestSectorTime_10');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 11:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 11)
    ? $prop('MiniSectors.SectorTime_11')
    : $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.AllTimeBestSectorTime_11');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 12:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 12)
    ? $prop('MiniSectors.SectorTime_12')
    : $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.AllTimeBestSectorTime_12');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 13:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 13)
    ? $prop('MiniSectors.SectorTime_13')
    : $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.AllTimeBestSectorTime_13');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 14:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 14)
    ? $prop('MiniSectors.SectorTime_14')
    : $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.AllTimeBestSectorTime_14');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 15:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 15)
    ? $prop('MiniSectors.SectorTime_15')
    : $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.AllTimeBestSectorTime_15');
if (time == null || best == null || time < 0 || best < 0) return '';
var delta = time - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

---

### Delta Color - Session Best Using Most Recent Time (Color binding)

**Sector 1:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 1)
    ? $prop('MiniSectors.SectorTime_01')
    : $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.SessionBestSectorTime_01');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 2:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 2)
    ? $prop('MiniSectors.SectorTime_02')
    : $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.SessionBestSectorTime_02');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 3:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 3)
    ? $prop('MiniSectors.SectorTime_03')
    : $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.SessionBestSectorTime_03');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 4:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 4)
    ? $prop('MiniSectors.SectorTime_04')
    : $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.SessionBestSectorTime_04');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 5:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 5)
    ? $prop('MiniSectors.SectorTime_05')
    : $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.SessionBestSectorTime_05');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 6:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 6)
    ? $prop('MiniSectors.SectorTime_06')
    : $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.SessionBestSectorTime_06');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 7:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 7)
    ? $prop('MiniSectors.SectorTime_07')
    : $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.SessionBestSectorTime_07');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 8:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 8)
    ? $prop('MiniSectors.SectorTime_08')
    : $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.SessionBestSectorTime_08');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 9:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 9)
    ? $prop('MiniSectors.SectorTime_09')
    : $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.SessionBestSectorTime_09');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 10:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 10)
    ? $prop('MiniSectors.SectorTime_10')
    : $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.SessionBestSectorTime_10');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 11:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 11)
    ? $prop('MiniSectors.SectorTime_11')
    : $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.SessionBestSectorTime_11');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 12:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 12)
    ? $prop('MiniSectors.SectorTime_12')
    : $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.SessionBestSectorTime_12');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 13:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 13)
    ? $prop('MiniSectors.SectorTime_13')
    : $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.SessionBestSectorTime_13');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 14:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 14)
    ? $prop('MiniSectors.SectorTime_14')
    : $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.SessionBestSectorTime_14');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 15:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 15)
    ? $prop('MiniSectors.SectorTime_15')
    : $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.SessionBestSectorTime_15');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

---

### Delta Color - All-Time Best Using Most Recent Time (Color binding)

**Sector 1:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 1)
    ? $prop('MiniSectors.SectorTime_01')
    : $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.AllTimeBestSectorTime_01');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 2:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 2)
    ? $prop('MiniSectors.SectorTime_02')
    : $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.AllTimeBestSectorTime_02');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 3:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 3)
    ? $prop('MiniSectors.SectorTime_03')
    : $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.AllTimeBestSectorTime_03');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 4:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 4)
    ? $prop('MiniSectors.SectorTime_04')
    : $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.AllTimeBestSectorTime_04');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 5:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 5)
    ? $prop('MiniSectors.SectorTime_05')
    : $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.AllTimeBestSectorTime_05');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 6:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 6)
    ? $prop('MiniSectors.SectorTime_06')
    : $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.AllTimeBestSectorTime_06');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 7:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 7)
    ? $prop('MiniSectors.SectorTime_07')
    : $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.AllTimeBestSectorTime_07');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 8:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 8)
    ? $prop('MiniSectors.SectorTime_08')
    : $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.AllTimeBestSectorTime_08');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 9:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 9)
    ? $prop('MiniSectors.SectorTime_09')
    : $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.AllTimeBestSectorTime_09');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 10:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 10)
    ? $prop('MiniSectors.SectorTime_10')
    : $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.AllTimeBestSectorTime_10');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 11:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 11)
    ? $prop('MiniSectors.SectorTime_11')
    : $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.AllTimeBestSectorTime_11');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 12:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 12)
    ? $prop('MiniSectors.SectorTime_12')
    : $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.AllTimeBestSectorTime_12');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 13:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 13)
    ? $prop('MiniSectors.SectorTime_13')
    : $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.AllTimeBestSectorTime_13');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 14:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 14)
    ? $prop('MiniSectors.SectorTime_14')
    : $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.AllTimeBestSectorTime_14');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 15:**
```javascript
var lastCompleted = $prop('MiniSectors.LastCompletedSectorNumber');
var time = (lastCompleted >= 15)
    ? $prop('MiniSectors.SectorTime_15')
    : $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.AllTimeBestSectorTime_15');
if (time == null || best == null || time < 0 || best < 0) return '#FFFFFF';
var delta = time - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

---

## Alternative: Last Lap Only Snippets

If you prefer to always show the previous completed lap's times (simpler, no mixing of current/last lap data), use these snippets instead.

### Last Lap Sector Time (Text binding)

**Sector 1:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_01');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 2:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_02');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 3:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_03');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 4:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_04');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 5:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_05');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 6:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_06');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 7:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_07');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 8:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_08');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 9:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_09');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 10:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_10');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 11:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_11');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 12:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_12');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 13:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_13');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 14:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_14');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

**Sector 15:**
```javascript
var time = $prop('MiniSectors.LastLapSectorTime_15');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

---

### Delta to Session Best (Text binding)

**Sector 1:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.SessionBestSectorTime_01');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 2:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.SessionBestSectorTime_02');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 3:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.SessionBestSectorTime_03');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 4:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.SessionBestSectorTime_04');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 5:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.SessionBestSectorTime_05');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 6:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.SessionBestSectorTime_06');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 7:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.SessionBestSectorTime_07');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 8:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.SessionBestSectorTime_08');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 9:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.SessionBestSectorTime_09');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 10:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.SessionBestSectorTime_10');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 11:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.SessionBestSectorTime_11');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 12:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.SessionBestSectorTime_12');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 13:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.SessionBestSectorTime_13');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 14:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.SessionBestSectorTime_14');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 15:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.SessionBestSectorTime_15');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

---

### Delta to All-Time Best (Text binding)

**Sector 1:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.AllTimeBestSectorTime_01');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 2:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.AllTimeBestSectorTime_02');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 3:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.AllTimeBestSectorTime_03');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 4:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.AllTimeBestSectorTime_04');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 5:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.AllTimeBestSectorTime_05');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 6:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.AllTimeBestSectorTime_06');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 7:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.AllTimeBestSectorTime_07');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 8:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.AllTimeBestSectorTime_08');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 9:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.AllTimeBestSectorTime_09');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 10:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.AllTimeBestSectorTime_10');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 11:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.AllTimeBestSectorTime_11');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 12:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.AllTimeBestSectorTime_12');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 13:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.AllTimeBestSectorTime_13');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 14:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.AllTimeBestSectorTime_14');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

**Sector 15:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.AllTimeBestSectorTime_15');
if (last == null || best == null || last < 0 || best < 0) return '';
var delta = last - best;
var sign = delta >= 0 ? '+' : '';
return sign + delta.toFixed(3);
```

---

### Delta Color - Session Best (Background/Foreground Color binding)

**Sector 1:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.SessionBestSectorTime_01');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';  // Green - faster
if (delta > 0.001) return '#FF0000';   // Red - slower
return '#FFFF00';  // Yellow - equal (personal best)
```

**Sector 2:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.SessionBestSectorTime_02');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 3:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.SessionBestSectorTime_03');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 4:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.SessionBestSectorTime_04');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 5:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.SessionBestSectorTime_05');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 6:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.SessionBestSectorTime_06');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 7:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.SessionBestSectorTime_07');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 8:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.SessionBestSectorTime_08');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 9:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.SessionBestSectorTime_09');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 10:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.SessionBestSectorTime_10');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 11:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.SessionBestSectorTime_11');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 12:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.SessionBestSectorTime_12');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 13:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.SessionBestSectorTime_13');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 14:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.SessionBestSectorTime_14');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

**Sector 15:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.SessionBestSectorTime_15');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#00FF00';
if (delta > 0.001) return '#FF0000';
return '#FFFF00';
```

---

### Delta Color - All-Time Best (Background/Foreground Color binding)

**Sector 1:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_01');
var best = $prop('MiniSectors.AllTimeBestSectorTime_01');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';  // Purple - all-time best beaten
if (delta > 0.001) return '#FF0000';   // Red - slower
return '#AA00FF';  // Purple - matched all-time best
```

**Sector 2:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_02');
var best = $prop('MiniSectors.AllTimeBestSectorTime_02');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 3:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_03');
var best = $prop('MiniSectors.AllTimeBestSectorTime_03');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 4:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_04');
var best = $prop('MiniSectors.AllTimeBestSectorTime_04');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 5:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_05');
var best = $prop('MiniSectors.AllTimeBestSectorTime_05');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 6:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_06');
var best = $prop('MiniSectors.AllTimeBestSectorTime_06');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 7:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_07');
var best = $prop('MiniSectors.AllTimeBestSectorTime_07');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 8:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_08');
var best = $prop('MiniSectors.AllTimeBestSectorTime_08');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 9:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_09');
var best = $prop('MiniSectors.AllTimeBestSectorTime_09');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 10:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_10');
var best = $prop('MiniSectors.AllTimeBestSectorTime_10');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 11:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_11');
var best = $prop('MiniSectors.AllTimeBestSectorTime_11');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 12:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_12');
var best = $prop('MiniSectors.AllTimeBestSectorTime_12');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 13:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_13');
var best = $prop('MiniSectors.AllTimeBestSectorTime_13');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 14:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_14');
var best = $prop('MiniSectors.AllTimeBestSectorTime_14');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

**Sector 15:**
```javascript
var last = $prop('MiniSectors.LastLapSectorTime_15');
var best = $prop('MiniSectors.AllTimeBestSectorTime_15');
if (last == null || best == null || last < 0 || best < 0) return '#FFFFFF';
var delta = last - best;
if (delta < -0.001) return '#AA00FF';
if (delta > 0.001) return '#FF0000';
return '#AA00FF';
```

---

### Highlight Current Sector Row (Background Color binding)

Use this to highlight the row of the sector you're currently driving.

**Sector 1:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 1) return '#333333';  // Highlighted
return '#1A1A1A';  // Normal background
```

**Sector 2:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 2) return '#333333';
return '#1A1A1A';
```

**Sector 3:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 3) return '#333333';
return '#1A1A1A';
```

**Sector 4:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 4) return '#333333';
return '#1A1A1A';
```

**Sector 5:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 5) return '#333333';
return '#1A1A1A';
```

**Sector 6:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 6) return '#333333';
return '#1A1A1A';
```

**Sector 7:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 7) return '#333333';
return '#1A1A1A';
```

**Sector 8:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 8) return '#333333';
return '#1A1A1A';
```

**Sector 9:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 9) return '#333333';
return '#1A1A1A';
```

**Sector 10:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 10) return '#333333';
return '#1A1A1A';
```

**Sector 11:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 11) return '#333333';
return '#1A1A1A';
```

**Sector 12:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 12) return '#333333';
return '#1A1A1A';
```

**Sector 13:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 13) return '#333333';
return '#1A1A1A';
```

**Sector 14:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 14) return '#333333';
return '#1A1A1A';
```

**Sector 15:**
```javascript
var current = $prop('MiniSectors.CurrentSectorNumber');
if (current == 15) return '#333333';
return '#1A1A1A';
```

---

## Bonus: Additional Useful Displays

### Current Sector Live Time

Shows the live timing for the sector you're currently in:

```javascript
var time = $prop('MiniSectors.CurrentSectorTime');
if (time == null || time < 0) return '---';
return time.toFixed(3);
```

### Current Turn Name

```javascript
var turn = $prop('MiniSectors.CurrentTurn');
if (turn == null || turn == '') return 'Unknown';
return turn;
```

### Track Name

```javascript
var track = $prop('MiniSectors.TrackId');
if (track == null || track == '') return 'Unknown Track';
return track;
```

### Total Sectors Display

```javascript
var count = $prop('MiniSectors.SectorCountForTrack');
if (count == null || count <= 0) return 'No sector data';
return count + ' sectors';
```

---

## SimHub Dashboard Editor Tips

1. **Create as Overlay**: In dashboard properties, set type to "Overlay" for in-game display

2. **Use TextBlock for text**: Add TextBlock elements and bind their Text property using JavaScript

3. **Row structure**: Create a container (StackPanel or Grid) for each sector row

4. **Copy/paste rows**: Build one complete row, then duplicate and update the sector numbers

5. **Test with replay**: Use a replay file in SimHub to test without running the game

6. **Property browser**: Use SimHub's property browser to verify MiniSectors properties are available

7. **Performance**: JavaScript runs every frame - keep calculations simple

---

## Color Reference

| Color | Hex | Meaning |
|-------|-----|---------|
| Green | `#00FF00` | Faster than session best |
| Yellow | `#FFFF00` | Matched session best |
| Red | `#FF0000` | Slower than best |
| Purple | `#AA00FF` | All-time best beaten |
| White | `#FFFFFF` | No data |
| Dark Gray | `#1A1A1A` | Row background |
| Light Gray | `#333333` | Current sector highlight |
