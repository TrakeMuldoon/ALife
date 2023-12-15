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
        /// Set to true to automatically start this scenario without showing the launcher
        /// NOTE: If multiple have this set to true, an exception will be thrown
        /// </summary>
        public readonly bool AutoStartScenario;

        /// <summary>
        /// The automatic start seed, used only if AutoStartScenario is true.
        /// </summary>
        public readonly Nullable<int> AutoStartSeed;

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
        /// <param name="autoStartScenario">Set to true to automatically start this scenario without showing the launcher. NOTE: If multiple have this set to true, an exception will be thrown.</param>
        /// <param name="autoStartSeed">The automatic start seed, used only if AutoStartScenario is true.</param>
        public ScenarioRegistration(string name, string description, bool debugModeOnly = false, bool autoStartScenario = false, int autoStartSeed = int.MinValue)
        {
            Name = name;
            Description = description;
            DebugModeOnly = debugModeOnly;
            AutoStartScenario = autoStartScenario;
            Nullable<int> actualSeed = autoStartSeed;
            if(AutoStartScenario && autoStartSeed == int.MinValue)
            {
                actualSeed = null;
            }
            AutoStartSeed = actualSeed;
        }
    }
}
