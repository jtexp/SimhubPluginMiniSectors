using System;
using System.Data.SQLite;
using System.IO;

namespace User.PluginMiniSectors
{
    internal class SectorBestRepository : ISectorBestRepository
    {
        private readonly string _connectionString;

        public SectorBestRepository()
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SimHub", "PluginsData", "User.PluginMiniSectors");

            Directory.CreateDirectory(folder);
            var dbPath = Path.Combine(folder, "MiniSectors.sqlite");
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public SectorBestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS sector_bests (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            track_id TEXT NOT NULL,
                            sector_number INTEGER NOT NULL,
                            best_time_sec REAL NOT NULL,
                            car_model TEXT NOT NULL,
                            weather_type TEXT,
                            track_temp_celsius REAL,
                            air_temp_celsius REAL,
                            grip_level TEXT,
                            recorded_at TEXT NOT NULL,
                            UNIQUE(track_id, sector_number, car_model)
                        );

                        CREATE INDEX IF NOT EXISTS idx_sector_bests_lookup
                            ON sector_bests(track_id, car_model);
                    ";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public double GetAllTimeBest(string trackId, int sectorNumber, string carModel)
        {
            if (string.IsNullOrWhiteSpace(trackId) || string.IsNullOrWhiteSpace(carModel))
                return -1.0;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT best_time_sec FROM sector_bests
                        WHERE track_id = @trackId
                          AND sector_number = @sectorNumber
                          AND car_model = @carModel
                    ";
                    cmd.Parameters.AddWithValue("@trackId", trackId);
                    cmd.Parameters.AddWithValue("@sectorNumber", sectorNumber);
                    cmd.Parameters.AddWithValue("@carModel", carModel);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDouble(result);
                    }
                }
            }

            return -1.0;
        }

        public void SaveSectorBest(string trackId, int sectorNumber, double timeSec,
                                   string carModel, TrackConditions conditions)
        {
            if (string.IsNullOrWhiteSpace(trackId) || string.IsNullOrWhiteSpace(carModel))
                return;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO sector_bests (
                            track_id, sector_number, best_time_sec, car_model,
                            weather_type, track_temp_celsius, air_temp_celsius, grip_level, recorded_at
                        )
                        VALUES (
                            @trackId, @sectorNumber, @timeSec, @carModel,
                            @weatherType, @trackTemp, @airTemp, @gripLevel, @recordedAt
                        )
                        ON CONFLICT(track_id, sector_number, car_model) DO UPDATE SET
                            best_time_sec = @timeSec,
                            weather_type = @weatherType,
                            track_temp_celsius = @trackTemp,
                            air_temp_celsius = @airTemp,
                            grip_level = @gripLevel,
                            recorded_at = @recordedAt
                        WHERE @timeSec < best_time_sec
                    ";
                    cmd.Parameters.AddWithValue("@trackId", trackId);
                    cmd.Parameters.AddWithValue("@sectorNumber", sectorNumber);
                    cmd.Parameters.AddWithValue("@timeSec", timeSec);
                    cmd.Parameters.AddWithValue("@carModel", carModel);
                    cmd.Parameters.AddWithValue("@weatherType", conditions?.WeatherType ?? "");
                    cmd.Parameters.AddWithValue("@trackTemp", conditions?.TrackTempCelsius ?? 0.0);
                    cmd.Parameters.AddWithValue("@airTemp", conditions?.AirTempCelsius ?? 0.0);
                    cmd.Parameters.AddWithValue("@gripLevel", conditions?.GripLevel ?? "");
                    cmd.Parameters.AddWithValue("@recordedAt", DateTime.UtcNow.ToString("o"));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LoadAllTimeBestsForTrack(string trackId, string carModel, double[] targetArray)
        {
            // Initialize all to -1 (unset)
            for (int i = 0; i < targetArray.Length; i++)
            {
                targetArray[i] = -1.0;
            }

            if (string.IsNullOrWhiteSpace(trackId) || string.IsNullOrWhiteSpace(carModel))
                return;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT sector_number, best_time_sec FROM sector_bests
                        WHERE track_id = @trackId AND car_model = @carModel
                    ";
                    cmd.Parameters.AddWithValue("@trackId", trackId);
                    cmd.Parameters.AddWithValue("@carModel", carModel);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int sectorNumber = reader.GetInt32(0);
                            double bestTime = reader.GetDouble(1);

                            if (sectorNumber >= 1 && sectorNumber < targetArray.Length)
                            {
                                targetArray[sectorNumber] = bestTime;
                            }
                        }
                    }
                }
            }
        }
    }
}
