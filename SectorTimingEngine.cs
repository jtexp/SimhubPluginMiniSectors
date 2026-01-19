using System;

namespace User.PluginMiniSectors
{
    internal class SectorTimingEngine
    {
        // --------------------------------------------------------------------
        // Constants
        // --------------------------------------------------------------------

        public const int MaxSectors = 60;

        // --------------------------------------------------------------------
        // Public read-only properties
        // --------------------------------------------------------------------

        public string CurrentTurn { get; private set; } = "";
        public int CurrentSectorNumber { get; private set; } = 0;
        public int SectorCount { get; private set; } = 0;
        public string TrackId { get; private set; } = "";
        public double TrackPositionPercent { get; private set; } = 0.0;
        public double CurrentSectorTime { get; private set; } = 0.0;
        public double LastCompletedSectorTime { get; private set; } = 0.0;

        // --------------------------------------------------------------------
        // Internal state
        // --------------------------------------------------------------------

        private readonly double[] _currentLapSectorTimesSec = new double[MaxSectors + 1]; // index 1..MaxSectors
        private readonly double[] _lastLapSectorTimesSec = new double[MaxSectors + 1];    // index 1..MaxSectors
        private readonly double[] _sessionBestSectorTimesSec = new double[MaxSectors + 1]; // index 1..MaxSectors, -1 = unset

        private int _prevSectorNumber = 0;
        private double _prevTp = 0.0;

        private bool _hasLastUpdate = false;
        private DateTime _lastUpdateUtc = DateTime.MinValue;

        // --------------------------------------------------------------------
        // Indexed getters
        // --------------------------------------------------------------------

        public double GetCurrentLapSectorTime(int sector)
        {
            if (sector < 1 || sector > MaxSectors) return 0.0;
            return _currentLapSectorTimesSec[sector];
        }

        public double GetLastLapSectorTime(int sector)
        {
            if (sector < 1 || sector > MaxSectors) return 0.0;
            return _lastLapSectorTimesSec[sector];
        }

        public double GetSessionBestSectorTime(int sector)
        {
            if (sector < 1 || sector > MaxSectors) return -1.0;
            return _sessionBestSectorTimesSec[sector];
        }

        // --------------------------------------------------------------------
        // Static helper methods
        // --------------------------------------------------------------------

        /// <summary>
        /// Determines whether the session best should be updated for a sector.
        /// </summary>
        /// <param name="sectorTime">The completed sector time in seconds</param>
        /// <param name="currentBest">The current session best (-1 if unset)</param>
        /// <param name="isLapValid">Whether the lap is valid (no cuts, etc.)</param>
        /// <returns>True if the sector time should become the new session best</returns>
        public static bool ShouldUpdateSessionBest(double sectorTime, double currentBest, bool isLapValid)
        {
            if (!isLapValid) return false;
            if (sectorTime <= 0) return false;
            return currentBest < 0 || sectorTime < currentBest;
        }

        private static string GetCurrentTurn(string trackId, double tp)
        {
            if (string.IsNullOrWhiteSpace(trackId)) return "";
            if (tp < 0 || tp > 1) return "";

            if (!TrackData.TrackTurnMap.TryGetValue(trackId, out var ranges) || ranges == null || ranges.Length == 0)
                return "";

            for (int i = 0; i < ranges.Length; i++)
            {
                if (ranges[i].Contains(tp))
                    return ranges[i].Label;
            }

            return "";
        }

        /// <summary>
        /// Sector number logic:
        /// - Sector 1 is [0.0, end_of_corner_1]
        /// - Sector k is (end_of_corner_(k-1), end_of_corner_k]
        /// Uses the End of each RangeLabel as the sector boundary.
        /// </summary>
        private static int GetCurrentSectorNumber(string trackId, double tp, out int sectorCount)
        {
            sectorCount = 0;

            if (string.IsNullOrWhiteSpace(trackId)) return 0;
            if (tp < 0 || tp > 1) return 0;

            if (!TrackData.TrackTurnMap.TryGetValue(trackId, out var ranges) || ranges == null || ranges.Length == 0)
                return 0;

            sectorCount = ranges.Length;

            double prevEnd = 0.0;

            for (int i = 0; i < ranges.Length; i++)
            {
                double curEnd = ranges[i].End;

                if (curEnd < prevEnd)
                    return 0;

                bool inSector =
                    (i == 0 && tp >= 0.0 && tp <= curEnd) ||
                    (i > 0 && tp > prevEnd && tp <= curEnd);

                if (inSector)
                    return i + 1;

                prevEnd = curEnd;
            }

            // Treat "after last boundary" as last sector.
            return ranges.Length;
        }

