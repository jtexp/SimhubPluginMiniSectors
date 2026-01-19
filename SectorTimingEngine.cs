using System;

namespace User.PluginMiniSectors
{
    internal class SectorTimingEngine
    {
        // --------------------------------------------------------------------
        // Constants
        // --------------------------------------------------------------------

        public const int MaxSectors = 15;

        // --------------------------------------------------------------------
        // Public read-only properties
        // --------------------------------------------------------------------

        public string CurrentTurn { get; private set; } = "";
        public int CurrentSectorNumber { get; private set; } = 0;
        public int SectorCount { get; private set; } = 0;
        public string TrackId { get; private set; } = "";
        public double TrackPositionPercent { get; private set; } = 0.0;
        public double CurrentSectorTime { get; private set; } = -1.0;
        public double LastCompletedSectorTime { get; private set; } = -1.0;
        public int LastCompletedSectorNumber { get; private set; } = 0;

        // --------------------------------------------------------------------
        // Internal state
        // --------------------------------------------------------------------

        private readonly double[] _currentLapSectorTimesSec = new double[MaxSectors + 1]; // index 1..MaxSectors
        private readonly double[] _lastLapSectorTimesSec = new double[MaxSectors + 1];    // index 1..MaxSectors
        private readonly double[] _sessionBestSectorTimesSec = new double[MaxSectors + 1]; // index 1..MaxSectors, -1 = unset
        private readonly double[] _allTimeBestSectorTimesSec = new double[MaxSectors + 1]; // index 1..MaxSectors, -1 = unset

        private int _prevSectorNumber = 0;
        private double _prevTp = 0.0;

        private double _sectorStartLapTimeSec = 0.0;
        private double _prevLapTimeSec = 0.0;

        private readonly ISectorBestRepository _repository;
        private TrackConditions _currentConditions;
        private string _currentCarModel = "";

        // --------------------------------------------------------------------
        // Constructor
        // --------------------------------------------------------------------

        public SectorTimingEngine() : this(null)
        {
        }

        public SectorTimingEngine(ISectorBestRepository repository)
        {
            _repository = repository;

            // Initialize all-time bests to -1 (unset)
            for (int i = 0; i < _allTimeBestSectorTimesSec.Length; i++)
            {
                _allTimeBestSectorTimesSec[i] = -1.0;
            }
        }

        // --------------------------------------------------------------------
        // Indexed getters
        // --------------------------------------------------------------------

        public double GetCurrentLapSectorTime(int sector)
        {
            if (sector < 1 || sector > MaxSectors) return -1.0;
            return _currentLapSectorTimesSec[sector];
        }

        public double GetLastLapSectorTime(int sector)
        {
            if (sector < 1 || sector > MaxSectors) return -1.0;
            return _lastLapSectorTimesSec[sector];
        }

        public double GetSessionBestSectorTime(int sector)
        {
            if (sector < 1 || sector > MaxSectors) return -1.0;
            return _sessionBestSectorTimesSec[sector];
        }

        public double GetAllTimeBestSectorTime(int sector)
        {
            if (sector < 1 || sector > MaxSectors) return -1.0;
            return _allTimeBestSectorTimesSec[sector];
        }

        // --------------------------------------------------------------------
        // Condition setter
        // --------------------------------------------------------------------

