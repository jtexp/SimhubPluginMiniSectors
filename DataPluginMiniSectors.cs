using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Windows.Media;

namespace User.PluginMiniSectors
{
    [PluginDescription("Create mini sectors data")]
    [PluginAuthor("John Popplewell")]
    [PluginName("Minisectors")]
    public class DataPluginMiniSectors : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public PluginSettings Settings;

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
        // Repository and engine
        // --------------------------------------------------------------------

        private readonly ISectorBestRepository _repository;
        private readonly SectorTimingEngine _engine;
        private readonly TrackConditions _conditions = new TrackConditions();

        public DataPluginMiniSectors()
        {
            _repository = new SectorBestRepository();
            _engine = new SectorTimingEngine(_repository);
        }

        // --------------------------------------------------------------------
        // SimHub Plugin Methods
        // --------------------------------------------------------------------

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning || data.NewData == null)
            {
                _engine.Reset();
                return;
            }
            string trackId = TryGetTrackId(data);
            double tp = TryGetTrackPositionPercent(data);
            bool isLapValid = TryGetIsLapValid(data);
            double currentLapTimeSec = TryGetCurrentLapTime(data);

            // Extract track conditions
            _conditions.CarModel = TryGetCarModel(data);
            _conditions.WeatherType = TryGetWeatherType(data);
            _conditions.TrackTempCelsius = TryGetTrackTemp(data);
            _conditions.AirTempCelsius = TryGetAirTemp(data);
            _conditions.GripLevel = TryGetGripLevel(data);

            _engine.SetConditions(_conditions);
            _engine.Update(trackId, tp, currentLapTimeSec, isLapValid);

            // Sample event triggering logic
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
            return new SettingsControl(this);
        }

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting plugin");

            Settings = this.ReadCommonSettings<PluginSettings>("GeneralSettings", () => new PluginSettings()) ?? new PluginSettings();

            // Initialize the repository (creates tables if needed)
            _repository.Initialize();
            SimHub.Logging.Current.Info("SQLite repository initialized");

            // Existing sample property
            this.AttachDelegate(name: "CurrentDateTime", valueProvider: () => DateTime.Now);

            // Existing mapping properties
            this.AttachDelegate(name: "CurrentTurn", valueProvider: () => _engine.CurrentTurn);
            this.AttachDelegate(name: "TrackId", valueProvider: () => _engine.TrackId);
            this.AttachDelegate(name: "TrackPositionPercent", valueProvider: () => _engine.TrackPositionPercent);

            // Car model property
            this.AttachDelegate(name: "CurrentCarModel", valueProvider: () => _conditions.CarModel);

            // Sector index properties
            this.AttachDelegate(name: "CurrentSectorNumber", valueProvider: () => _engine.CurrentSectorNumber);
            this.AttachDelegate(name: "SectorCountForTrack", valueProvider: () => _engine.SectorCount);

            // Sector timing properties
            this.AttachDelegate(name: "CurrentSectorTime", valueProvider: () => _engine.CurrentSectorTime);
            this.AttachDelegate(name: "LastCompletedSectorTime", valueProvider: () => _engine.LastCompletedSectorTime);
            this.AttachDelegate(name: "LastCompletedSectorNumber", valueProvider: () => _engine.LastCompletedSectorNumber);

            // Expose per-sector times as properties (fixed list so you can reference them in SimHub formulas)
            for (int i = 1; i <= SectorTimingEngine.MaxSectors; i++)
            {
                int sectorIndex = i; // capture for closure

                this.AttachDelegate(
                    name: $"SectorTime_{sectorIndex:00}",
                    valueProvider: () => _engine.GetCurrentLapSectorTime(sectorIndex));

                this.AttachDelegate(
                    name: $"LastLapSectorTime_{sectorIndex:00}",
                    valueProvider: () => _engine.GetLastLapSectorTime(sectorIndex));

                this.AttachDelegate(
                    name: $"SessionBestSectorTime_{sectorIndex:00}",
                    valueProvider: () => _engine.GetSessionBestSectorTime(sectorIndex));

                this.AttachDelegate(
                    name: $"AllTimeBestSectorTime_{sectorIndex:00}",
                    valueProvider: () => _engine.GetAllTimeBestSectorTime(sectorIndex));
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

            // Initialize runtime state
            _engine.Reset();
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

        private static bool TryGetIsLapValid(in GameData data)
        {
            try
            {
                return data.NewData.IsLapValid;
            }
            catch
            {
                return false;
            }
        }

        private static double TryGetCurrentLapTime(in GameData data)
        {
            try
            {
                return data.NewData.CurrentLapTime.TotalSeconds;
            }
            catch
            {
                return 0.0;
            }
        }

        private static string TryGetCarModel(in GameData data)
        {
            try
            {
                return data.NewData.CarModel ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static string TryGetWeatherType(in GameData data)
        {
            try
            {
                // RoadWetness: 0 = dry, higher values = wetter
                // Using a simple categorization based on road wetness level
                double roadWetness = data.NewData.RoadWetness;

                if (roadWetness <= 0.0)
                    return "Dry";
                else if (roadWetness < 0.3)
                    return "Damp";
                else if (roadWetness < 0.7)
                    return "Wet";
                else
                    return "VeryWet";
            }
            catch
            {
                return "";
            }
        }

        private static double TryGetTrackTemp(in GameData data)
        {
            try
            {
                return data.NewData.RoadTemperature;
            }
            catch
            {
                return 0.0;
            }
        }

        private static double TryGetAirTemp(in GameData data)
        {
            try
            {
                return data.NewData.AirTemperature;
            }
            catch
            {
                return 0.0;
            }
        }

        private static string TryGetGripLevel(in GameData data)
        {
            try
            {
                // SimHub doesn't expose a direct grip level property across all games.
                // Game-specific implementations could access raw telemetry data in the future.
                // For now, returning empty string to indicate data is not available.
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}
