namespace User.PluginMiniSectors
{
    internal class TrackConditions
    {
        public string CarModel { get; set; } = "";
        public string WeatherType { get; set; } = "";      // "Dry", "LightRain", "Rain", etc.
        public double TrackTempCelsius { get; set; } = 0.0;
        public double AirTempCelsius { get; set; } = 0.0;
        public string GripLevel { get; set; } = "";        // "Green", "Fast", "Optimal"
    }
}
