using Microsoft.VisualStudio.TestTools.UnitTesting;
using User.PluginMiniSectors;

namespace User.PluginMiniSectors.Tests
{
    [TestClass]
    public class TrackDataTests
    {
        [TestMethod]
        public void RangeLabel_Contains_ReturnsTrueWhenValueInRange()
        {
            var range = new RangeLabel(0.1, 0.2, "Test Corner");

            Assert.IsTrue(range.Contains(0.15));
            Assert.IsTrue(range.Contains(0.1));  // inclusive start
            Assert.IsTrue(range.Contains(0.2));  // inclusive end
        }

        [TestMethod]
        public void RangeLabel_Contains_ReturnsFalseWhenValueOutOfRange()
        {
            var range = new RangeLabel(0.1, 0.2, "Test Corner");

            Assert.IsFalse(range.Contains(0.05));
            Assert.IsFalse(range.Contains(0.25));
            Assert.IsFalse(range.Contains(0.0));
            Assert.IsFalse(range.Contains(1.0));
        }

        [TestMethod]
        public void TrackTurnMap_ContainsSpa()
        {
            Assert.IsTrue(TrackData.TrackTurnMap.ContainsKey("Spa"));
        }

        [TestMethod]
        public void TrackTurnMap_SpaHasLaSource()
        {
            var spaCorners = TrackData.TrackTurnMap["Spa"];

            Assert.IsNotNull(spaCorners);
            Assert.IsTrue(spaCorners.Length > 0);
            Assert.AreEqual("La Source", spaCorners[0].Label);
        }

        [TestMethod]
        public void TrackTurnMap_IsCaseInsensitive()
        {
            Assert.IsTrue(TrackData.TrackTurnMap.ContainsKey("spa"));
            Assert.IsTrue(TrackData.TrackTurnMap.ContainsKey("SPA"));
            Assert.IsTrue(TrackData.TrackTurnMap.ContainsKey("Spa"));
        }

        [TestMethod]
        public void TrackTurnMap_ContainsExpectedTrackCount()
        {
            // We have 22 tracks mapped
            Assert.AreEqual(22, TrackData.TrackTurnMap.Count);
        }

        [TestMethod]
        public void TrackTurnMap_NurburgringNordschleifeHasManyCorners()
        {
            var nordschleife = TrackData.TrackTurnMap["nurburgring_24h"];

            // Nordschleife has 43+ corners
            Assert.IsTrue(nordschleife.Length >= 40);
        }
    }
}
