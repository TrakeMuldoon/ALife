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
        public void Performance_00001Agents() => RunPerformanceTest(1, 50000);

        [TestMethod]
        public void Performance_00010Agents() => RunPerformanceTest(10, 10000);

        [TestMethod]
        public void Performance_00025Agents() => RunPerformanceTest(25, 4000);

        [TestMethod]
        public void Performance_00050Agents() => RunPerformanceTest(50, 2000);

        [TestMethod]
        public void Performance_00100Agents() => RunPerformanceTest(100, 1000);

        [TestMethod]
        public void Performance_00250Agents() => RunPerformanceTest(250, 350);

        [TestMethod]
        public void Performance_00500Agents() => RunPerformanceTest(500, 125);

        [TestMethod]
        [Ignore]
        public void Performance_01000Agents() => RunPerformanceTest(1000, 50);

        [TestMethod]
        [Ignore]
        public void Performance_02500Agents() => RunPerformanceTest(2500, 35);

        [TestMethod]
        [Ignore]
        public void Performance_05000Agents() => RunPerformanceTest(5000, 15);

        [TestMethod]
        [Ignore]
        public void Performance_10000Agents() => RunPerformanceTest(10000, 100);

        [TestMethod]
        public void Performance_Consolidated()
        {
            // Agent Count, Minimum Ticks per Second
            List<(int, int)> scenarios = new()
            {
                (1, 50000),
                (10, 10000),
                (25, 4000),
                (50, 2000), 
                (100, 1000),
                (250, 350), 
                (500, 125),
                (1000, 50),
                (2500, 35), 
                (5000, 15), 
                (10000, 5)
            };

            List<(int, int, double, double, bool)> results = new();
            StringBuilder resultsText = new();
            TestContext.WriteLine("Executing scenarios...");
            foreach((int agentCount, int minimumTps) in scenarios)
            {
                TestContext.WriteLine($"  Executing scenario for {agentCount} agents...");
                (double elapsedSeconds, double tps) = RunPerformanceTest(agentCount, minimumTps, false, false);
                bool scenarioPassed = tps >= minimumTps;
                results.Add((agentCount, minimumTps, elapsedSeconds, tps, scenarioPassed));
                string scenarioResultText = scenarioPassed ? "Passed" : "Failed";
                resultsText.AppendLine($"  Scenario: Result={scenarioResultText} Agents={agentCount,-5} Elapsed={elapsedSeconds:F3,-7}s TPS={tps:F2,-8} MinTPS={minimumTps,-6}");
            }
            
            TestContext.WriteLine($"Scenario Results:\n{resultsText}");

            Assert.IsTrue(results.All(result => result.Item5), "A scenario failed to meet the minimum TPS expected!");
        }
    }
}