        public void SetConditions(TrackConditions conditions)
        {
            _currentConditions = conditions;
            if (conditions != null)
            {
                _currentCarModel = conditions.CarModel ?? "";
            }
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

        private static void InitializeArrayToUnset(double[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = -1.0;
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
            CurrentSectorTime = -1.0;
            LastCompletedSectorTime = -1.0;
            LastCompletedSectorNumber = 0;

            ResetTimingStateForNewSessionOrTrack();
        }

        private void ResetTimingStateForNewSessionOrTrack()
        {
            CurrentSectorTime = -1.0;
            LastCompletedSectorTime = -1.0;
            _prevSectorNumber = 0;
            _prevTp = 0.0;

            InitializeArrayToUnset(_currentLapSectorTimesSec);
            InitializeArrayToUnset(_lastLapSectorTimesSec);

            // Initialize session bests to -1 (unset)
            for (int i = 0; i < _sessionBestSectorTimesSec.Length; i++)
            {
                _sessionBestSectorTimesSec[i] = -1.0;
            }

            // Initialize all-time bests to -1 (unset) - will be loaded from DB on track change
            for (int i = 0; i < _allTimeBestSectorTimesSec.Length; i++)
            {
                _allTimeBestSectorTimesSec[i] = -1.0;
            }

            _sectorStartLapTimeSec = 0.0;
            _prevLapTimeSec = 0.0;
        }

        private void FinalizeLapToLastLap()
        {
            // Copy current lap sector times to last lap sector times
            Array.Copy(_currentLapSectorTimesSec, _lastLapSectorTimesSec, _currentLapSectorTimesSec.Length);

            // Reset current lap times
            InitializeArrayToUnset(_currentLapSectorTimesSec);

            CurrentSectorTime = -1.0;
            LastCompletedSectorTime = -1.0;

            // After lap wrap, we will re-seed sector state on next update.
            _prevSectorNumber = 0;
        }

        public void Update(string trackId, double tp, double currentLapTimeSec, bool isLapValid)
        {
            // Track change detection (track id can sometimes appear late; be tolerant)
            bool trackChanged = !string.Equals(TrackId ?? "", trackId ?? "", StringComparison.OrdinalIgnoreCase);
            bool carModelChanged = _currentConditions != null &&
                                   !string.Equals(_currentCarModel ?? "", _currentConditions.CarModel ?? "", StringComparison.OrdinalIgnoreCase);

            if ((trackChanged || carModelChanged) && !string.IsNullOrWhiteSpace(trackId))
            {
                // new track or car => reset timing and lap storage
                ResetTimingStateForNewSessionOrTrack();

                // Update current car model
                if (_currentConditions != null)
                {
                    _currentCarModel = _currentConditions.CarModel ?? "";
                }

                // Load all-time bests from repository
                if (_repository != null && !string.IsNullOrWhiteSpace(_currentCarModel))
                {
                    _repository.LoadAllTimeBestsForTrack(trackId, _currentCarModel, _allTimeBestSectorTimesSec);
                }
            }

            TrackId = trackId ?? "";
            TrackPositionPercent = tp;

            // Compute derived values
            CurrentTurn = GetCurrentTurn(TrackId, TrackPositionPercent);
            int sectorNumber = GetCurrentSectorNumber(TrackId, TrackPositionPercent, out int sectorCount);
            SectorCount = sectorCount;
            CurrentSectorNumber = sectorNumber;

            // If we cannot map a sector, do not update timing (prevents garbage times)
            if (sectorNumber <= 0)
            {
                _prevTp = tp;
                _prevLapTimeSec = currentLapTimeSec;
                return;
            }

            // Lap wrap detection: use lap time reset as the authoritative signal
            // This is the most reliable indicator - the game knows when a lap is completed
            bool lapTimeReset = currentLapTimeSec < _prevLapTimeSec - 1.0; // Allow small jitter
            bool lapWrapped = lapTimeReset;

            if (lapWrapped)
            {
                // Finalize the last sector's time before copying to last lap
                // This captures the time from the last corner boundary to the start/finish line
                if (_prevSectorNumber > 0 && _prevSectorNumber <= MaxSectors)
                {
                    // If lap time has reset, use the previous frame's lap time; otherwise use current
                    // (position wrap can be detected before lap time resets in some sims)
                    double effectiveLapTime = lapTimeReset ? _prevLapTimeSec : currentLapTimeSec;
                    double finalSectorTime = effectiveLapTime - _sectorStartLapTimeSec;

                    // Minimum sector time threshold to filter out glitches
                    const double MinSectorTimeSec = 0.5;
                    if (finalSectorTime >= MinSectorTimeSec)
                    {
                        _currentLapSectorTimesSec[_prevSectorNumber] = finalSectorTime;

                        // Update session best if applicable
                        if (ShouldUpdateSessionBest(finalSectorTime, _sessionBestSectorTimesSec[_prevSectorNumber], isLapValid))
                        {
                            _sessionBestSectorTimesSec[_prevSectorNumber] = finalSectorTime;

                            // Check if this also beats all-time best, and persist if so
                            bool beatsAllTimeBest = _allTimeBestSectorTimesSec[_prevSectorNumber] < 0 ||
                                                    finalSectorTime < _allTimeBestSectorTimesSec[_prevSectorNumber];
                            if (beatsAllTimeBest)
                            {
                                _allTimeBestSectorTimesSec[_prevSectorNumber] = finalSectorTime;

                                // Persist to repository
                                if (_repository != null && !string.IsNullOrWhiteSpace(_currentCarModel))
                                {
                                    _repository.SaveSectorBest(TrackId, _prevSectorNumber, finalSectorTime,
                                                               _currentCarModel, _currentConditions);
                                }
                            }
                        }

                        LastCompletedSectorTime = finalSectorTime;
                        LastCompletedSectorNumber = _prevSectorNumber;
                    }
                }

                FinalizeLapToLastLap();
                _sectorStartLapTimeSec = currentLapTimeSec;
                _prevTp = tp;
                _prevLapTimeSec = currentLapTimeSec;
                _prevSectorNumber = sectorNumber;
                return;
            }

            // Sector transition detection
            // Only allow forward sector transitions (sector increase) during a lap
            // Sector decreases mid-lap are position glitches and should be ignored
            if (_prevSectorNumber > 0 && sectorNumber > _prevSectorNumber)
            {
                // Finalize the previous sector time
                int prevSector = _prevSectorNumber;
                double completedSectorTime = currentLapTimeSec - _sectorStartLapTimeSec;

                // Minimum sector time threshold to filter out glitches
                const double MinSectorTimeSec = 0.5;
                if (prevSector >= 1 && prevSector <= MaxSectors && completedSectorTime >= MinSectorTimeSec)
                {
                    _currentLapSectorTimesSec[prevSector] = completedSectorTime;

                    // Update session best if applicable
                    if (ShouldUpdateSessionBest(completedSectorTime, _sessionBestSectorTimesSec[prevSector], isLapValid))
                    {
                        _sessionBestSectorTimesSec[prevSector] = completedSectorTime;

                        // Check if this also beats all-time best, and persist if so
                        bool beatsAllTimeBest = _allTimeBestSectorTimesSec[prevSector] < 0 ||
                                                completedSectorTime < _allTimeBestSectorTimesSec[prevSector];
                        if (beatsAllTimeBest)
                        {
                            _allTimeBestSectorTimesSec[prevSector] = completedSectorTime;

                            // Persist to repository
                            if (_repository != null && !string.IsNullOrWhiteSpace(_currentCarModel))
                            {
                                _repository.SaveSectorBest(TrackId, prevSector, completedSectorTime,
                                                           _currentCarModel, _currentConditions);
                            }
                        }
                    }

                    LastCompletedSectorTime = completedSectorTime;
                    LastCompletedSectorNumber = prevSector;
                }

                // Start timing new sector from current lap time
                _sectorStartLapTimeSec = currentLapTimeSec;
            }

            // Calculate current sector time from lap time
            CurrentSectorTime = currentLapTimeSec - _sectorStartLapTimeSec;

            _prevTp = tp;
            _prevLapTimeSec = currentLapTimeSec;

            // Only update _prevSectorNumber for forward transitions or initial state
            // This prevents backward glitches from corrupting subsequent forward transitions
            if (_prevSectorNumber == 0 || sectorNumber >= _prevSectorNumber)
            {
                // Initialize sector start time when first entering a sector
                if (_prevSectorNumber == 0)
                {
                    _sectorStartLapTimeSec = currentLapTimeSec;
                }
                _prevSectorNumber = sectorNumber;
            }
        }
    }
}
