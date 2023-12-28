using ALife.Core.Scenarios;
using System;
using System.Numerics;

namespace ALife.Core.V2
{
    public class SimulationController
    {
        public string ScenarioName { get; private set; }

        public int SimulationHeight { get; private set; }

        public int SimulationWidth { get; private set; }

        public int StartingSeed { get; private set; }

        public SimulationController()
        {

        }

        public SimulationController(string scenarioName, Nullable<int> startingSeed, Nullable<int> width = null, Nullable<int> height = null)
        {
            ScenarioName = scenarioName;
            StartingSeed = startingSeed ?? new Random().Next();

            SimulationWidth = width ?? 0;
            SimulationHeight = height ?? 0;
        }
    }
}
