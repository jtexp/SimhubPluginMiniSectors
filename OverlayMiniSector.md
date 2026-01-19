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
