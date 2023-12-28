using ALife.Core.Utility.Random;
using System;

namespace ALife.Core
{
    public class Simulation
    {
        public Simulation()
        {
        }

        public Simulation(uint id, string scenarioName, Nullable<int> startingSeed, Nullable<int> width = null, Nullable<int> height = null)
        {
            Id = id;
            ScenarioName = scenarioName;
            StartingSeed = startingSeed ?? new Random().Next();

            SimulationWidth = width ?? 0;
            SimulationHeight = height ?? 0;

            Random = new FastRandom(StartingSeed);
        }

        public uint Id { get; private set; }

        public FastRandom Random { get; private set; }

        public string ScenarioName { get; private set; }

        public int SimulationHeight { get; private set; }

        public int SimulationWidth { get; private set; }

        public int StartingSeed { get; private set; }
    }
}
