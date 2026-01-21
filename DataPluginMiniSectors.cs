using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// Exposes the repository for SettingsControl to query records.
        /// </summary>
        internal ISectorBestRepository Repository => _repository;

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
            return new SettingsControl(this);
        }

        public void Init(PluginManager pluginManager)
        {
            // Handle assembly version mismatches (e.g., Newtonsoft.Json) by using
            // whatever version SimHub already loaded
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                return AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == requestedAssembly.Name);
            };

            SimHub.Logging.Current.Info("Starting plugin");

            Settings = this.ReadCommonSettings<PluginSettings>("GeneralSettings", () => new PluginSettings());

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

            // Declare an input mapping
            this.AddInputMapping(
                inputName: "InputPressed",
                inputPressed: (a, b) => { /* pressed */ },
                inputReleased: (a, b) => { /* released */ }
            );

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
                // SimHub doesn't have a direct grip level property for all games.
                // For ACC, you might access raw data. For now, we'll return empty
                // and this can be enhanced later with game-specific logic.
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}
