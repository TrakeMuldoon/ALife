using System;
using System.Collections.Generic;

namespace ALife.Core
{
    public sealed class SimulationManager
    {
        private static readonly Lazy<SimulationManager> _lazy = new Lazy<SimulationManager>(() => new SimulationManager());
        private uint _nextId = 0;
        private Dictionary<uint, Simulation> _simulations;

        // Technically, we should have the next id be smart and not just increment, but this is fine for now.

        private SimulationManager()
        {
            _simulations = new Dictionary<uint, Simulation>();
        }

        public static SimulationManager Manager => _lazy.Value;

        public Simulation CreateSimulation(string scenarioName, Nullable<int> startingSeed, Nullable<int> width = null, Nullable<int> height = null)
        {
            Simulation sim = new Simulation(_nextId, scenarioName, startingSeed, width, height);
            _simulations.Add(_nextId, sim);
            ++_nextId;

            return sim;
        }

        public Simulation GetSimulationForId(uint id)
        {
            if(!_simulations.TryGetValue(id, out Simulation sim))
            {
                throw new ArgumentException($"No simulation found for id {id}");
            }

            return sim;
        }

        public Simulation GetSimulationForIdUnsafe(uint id)
        {
            return _simulations[id];
        }
    }
}
