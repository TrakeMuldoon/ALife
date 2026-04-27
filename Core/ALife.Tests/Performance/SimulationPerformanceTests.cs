using ALife.Core;
using ALife.Core.Scenarios.TestScenarios;
using ALife.Core.Utility;
using System.Diagnostics;
using System.Text;

namespace ALife.Tests.Performance
{
    [TestClass]
    public class SimulationPerformanceTests
    {
        private const int Seed = 42;
        private const int TickCount = 1000;

        private static readonly Dictionary<int, int> scenarios = new()
        {
            // Agent Count, Minimum Ticks per Second (DEFAULT)
            { 1, 30000 },
            { 10, 5000 },
            { 25, 2000 },
            { 50, 1000 }, 
            { 100, 500 },
            { 250, 200 }, 
            { 500, 100 },
            { 1000, 50 },
            { 2500, 20 }, 
            { 5000, 9 }, 
            { 10000, 4 },
        };

        private static readonly Dictionary<string, double> cpuTpsMultiplier = new()
        {
            { "Apple M3 Max", 1.6 },
            { "Apple M2", 1.0 },  // TODO: Test this, and a Ryzen
        };
        
        [TestMethod]
        public void Performance_Consolidated()
        {
            bool allPassed = true;
            StringBuilder resultsText = new();
            StringBuilder resultsCsv = new();
            resultsCsv.AppendLine("Passed,Agent Count,Minimum TPS,Actual TPS,Base TPS,Elapsed (s)");
            double totalElapsedSeconds = 0;
            double tpsMultiplier = cpuTpsMultiplier.GetValueOrDefault(SysInfo.Instance.CpuName, 1);
            
            foreach((int agentCount, int baseMinimumTps) in scenarios)
            {
                (double elapsedSeconds, double tps) = RunPerformanceTest(agentCount, false, false);
                int minimumTps = (int)(baseMinimumTps * tpsMultiplier);
                bool scenarioPassed = tps >= minimumTps;
                totalElapsedSeconds += elapsedSeconds;
                string scenarioResultText = scenarioPassed ? "Passed" : "Failed";
                if(!scenarioPassed)
                {
                    allPassed = false;
                }
                resultsCsv.AppendLine($"{scenarioResultText},{agentCount},{minimumTps},{tps:F2},{baseMinimumTps},{elapsedSeconds:F5}");
                resultsText.AppendLine($"  Scenario: Result={scenarioResultText} Agents={agentCount,-5} MinTPS={minimumTps,-6} ActualTPS={tps:F2} BaseTPS={baseMinimumTps,-6} Elapsed={elapsedSeconds:F3}s");
            }
            
            TestContext.WriteLine($"Scenario Results (Total Time: {totalElapsedSeconds:F3}s):\n{resultsText}");
            string timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
            string performanceDirectory = Path.Join(Helpers.GetSolutionRootFromTestContext(TestContext), "PerformanceTests", "Simulation", "LocalResults");
            if(!Directory.Exists(performanceDirectory))
            {
                Directory.CreateDirectory(performanceDirectory);
            }
            
            File.WriteAllText(Path.Join(performanceDirectory, $"performance_results_{timeStamp}.txt"), resultsText.ToString());
            File.WriteAllText(Path.Join(performanceDirectory, $"performance_results_{timeStamp}.csv"), resultsCsv.ToString());
            
            Assert.IsTrue(allPassed, "A scenario failed to meet the minimum TPS expected!");
        }

        public TestContext TestContext { get; set; } = null!;

        private (double elapsedSeconds, double tps) RunPerformanceTest(int agentCount, bool printOutputLine = true, bool runTpsAssert = true)
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
                double tpsMultiplier = cpuTpsMultiplier.GetValueOrDefault(SysInfo.Instance.CpuName, 1);
                int minimumTps = (int)(scenarios[agentCount] * tpsMultiplier);
                Assert.IsTrue(tps >= minimumTps, $"Simulation did not meet the expected minimum TPS ({tps:F3} vs {minimumTps:F3} minimum)");
            }
            Assert.AreEqual(TickCount, Planet.World.Turns, "Simulation did not complete the expected number of ticks");
            Assert.AreEqual(agentCount, Planet.World.AllActiveObjects.Count, "Agent count changed unexpectedly during simulation");

            return (elapsedSeconds, tps);
        }

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00001Agents() => RunPerformanceTest(1);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00010Agents() => RunPerformanceTest(10);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00025Agents() => RunPerformanceTest(25);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00050Agents() => RunPerformanceTest(50);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00100Agents() => RunPerformanceTest(100);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00250Agents() => RunPerformanceTest(250);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_00500Agents() => RunPerformanceTest(500);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_01000Agents() => RunPerformanceTest(1000);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_02500Agents() => RunPerformanceTest(2500);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_05000Agents() => RunPerformanceTest(5000);

        [TestMethod]
        [Ignore("Remove IGNORE attribute to enable for manually testing scenario")]
        public void Performance_10000Agents() => RunPerformanceTest(10000);
    }
}
