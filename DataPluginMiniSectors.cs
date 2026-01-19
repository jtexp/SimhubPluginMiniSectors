using GameReaderCommon;
using SimHub.Plugins;
using System;
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
        // Sector timing engine
        // --------------------------------------------------------------------

        private readonly SectorTimingEngine _engine = new SectorTimingEngine();

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

            _engine.Update(trackId, tp, DateTime.UtcNow, isLapValid);

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
            this.AttachDelegate(name: "CurrentTurn", valueProvider: () => _engine.CurrentTurn);
            this.AttachDelegate(name: "TrackId", valueProvider: () => _engine.TrackId);
            this.AttachDelegate(name: "TrackPositionPercent", valueProvider: () => _engine.TrackPositionPercent);

            // Sector index properties
            this.AttachDelegate(name: "CurrentSectorNumber", valueProvider: () => _engine.CurrentSectorNumber);
            this.AttachDelegate(name: "SectorCountForTrack", valueProvider: () => _engine.SectorCount);

            // Sector timing properties
            this.AttachDelegate(name: "CurrentSectorTime", valueProvider: () => _engine.CurrentSectorTime);
            this.AttachDelegate(name: "LastCompletedSectorTime", valueProvider: () => _engine.LastCompletedSectorTime);

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
    }
}
