namespace User.PluginMiniSectors
{
    internal interface ISectorBestRepository
    {
        void Initialize();
        double GetAllTimeBest(string trackId, int sectorNumber, string carModel);
        void SaveSectorBest(string trackId, int sectorNumber, double timeSec,
                            string carModel, TrackConditions conditions);
        void LoadAllTimeBestsForTrack(string trackId, string carModel, double[] targetArray);
    }
}
