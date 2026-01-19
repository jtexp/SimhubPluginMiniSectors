using System;
using System.Data.SQLite;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using User.PluginMiniSectors;

namespace User.PluginMiniSectors.Tests
{
    [TestClass]
    public class SectorBestRepositoryTests
    {
        private string _testDbPath;
        private string _connectionString;
        private SectorBestRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Create a unique temp database for each test
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_minisectors_{Guid.NewGuid()}.sqlite");
            _connectionString = $"Data Source={_testDbPath};Version=3;";
            _repository = new SectorBestRepository(_connectionString);
            _repository.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete the test database
            if (File.Exists(_testDbPath))
            {
                // Force garbage collection to release SQLite connections
                GC.Collect();
                GC.WaitForPendingFinalizers();
                try
                {
                    File.Delete(_testDbPath);
                }
                catch
                {
                    // Ignore cleanup errors in tests
                }
            }
        }

        [TestMethod]
        public void Initialize_CreatesTableSuccessfully()
        {
            // Table should exist after Initialize
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='sector_bests'";
                    var result = cmd.ExecuteScalar();
                    Assert.IsNotNull(result);
                    Assert.AreEqual("sector_bests", result.ToString());
                }
            }
        }

        [TestMethod]
        public void GetAllTimeBest_ReturnsNegativeOneWhenNoData()
        {
            double result = _repository.GetAllTimeBest("monza", 1, "ferrari_488_gt3");
            Assert.AreEqual(-1.0, result);
        }

        [TestMethod]
        public void GetAllTimeBest_ReturnsNegativeOneForEmptyTrackId()
        {
            double result = _repository.GetAllTimeBest("", 1, "ferrari_488_gt3");
            Assert.AreEqual(-1.0, result);
        }

        [TestMethod]
        public void GetAllTimeBest_ReturnsNegativeOneForEmptyCarModel()
        {
            double result = _repository.GetAllTimeBest("monza", 1, "");
            Assert.AreEqual(-1.0, result);
        }

        [TestMethod]
        public void SaveSectorBest_SavesAndRetrievesTime()
        {
            var conditions = new TrackConditions
            {
                CarModel = "ferrari_488_gt3",
                WeatherType = "Dry",
                TrackTempCelsius = 30.0,
                AirTempCelsius = 25.0,
                GripLevel = "Optimal"
            };

            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);

            double result = _repository.GetAllTimeBest("monza", 1, "ferrari_488_gt3");
            Assert.AreEqual(15.5, result, 0.001);
        }

        [TestMethod]
        public void SaveSectorBest_UpdatesOnlyWhenFaster()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };

            // Save initial time
            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);

            // Try to save slower time
            _repository.SaveSectorBest("monza", 1, 16.0, "ferrari_488_gt3", conditions);

            // Should still have faster time
            double result = _repository.GetAllTimeBest("monza", 1, "ferrari_488_gt3");
            Assert.AreEqual(15.5, result, 0.001);
        }

        [TestMethod]
        public void SaveSectorBest_UpdatesWhenFaster()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };

            // Save initial time
            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);

            // Save faster time
            _repository.SaveSectorBest("monza", 1, 14.8, "ferrari_488_gt3", conditions);

            // Should have faster time
            double result = _repository.GetAllTimeBest("monza", 1, "ferrari_488_gt3");
            Assert.AreEqual(14.8, result, 0.001);
        }

        [TestMethod]
        public void SaveSectorBest_SeparatesByCar()
        {
            var ferrariConditions = new TrackConditions { CarModel = "ferrari_488_gt3" };
            var porscheConditions = new TrackConditions { CarModel = "porsche_991ii_gt3_r" };

            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", ferrariConditions);
            _repository.SaveSectorBest("monza", 1, 16.2, "porsche_991ii_gt3_r", porscheConditions);

            double ferrariTime = _repository.GetAllTimeBest("monza", 1, "ferrari_488_gt3");
            double porscheTime = _repository.GetAllTimeBest("monza", 1, "porsche_991ii_gt3_r");

            Assert.AreEqual(15.5, ferrariTime, 0.001);
            Assert.AreEqual(16.2, porscheTime, 0.001);
        }

        [TestMethod]
        public void SaveSectorBest_SeparatesBySector()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };

            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);
            _repository.SaveSectorBest("monza", 2, 22.3, "ferrari_488_gt3", conditions);

            double sector1 = _repository.GetAllTimeBest("monza", 1, "ferrari_488_gt3");
            double sector2 = _repository.GetAllTimeBest("monza", 2, "ferrari_488_gt3");

            Assert.AreEqual(15.5, sector1, 0.001);
            Assert.AreEqual(22.3, sector2, 0.001);
        }

        [TestMethod]
        public void SaveSectorBest_SeparatesByTrack()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };

            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);
            _repository.SaveSectorBest("spa", 1, 28.7, "ferrari_488_gt3", conditions);

            double monza = _repository.GetAllTimeBest("monza", 1, "ferrari_488_gt3");
            double spa = _repository.GetAllTimeBest("spa", 1, "ferrari_488_gt3");

            Assert.AreEqual(15.5, monza, 0.001);
            Assert.AreEqual(28.7, spa, 0.001);
        }

        [TestMethod]
        public void LoadAllTimeBestsForTrack_LoadsMultipleSectors()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };

            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);
            _repository.SaveSectorBest("monza", 2, 22.3, "ferrari_488_gt3", conditions);
            _repository.SaveSectorBest("monza", 3, 18.1, "ferrari_488_gt3", conditions);

            double[] times = new double[61]; // index 1..60
            _repository.LoadAllTimeBestsForTrack("monza", "ferrari_488_gt3", times);

            Assert.AreEqual(15.5, times[1], 0.001);
            Assert.AreEqual(22.3, times[2], 0.001);
            Assert.AreEqual(18.1, times[3], 0.001);
            Assert.AreEqual(-1.0, times[4]); // Unset sector
        }

        [TestMethod]
        public void LoadAllTimeBestsForTrack_InitializesToNegativeOne()
        {
            double[] times = new double[61];
            // Pre-fill with different values
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = 999.0;
            }

            _repository.LoadAllTimeBestsForTrack("nonexistent_track", "ferrari_488_gt3", times);

            // All should be reset to -1
            for (int i = 0; i < times.Length; i++)
            {
                Assert.AreEqual(-1.0, times[i]);
            }
        }

        [TestMethod]
        public void LoadAllTimeBestsForTrack_ReturnsEmptyForEmptyTrackId()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };
            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);

            double[] times = new double[61];
            _repository.LoadAllTimeBestsForTrack("", "ferrari_488_gt3", times);

            Assert.AreEqual(-1.0, times[1]);
        }

        [TestMethod]
        public void LoadAllTimeBestsForTrack_ReturnsEmptyForEmptyCarModel()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };
            _repository.SaveSectorBest("monza", 1, 15.5, "ferrari_488_gt3", conditions);

            double[] times = new double[61];
            _repository.LoadAllTimeBestsForTrack("monza", "", times);

            Assert.AreEqual(-1.0, times[1]);
        }

        [TestMethod]
        public void SaveSectorBest_DoesNotSaveForEmptyTrackId()
        {
            var conditions = new TrackConditions { CarModel = "ferrari_488_gt3" };
            _repository.SaveSectorBest("", 1, 15.5, "ferrari_488_gt3", conditions);

            double result = _repository.GetAllTimeBest("", 1, "ferrari_488_gt3");
            Assert.AreEqual(-1.0, result);
        }

        [TestMethod]
        public void SaveSectorBest_DoesNotSaveForEmptyCarModel()
        {
            var conditions = new TrackConditions { CarModel = "" };
            _repository.SaveSectorBest("monza", 1, 15.5, "", conditions);

            double result = _repository.GetAllTimeBest("monza", 1, "");
            Assert.AreEqual(-1.0, result);
        }
    }
}
