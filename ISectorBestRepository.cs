using System.Collections.Generic;

namespace User.PluginMiniSectors
{
    internal interface ISectorBestRepository
    {
        void Initialize();
        double GetAllTimeBest(string trackId, int sectorNumber, string carModel);
        void SaveSectorBest(string trackId, int sectorNumber, double timeSec,
                            string carModel, TrackConditions conditions);
        void LoadAllTimeBestsForTrack(string trackId, string carModel, double[] targetArray);

        /// <summary>
        /// Gets recent sector best records, ordered by recorded_at DESC.
        /// </summary>
        /// <param name="limit">Maximum number of records to return (default 500)</param>
        List<SectorBestRecord> GetRecentRecords(int limit = 500);

        /// <summary>
        /// Gets all sector best records, ordered by recorded_at DESC.
        /// </summary>
        List<SectorBestRecord> GetAllRecords();
    }
}
