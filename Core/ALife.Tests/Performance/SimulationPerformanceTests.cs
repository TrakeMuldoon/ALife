using ALife.Core;
using ALife.Core.Scenarios.TestScenarios;
using System.Diagnostics;
using System.Text;

namespace ALife.Tests.Performance
{
    [TestClass]
    public class SimulationPerformanceTests
    {
        private const int Seed = 42;
        private const int TickCount = 1000;

        public TestContext TestContext { get; set; } = null!;

        private (double elapsedSeconds, double tps) RunPerformanceTest(int agentCount, int minimumTps, bool printOutputLine = true, bool runTpsAssert = true)
        {
            PerformanceBenchmarkScenario scenario = new PerformanceBenchmarkScenario(agentCount);
            Planet.CreateWorld(Seed, scenario);

            Stopwatch stopwatch = Stopwatch.StartNew();
            Planet.World.ExecuteManyTurns(TickCount);
            stopwatch.Stop();

            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            double tps = TickCount / elapsedSeconds;

            if(printOutputLine)
            {
                TestContext.WriteLine($"Agents={agentCount,-5} Ticks={TickCount} Elapsed={elapsedSeconds:F3}s TPS={tps:F2}");
            }

            if(runTpsAssert)
            {
                Assert.IsTrue(tps >= minimumTps, $"Simulation did not meet the expected minimum TPS ({tps:F3} vs {minimumTps:F3} minimum)");
            }
            Assert.AreEqual(TickCount, Planet.World.Turns, "Simulation did not complete the expected number of ticks");
            Assert.AreEqual(agentCount, Planet.World.AllActiveObjects.Count, "Agent count changed unexpectedly during simulation");

            return (elapsedSeconds, tps);
        }

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00001Agents() => RunPerformanceTest(1, 30000);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00010Agents() => RunPerformanceTest(10, 5000);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00025Agents() => RunPerformanceTest(25, 2000);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00050Agents() => RunPerformanceTest(50, 1000);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00100Agents() => RunPerformanceTest(100, 500);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00250Agents() => RunPerformanceTest(250, 200);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00500Agents() => RunPerformanceTest(500, 100);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_01000Agents() => RunPerformanceTest(1000, 50);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_02500Agents() => RunPerformanceTest(2500, 20);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_05000Agents() => RunPerformanceTest(5000, 9);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_10000Agents() => RunPerformanceTest(10000, 4);

        [TestMethod]
        public void Performance_Consolidated()
        {
            // Agent Count, Minimum Ticks per Second
            List<(int, int)> scenarios = new()
            {
                (1, 30000),
                (10, 5000),
                (25, 2000),
                (50, 1000), 
                (100, 500),
                (250, 200), 
                (500, 100),
                (1000, 50),
                (2500, 20), 
                (5000, 9) 
                //(10000, 4)
            };

            List<(int, int, double, double, bool)> results = new();
            StringBuilder resultsText = new();
            double totalElapsedSeconds = 0;
            foreach((int agentCount, int minimumTps) in scenarios)
            {
                (double elapsedSeconds, double tps) = RunPerformanceTest(agentCount, minimumTps, false, false);
                bool scenarioPassed = tps >= minimumTps;
                totalElapsedSeconds += elapsedSeconds;
                results.Add((agentCount, minimumTps, elapsedSeconds, tps, scenarioPassed));
                string scenarioResultText = scenarioPassed ? "Passed" : "Failed";
                resultsText.AppendLine($"  Scenario: Result={scenarioResultText} Agents={agentCount,-5} MinTPS={minimumTps,-6} ActualTPS={tps:F2} Elapsed={elapsedSeconds:F3}s");
            }
            
            TestContext.WriteLine($"Scenario Results (Total Time: {totalElapsedSeconds:F3}s):\n{resultsText}");

            Assert.IsTrue(results.All(result => result.Item5), "A scenario failed to meet the minimum TPS expected!");
        }


        [TestMethod]
        [TestCategory("Performance")]
        public void RunPerfTest()
        {
            // Agent Count, Minimum Ticks per Second
            List<(int, int)> scenarios = new()
            {
                (1, 30000),
                (10, 5000),
                (25, 2000),
                (50, 1000),
                (100, 500),
                (250, 200),
                (500, 100),
                (1000, 50),
                (2500, 20),
                (5000, 9),
                (10000, 4)
            };

            List<(int, int, double, double, bool)> results = new();
            StringBuilder resultsText = new();
            double totalElapsedSeconds = 0;
            int localTickCount = 800;

            foreach((int agentCount, int minimumTps) in scenarios)
            {
                PerformanceBenchmarkScenario scenario = new PerformanceBenchmarkScenario(agentCount);
                Planet.CreateWorld(Seed, scenario);
                Stopwatch stopwatch = Stopwatch.StartNew();
                Planet.World.ExecuteManyTurns(localTickCount);
                stopwatch.Stop();

                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                double tps = TickCount / elapsedSeconds;

                TestContext.WriteLine($"Agents={agentCount,-5} Ticks={TickCount} Elapsed={elapsedSeconds:F3}s TPS={tps:F2}");
              
                bool scenarioPassed = tps >= minimumTps;
                totalElapsedSeconds += elapsedSeconds;
                results.Add((agentCount, minimumTps, elapsedSeconds, tps, scenarioPassed));
                string scenarioResultText = scenarioPassed ? "Passed" : "Failed";
                resultsText.AppendLine($"  Scenario: Result={scenarioResultText} Agents={agentCount,-5} MinTPS={minimumTps,-6} ActualTPS={tps:F2} Elapsed={elapsedSeconds:F3}s");
            }

            TestContext.WriteLine($"Scenario Results (Total Time: {totalElapsedSeconds:F3}s):\n{resultsText}");
        }
    }
}
