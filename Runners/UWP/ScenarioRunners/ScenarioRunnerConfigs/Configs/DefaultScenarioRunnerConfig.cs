using ALife.Core;
using ALife.Core.WorldObjects.Agents;
using System;
using System.Linq;

namespace ALifeUni.ScenarioRunners.ScenarioRunnerConfigs.Configs
{
    /// <summary>
    /// The default scenario runner config for all scenarios if no other config is available.
    /// </summary>
    /// <seealso cref="ALifeUni.NewScenarioRunners.ScenarioRunnerConfigs.AbstractScenarionRunnerConfig"/>
    public class DefaultScenarioRunnerConfig : AbstractScenarionRunnerConfig
    {
        /// <summary>
        /// This function will be called at the end of every batch. Use the Planet.World instance to determine if the
        /// simulation should end. Use WriteMessage (No automatic newline) to write a message if desired, when the
        /// simulation should end.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        /// <returns>A bool indicating whether or not the simulation should end</returns>
        public override bool ShouldEndSimulationInternal(Action<string> WriteMessage)
        {
            var population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            if(population == 0)
            {
                WriteMessage($"|> All Dead{Environment.NewLine}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// At the end of every successful simulation, a message can be printed with additional interesting or relevant
        /// information. Use Planet.World instance to find the information.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        public override void SimulationSuccessInformationInternal(Action<string> WriteMessage)
        {
            var count = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            WriteMessage($"\tSurviving: {count}{Environment.NewLine}");

            if(count > 0)
            {
                ScenarioState = ScenarioState.CompleteSuccessful;
            }
        }

        /// <summary>
        /// At the end of every set of batched turns (usually 10), a message can be printed with additional interesting
        /// or relevant information. Use Planet.World instance to find the information.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        public override void UpdateStatusDetails(Action<string> WriteMessage)
        {
            var population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            WriteMessage($"Pop: {population}{Environment.NewLine}");
        }
    }
}
