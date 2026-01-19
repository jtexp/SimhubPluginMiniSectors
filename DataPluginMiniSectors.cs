using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows.Media;

namespace User.PluginMiniSectors
{
    [PluginDescription("Create mini sectors data")]
    [PluginAuthor("John Popplewell")]
    [PluginName("Minisectors")]
    public class DataPluginMiniSectors : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public DataPluginDemoSettings Settings;

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "Minisectors";

        // --------------------------------------------------------------------
        // Current Turn + Sector Properties
        // --------------------------------------------------------------------

        private string _currentTurn = "";
        private int _currentSectorNumber = 0;          // 1-based; 0 = unknown/unmapped
        private int _sectorCountForTrack = 0;          // number of sectors derived from corners (end boundaries)
        private string _currentTrackId = "";
        private double _trackPositionPercent = 0.0;

        // --------------------------------------------------------------------
        // Sector timing state
        // --------------------------------------------------------------------

        // Max sectors you'll ever expose via properties (enough headroom for Nordschleife lists)
        private const int MaxExposedSectors = 60;

        // We store times as seconds
        private readonly double[] _currentLapSectorTimesSec = new double[MaxExposedSectors + 1]; // index 1..MaxExposedSectors
        private readonly double[] _lastLapSectorTimesSec = new double[MaxExposedSectors + 1];    // index 1..MaxExposedSectors

        private double _currentSectorElapsedSec = 0.0;
        private double _lastCompletedSectorTimeSec = 0.0;

        private int _prevSectorNumber = 0;
        private double _prevTp = 0.0;

        private bool _hasLastUpdate = false;
        private DateTime _lastUpdateUtc = DateTime.MinValue;


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

        private void ResetTimingStateForNewSessionOrTrack()
        {
            _currentSectorElapsedSec = 0.0;
            _lastCompletedSectorTimeSec = 0.0;
            _prevSectorNumber = 0;
            _prevTp = 0.0;

            Array.Clear(_currentLapSectorTimesSec, 0, _currentLapSectorTimesSec.Length);
            Array.Clear(_lastLapSectorTimesSec, 0, _lastLapSectorTimesSec.Length);

            _hasLastUpdate = false;
            _lastUpdateUtc = DateTime.MinValue;
        }

        private void FinalizeLapToLastLap()
        {
            // Copy current lap sector times to last lap sector times
            Array.Copy(_currentLapSectorTimesSec, _lastLapSectorTimesSec, _currentLapSectorTimesSec.Length);

            // Reset current lap times
            Array.Clear(_currentLapSectorTimesSec, 0, _currentLapSectorTimesSec.Length);

            _currentSectorElapsedSec = 0.0;
            _lastCompletedSectorTimeSec = 0.0;

            // After lap wrap, we will re-seed sector state on next update.
            _prevSectorNumber = 0;
        }

        private static bool IsLapWrap(double prevTp, double curTp)
        {
            // TrackPositionPercent should move 0..1 and wrap.
            // Use a tolerant condition: "near end" -> "near start"
            return prevTp > 0.95 && curTp < 0.05;
        }

        private void UpdateTurnSectorAndTimingFromTelemetry(string trackId, double tp, DateTime nowUtc)
        {
            // Track change detection (track id can sometimes appear late; be tolerant)
            bool trackChanged = !string.Equals(_currentTrackId ?? "", trackId ?? "", StringComparison.OrdinalIgnoreCase);
            if (trackChanged && !string.IsNullOrWhiteSpace(trackId))
            {
                // new track => reset timing and lap storage
                ResetTimingStateForNewSessionOrTrack();
            }

            _currentTrackId = trackId ?? "";
            _trackPositionPercent = tp;

            // Compute derived values
            _currentTurn = GetCurrentTurn(_currentTrackId, _trackPositionPercent);
            int sectorNumber = GetCurrentSectorNumber(_currentTrackId, _trackPositionPercent, out _sectorCountForTrack);
            _currentSectorNumber = sectorNumber;

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

                if (prevSector >= 1 && prevSector <= MaxExposedSectors)
                {
                    _currentLapSectorTimesSec[prevSector] = _currentSectorElapsedSec;
                }

                _lastCompletedSectorTimeSec = _currentSectorElapsedSec;

                // Reset accumulator for new sector
                _currentSectorElapsedSec = 0.0;
            }

            // Accumulate for current sector
            _currentSectorElapsedSec += dtSec;

