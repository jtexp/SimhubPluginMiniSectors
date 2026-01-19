using Microsoft.VisualStudio.TestTools.UnitTesting;
using User.PluginMiniSectors;

namespace User.PluginMiniSectors.Tests
{
    [TestClass]
    public class SessionBestTests
    {
        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsTrueWhenNoCurrentBest()
        {
            // -1 indicates no current best
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 30.5,
                currentBest: -1.0,
                isLapValid: true);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsTrueWhenFasterThanBest()
        {
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 28.0,
                currentBest: 30.0,
                isLapValid: true);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsFalseWhenSlowerThanBest()
        {
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 32.0,
                currentBest: 30.0,
                isLapValid: true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsFalseWhenEqualToBest()
        {
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 30.0,
                currentBest: 30.0,
                isLapValid: true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsFalseWhenLapInvalid()
        {
            // Even if faster, invalid lap should not update best
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 25.0,
                currentBest: 30.0,
                isLapValid: false);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsFalseWhenLapInvalidAndNoBest()
        {
            // Invalid lap should not set first best either
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 30.0,
                currentBest: -1.0,
                isLapValid: false);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsFalseWhenSectorTimeZero()
        {
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 0.0,
                currentBest: -1.0,
                isLapValid: true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_ReturnsFalseWhenSectorTimeNegative()
        {
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: -5.0,
                currentBest: -1.0,
                isLapValid: true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldUpdateSessionBest_HandlesSmallImprovements()
        {
            // Even tiny improvements should count
            bool result = SectorTimingEngine.ShouldUpdateSessionBest(
                sectorTime: 29.999,
                currentBest: 30.0,
                isLapValid: true);

            Assert.IsTrue(result);
        }
    }
}
