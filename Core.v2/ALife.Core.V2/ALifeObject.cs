using System.Diagnostics;

namespace ALife.Core
{
    /// <summary>
    /// A base class for all ALife objects.
    /// </summary>
    [DebuggerDisplay("{_simulation}")]
    public abstract class ALifeObject
    {
        /// <summary>
        /// The simulation this object is attached to.
        /// </summary>
        private Simulation _simulation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ALifeObject"/> class.
        /// </summary>
        /// <param name="sim">The sim.</param>
        public ALifeObject(Simulation sim)
        {
            _simulation = sim;
        }

        /// <summary>
        /// Gets the simulation.
        /// </summary>
        /// <value>The simulation.</value>
        public Simulation Simulation { get => _simulation; }
    }
}
