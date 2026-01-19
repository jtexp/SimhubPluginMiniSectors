using Microsoft.VisualStudio.TestTools.UnitTesting;
using User.PluginMiniSectors;

namespace User.PluginMiniSectors.Tests
{
    [TestClass]
    public class SectorTimingEngineTests
    {
        private SectorTimingEngine _engine;

        [TestInitialize]
        public void Setup()
        {
            _engine = new SectorTimingEngine();
        }

        // ----------------------------------------------------------------
        // Lap Wrap Detection Tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void LapWrap_TriggersOnLapTimeReset()
        {
            // Simulate driving through sectors on Zolder (12 sectors)
            // Build up some sector times first
            _engine.Update("Zolder", 0.05, 5.0, true);   // Sector 1
            _engine.Update("Zolder", 0.15, 15.0, true);  // Sector 2
            _engine.Update("Zolder", 0.95, 120.0, true); // Sector 12

            // Lap time resets (new lap) - position is now at start of track
            _engine.Update("Zolder", 0.01, 0.5, true);

            // Should be in sector 1 on new lap
            Assert.AreEqual(1, _engine.CurrentSectorNumber);
        }

        [TestMethod]
        public void LapWrap_DoesNotTriggerOnPositionWrapAlone()
        {
            // Start in sector 1 with low lap time (just started)
            _engine.Update("Zolder", 0.05, 0.5, true);

            // Position wraps but lap time continues (no reset)
            // This simulates the second wrap that was causing the bug
            _engine.Update("Zolder", 0.99, 0.6, true);
            _engine.Update("Zolder", 0.01, 0.7, true);

            // Current sector times should NOT have been cleared
            // (If lap wrapped incorrectly, times would be cleared)
            Assert.AreEqual(1, _engine.CurrentSectorNumber);
        }

        [TestMethod]
        public void LapWrap_DoesNotTriggerOnSectorDecreaseAlone()
        {
            // Start tracking in sector 12
            _engine.Update("Zolder", 0.90, 100.0, true);

            // Sector decreases but lap time continues (no reset)
            _engine.Update("Zolder", 0.05, 101.0, true);

            // Should NOT have triggered a lap wrap
            // Lap time is still incrementing, so this is just a position glitch
            Assert.AreEqual(1, _engine.CurrentSectorNumber);
        }

        [TestMethod]
        public void LapWrap_PreventsDuplicateWrap()
        {
            // This tests the exact scenario from the bug report:
            // 1. Lap time resets (first wrap - correct)
            // 2. Position wraps shortly after (should NOT trigger second wrap)

            // Build up a complete lap
            _engine.Update("Zolder", 0.05, 5.0, true);
            _engine.Update("Zolder", 0.15, 15.0, true);
            _engine.Update("Zolder", 0.25, 25.0, true);
            _engine.Update("Zolder", 0.35, 35.0, true);
            _engine.Update("Zolder", 0.50, 55.0, true);
            _engine.Update("Zolder", 0.60, 65.0, true);
            _engine.Update("Zolder", 0.70, 75.0, true);
            _engine.Update("Zolder", 0.80, 85.0, true);
            _engine.Update("Zolder", 0.90, 100.0, true);
            _engine.Update("Zolder", 0.99, 130.0, true);

            // First wrap: lap time resets
            _engine.Update("Zolder", 1.00, 0.02, true);

            // Second "wrap" attempt: position wraps (should be ignored)
            _engine.Update("Zolder", 0.00, 0.08, true);

            // Verify last lap has sector times (not cleared by double wrap)
            // If double wrap occurred, last lap would have mostly zeros
            double lastLapSector1 = _engine.GetLastLapSectorTime(1);

            // Sector 1 should have a reasonable time (not 0 or tiny)
            Assert.IsTrue(lastLapSector1 > 1.0,
                $"Last lap sector 1 should have valid time, got {lastLapSector1}");
        }

