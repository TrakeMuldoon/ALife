using System;

namespace ALife.Core.Scenarios
{
    /// <summary>
    /// An attribute indicating that a class is a registered scenario
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScenarioRegistration : Attribute
    {
        /// <summary>
        /// The auto start mode. Only one scenario can be registered with either AutoStartVisual or AutoStartConsole
        /// </summary>
        public readonly AutoStartMode AutoStartScenario;

        /// <summary>
        /// The automatic start seed, used only if AutoStartScenario is true.
        /// </summary>
        public readonly int? AutoStartSeed;

        /// <summary>
        /// Set to true to only list this scenario in debug mode
        /// </summary>
        public readonly bool DebugModeOnly;

        /// <summary>
        /// The description of the scenario
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The name of the scenario
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioRegistration"/> class.
        /// </summary>
        /// <param name="name">The name of the scenario.</param>
        /// <param name="description">The description of the scenario.</param>
        /// <param name="debugModeOnly">True to only list this scenario in debug mode.</param>
        /// <param name="autoStartScenario">The auto start mode. Only one scenario can be registered with either AutoStartVisual or AutoStartConsole.</param>
        /// <param name="autoStartSeed">The automatic start seed, used only if AutoStartScenario is true.</param>
        public ScenarioRegistration(string name, string description, bool debugModeOnly = false, AutoStartMode autoStartScenario = AutoStartMode.None, int autoStartSeed = int.MinValue)
        {
            Name = name;
            Description = description;
            DebugModeOnly = debugModeOnly;
            AutoStartScenario = autoStartScenario;
            Nullable<int> actualSeed = autoStartSeed;
            if(AutoStartScenario == AutoStartMode.None && autoStartSeed == int.MinValue)
            {
                actualSeed = null;
            }
            AutoStartSeed = actualSeed;
        }
    }
}
