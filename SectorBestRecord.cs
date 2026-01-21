using System;

namespace User.PluginMiniSectors
{
    /// <summary>
    /// Data model representing a stored sector best time record.
    /// Used for displaying records in the Settings UI DataGrid.
    /// </summary>
    public class SectorBestRecord
    {
        public int Id { get; set; }
        public string TrackId { get; set; }
        public int SectorNumber { get; set; }
        public double BestTimeSec { get; set; }
        public string CarModel { get; set; }
        public string WeatherType { get; set; }
        public double TrackTempCelsius { get; set; }
        public double AirTempCelsius { get; set; }
        public string GripLevel { get; set; }
        public DateTime RecordedAt { get; set; }

        /// <summary>
        /// Best time formatted for display (e.g., "1:23.456")
        /// </summary>
        public string BestTimeFormatted =>
            TimeSpan.FromSeconds(BestTimeSec).ToString(@"m\:ss\.fff");
    }
}