        private static bool IsLapWrap(double prevTp, double curTp)
        {
            // TrackPositionPercent should move 0..1 and wrap.
            // Use a tolerant condition: "near end" -> "near start"
            return prevTp > 0.95 && curTp < 0.05;
        }

        // --------------------------------------------------------------------
        // Main methods
        // --------------------------------------------------------------------

        public void Reset()
        {
            CurrentTurn = "";
            CurrentSectorNumber = 0;
            SectorCount = 0;
            TrackId = "";
            TrackPositionPercent = 0.0;
            CurrentSectorTime = 0.0;
            LastCompletedSectorTime = 0.0;

            ResetTimingStateForNewSessionOrTrack();
        }

        private void ResetTimingStateForNewSessionOrTrack()
        {
            CurrentSectorTime = 0.0;
            LastCompletedSectorTime = 0.0;
            _prevSectorNumber = 0;
            _prevTp = 0.0;

            Array.Clear(_currentLapSectorTimesSec, 0, _currentLapSectorTimesSec.Length);
            Array.Clear(_lastLapSectorTimesSec, 0, _lastLapSectorTimesSec.Length);

            // Initialize session bests to -1 (unset)
            for (int i = 0; i < _sessionBestSectorTimesSec.Length; i++)
            {
                _sessionBestSectorTimesSec[i] = -1.0;
            }

            _hasLastUpdate = false;
            _lastUpdateUtc = DateTime.MinValue;
        }

        private void FinalizeLapToLastLap()
        {
            // Copy current lap sector times to last lap sector times
            Array.Copy(_currentLapSectorTimesSec, _lastLapSectorTimesSec, _currentLapSectorTimesSec.Length);

            // Reset current lap times
            Array.Clear(_currentLapSectorTimesSec, 0, _currentLapSectorTimesSec.Length);

            CurrentSectorTime = 0.0;
            LastCompletedSectorTime = 0.0;

            // After lap wrap, we will re-seed sector state on next update.
            _prevSectorNumber = 0;
        }

        public void Update(string trackId, double tp, DateTime nowUtc, bool isLapValid)
        {
            // Track change detection (track id can sometimes appear late; be tolerant)
            bool trackChanged = !string.Equals(TrackId ?? "", trackId ?? "", StringComparison.OrdinalIgnoreCase);
            if (trackChanged && !string.IsNullOrWhiteSpace(trackId))
            {
                // new track => reset timing and lap storage
                ResetTimingStateForNewSessionOrTrack();
            }

            TrackId = trackId ?? "";
            TrackPositionPercent = tp;

            // Compute derived values
            CurrentTurn = GetCurrentTurn(TrackId, TrackPositionPercent);
            int sectorNumber = GetCurrentSectorNumber(TrackId, TrackPositionPercent, out int sectorCount);
            SectorCount = sectorCount;
            CurrentSectorNumber = sectorNumber;

            // Update dt
            double dtSec = 0.0;
            if (_hasLastUpdate)
            {
                dtSec = (nowUtc - _lastUpdateUtc).TotalSeconds;

                // Guard: dt can spike when game pauses / plugin stalls.
                // Clamp to something reasonable to avoid polluting sector times.
                if (dtSec < 0) dtSec = 0;
                if (dtSec > 1.0) dtSec = 1.0; // you can relax this if you prefer
            }
            _lastUpdateUtc = nowUtc;
            _hasLastUpdate = true;

            // If we cannot map a sector, do not accumulate (prevents garbage times)
            if (sectorNumber <= 0)
            {
                _prevTp = tp;
                return;
            }

            // Lap wrap detection
            bool lapWrapped = IsLapWrap(_prevTp, tp) || (_prevSectorNumber > 0 && sectorNumber < _prevSectorNumber);

            if (lapWrapped)
            {
                FinalizeLapToLastLap();
                _prevTp = tp;
                _prevSectorNumber = sectorNumber;
                return;
            }

            // Sector transition detection
            if (_prevSectorNumber > 0 && sectorNumber != _prevSectorNumber)
            {
                // Finalize the previous sector time (store elapsed)
                int prevSector = _prevSectorNumber;

                if (prevSector >= 1 && prevSector <= MaxSectors)
                {
                    _currentLapSectorTimesSec[prevSector] = CurrentSectorTime;

                    // Update session best if applicable
                    if (ShouldUpdateSessionBest(CurrentSectorTime, _sessionBestSectorTimesSec[prevSector], isLapValid))
                    {
                        _sessionBestSectorTimesSec[prevSector] = CurrentSectorTime;
                    }
                }

                LastCompletedSectorTime = CurrentSectorTime;

                // Reset accumulator for new sector
                CurrentSectorTime = 0.0;
            }

            // Accumulate for current sector
            CurrentSectorTime += dtSec;

            _prevTp = tp;
            _prevSectorNumber = sectorNumber;
        }
    }
}
