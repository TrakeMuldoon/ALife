using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core
{
    /// <summary>
    /// A base class for all ALife objects.
    /// </summary>
    [DebuggerDisplay("Object for #{_simulation}")]
    public abstract class BaseObject
    {
        /// <summary>
        /// The simulation this object is attached to.
        /// </summary>
        [JsonIgnore]
        private Simulation _simulation;

        /// <summary>
        /// The simulation identifier
        /// </summary>
        private uint _simulationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class.
        /// </summary>
        /// <param name="sim">The sim.</param>
        public BaseObject(Simulation sim)
        {
            _simulation = sim;
            _simulationId = sim.Id;
        }

        /// <summary>
        /// Gets the simulation.
        /// </summary>
        /// <value>The simulation.</value>
        public Simulation Simulation
        {
            get
            {
#if NET8_0_OR_GREATER
                _simulation ??= SimulationManager.Manager.GetSimulationForId(_simulationId);
#else
                if (_simulation == null)
                {
                    _simulation = SimulationManager.Manager.GetSimulationForId(_simulationId);
                }
#endif
                return _simulation;
            }
        }
    }
}