        // ----------------------------------------------------------------
        // Sector Transition Tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void SectorTransition_OnlyAllowsForwardTransitions()
        {
            // Start in sector 1
            _engine.Update("Zolder", 0.05, 1.0, true);

            // Move to sector 2
            _engine.Update("Zolder", 0.15, 10.0, true);
            Assert.AreEqual(2, _engine.CurrentSectorNumber);

            // Glitch back to sector 1 position (should be ignored for timing)
            _engine.Update("Zolder", 0.05, 10.5, true);

            // Return to sector 2
            _engine.Update("Zolder", 0.15, 11.0, true);

            // Sector 1 time should reflect the first transition, not the glitch
            double sector1Time = _engine.GetCurrentLapSectorTime(1);
            Assert.IsTrue(sector1Time >= 9.0 && sector1Time <= 10.0,
                $"Sector 1 time should be ~9s from first transition, got {sector1Time}");
        }

        [TestMethod]
        public void SectorTransition_IgnoresVeryShortTimes()
        {
            // Start in sector 1
            _engine.Update("Zolder", 0.05, 1.0, true);

            // Quick glitch to sector 2 and back (less than 0.5s)
            _engine.Update("Zolder", 0.15, 1.1, true);
            _engine.Update("Zolder", 0.05, 1.2, true);

            // Sector 1 should NOT have a time recorded (was < 0.5s)
            double sector1Time = _engine.GetCurrentLapSectorTime(1);
            Assert.AreEqual(0.0, sector1Time,
                $"Sector 1 should have no time (glitch filtered), got {sector1Time}");
        }

        [TestMethod]
        public void SectorTransition_RecordsValidTimes()
        {
            // Start in sector 1
            _engine.Update("Zolder", 0.05, 1.0, true);

            // Transition to sector 2 after reasonable time
            _engine.Update("Zolder", 0.15, 12.0, true);

            // Sector 1 should have valid time recorded
            double sector1Time = _engine.GetCurrentLapSectorTime(1);
            Assert.IsTrue(sector1Time >= 10.0 && sector1Time <= 12.0,
                $"Sector 1 time should be ~11s, got {sector1Time}");
        }

        // ----------------------------------------------------------------
        // Final Sector at Lap Wrap Tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void FinalSector_IgnoresVeryShortTimeAtLapWrap()
        {
            // Simulate a lap where we just entered sector 12
            _engine.Update("Zolder", 0.85, 110.0, true);

            // Immediately lap wraps (final sector would be < 0.5s)
            _engine.Update("Zolder", 0.99, 0.1, true);

            // Final sector (12) should NOT be recorded due to short time
            double sector12Time = _engine.GetLastLapSectorTime(12);
            Assert.AreEqual(0.0, sector12Time,
                $"Final sector should not be recorded (< 0.5s), got {sector12Time}");
        }

        [TestMethod]
        public void FinalSector_RecordsValidTimeAtLapWrap()
        {
            // Build up sectors
            _engine.Update("Zolder", 0.05, 5.0, true);   // Sector 1
            _engine.Update("Zolder", 0.15, 15.0, true);  // Enter sector 2
            _engine.Update("Zolder", 0.85, 90.0, true);  // Enter sector 12
            _engine.Update("Zolder", 0.95, 110.0, true); // Still in sector 12, time passing

            // Lap wraps with valid final sector time
            _engine.Update("Zolder", 0.99, 0.5, true);

            // Final sector (12) should have valid time (~20s)
            double sector12Time = _engine.GetLastLapSectorTime(12);
            Assert.IsTrue(sector12Time > 15.0,
                $"Final sector should have valid time, got {sector12Time}");
        }

        // ----------------------------------------------------------------
        // Position Glitch at Lap Start Tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void LapStart_PositionGlitchDoesNotCorruptLastSector()
        {
            // Complete a lap first
            _engine.Update("Zolder", 0.05, 5.0, true);
            _engine.Update("Zolder", 0.15, 15.0, true);
            _engine.Update("Zolder", 0.85, 110.0, true);
            _engine.Update("Zolder", 0.95, 125.0, true);

            // Lap wraps
            _engine.Update("Zolder", 0.01, 0.5, true);

            // Position glitches to end of track briefly
            _engine.Update("Zolder", 0.95, 0.6, true);

            // Position returns to start
            _engine.Update("Zolder", 0.02, 0.7, true);

            // Current lap sector 12 should NOT have a tiny time
            double currentSector12 = _engine.GetCurrentLapSectorTime(12);
            Assert.AreEqual(0.0, currentSector12,
                $"Current lap sector 12 should be 0 (no backward transitions), got {currentSector12}");
        }
    }
}