            _prevTp = tp;
            _prevSectorNumber = sectorNumber;
        }

        // --------------------------------------------------------------------
        // SimHub Plugin Methods
        // --------------------------------------------------------------------

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning || data.NewData == null)
            {
                _currentTurn = "";
                _currentSectorNumber = 0;
                _sectorCountForTrack = 0;
                _currentTrackId = "";
                _trackPositionPercent = 0.0;

                // Timing state should reset when the game stops, otherwise you get dt spikes on resume.
                ResetTimingStateForNewSessionOrTrack();
                return;
            }

            string trackId = TryGetTrackId(data);
            double tp = TryGetTrackPositionPercent(data);

            UpdateTurnSectorAndTimingFromTelemetry(trackId, tp, DateTime.UtcNow);

            // Existing sample logic retained (and corrected)
            if (data.OldData != null)
            {
                if (data.OldData.SpeedKmh < Settings.SpeedWarningLevel && data.NewData.SpeedKmh >= Settings.SpeedWarningLevel)
                {
                    this.TriggerEvent("SpeedWarning");
                }
            }
        }

        public void End(PluginManager pluginManager)
        {
            this.SaveCommonSettings("GeneralSettings", Settings);
        }

        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControlDemo(this);
        }

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting plugin");

            Settings = this.ReadCommonSettings<DataPluginDemoSettings>("GeneralSettings", () => new DataPluginDemoSettings());



            // Existing sample property
            this.AttachDelegate(name: "CurrentDateTime", valueProvider: () => DateTime.Now);

            // Existing mapping properties
            this.AttachDelegate(name: "CurrentTurn", valueProvider: () => _currentTurn);
            this.AttachDelegate(name: "TrackId", valueProvider: () => _currentTrackId);
            this.AttachDelegate(name: "TrackPositionPercent", valueProvider: () => _trackPositionPercent);

            // Sector index properties
            this.AttachDelegate(name: "CurrentSectorNumber", valueProvider: () => _currentSectorNumber);
            this.AttachDelegate(name: "SectorCountForTrack", valueProvider: () => _sectorCountForTrack);

            // Sector timing properties
            this.AttachDelegate(name: "CurrentSectorTime", valueProvider: () => _currentSectorElapsedSec);
            this.AttachDelegate(name: "LastCompletedSectorTime", valueProvider: () => _lastCompletedSectorTimeSec);

            // Expose per-sector times as properties (fixed list so you can reference them in SimHub formulas)
            for (int i = 1; i <= MaxExposedSectors; i++)
            {
                int sectorIndex = i; // capture for closure

                this.AttachDelegate(
                    name: $"SectorTime_{sectorIndex:00}",
                    valueProvider: () =>
                    {
                        if (sectorIndex < 1 || sectorIndex > MaxExposedSectors) return 0.0;
                        return _currentLapSectorTimesSec[sectorIndex];
                    });

                this.AttachDelegate(
                    name: $"LastLapSectorTime_{sectorIndex:00}",
                    valueProvider: () =>
                    {
                        if (sectorIndex < 1 || sectorIndex > MaxExposedSectors) return 0.0;
                        return _lastLapSectorTimesSec[sectorIndex];
                    });
            }

            // Declare an event
            this.AddEvent(eventName: "SpeedWarning");

            // Declare actions
            this.AddAction(
                actionName: "IncrementSpeedWarning",
                actionStart: (a, b) =>
                {
                    Settings.SpeedWarningLevel++;
                    SimHub.Logging.Current.Info("Speed warning changed");
                });

            this.AddAction(
                actionName: "DecrementSpeedWarning",
                actionStart: (a, b) =>
                {
                    Settings.SpeedWarningLevel--;
                });

            // Declare an input mapping
            this.AddInputMapping(
                inputName: "InputPressed",
                inputPressed: (a, b) => { /* pressed */ },
                inputReleased: (a, b) => { /* released */ }
            );



            var folder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "SimHub", "PluginsData", "User.PluginMiniSectors");

            Directory.CreateDirectory(folder);
            var dbPath = Path.Combine(folder, "MiniSectors.sqlite");

            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS test (id INTEGER PRIMARY KEY, v TEXT);";
                    cmd.ExecuteNonQuery();
                }
            }

            SimHub.Logging.Current.Info("SQLite initialized OK");


            // Initialize runtime state
            ResetTimingStateForNewSessionOrTrack();



        }

        // --------------------------------------------------------------------
        // Telemetry field extraction
        // --------------------------------------------------------------------

        private static string TryGetTrackId(in GameData data)
        {
            try
            {
                return data.NewData.TrackName;
            }
            catch
            {
                return "";
            }
        }

        private static double TryGetTrackPositionPercent(in GameData data)
        {
            try
            {
                return data.NewData.TrackPositionPercent;
            }
            catch
            {
                return 0.0;
            }
        }
    }
}
