using ALifeUni.ALife;
using ALifeUni.ALife.Scenarios;
using ALifeUni.ALife.WorldObjects.Agents;
using ALifeUni.ALife.WorldObjects.Agents.CustomAgents;
using System;
using System.Linq;

namespace ALifeUni.ScenarioRunners.ScenarioRunnerConfigs.Configs
{
    /// <summary>
    /// The default configuration defined for the RabbitScenario
    /// </summary>
    /// <seealso cref="ALifeUni.NewScenarioRunners.ScenarioRunnerConfigs.AbstractScenarionRunnerConfig"/>
    [ScenarioRunnerConfigRegistration(typeof(RabbitScenario))]
    public class DefaultRabbitScenarioRunnerConfig : AbstractScenarionRunnerConfig
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
            if (population == 1)
            {
                WriteMessage($"|> Only the Rabbit Remains!{Environment.NewLine}");
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

            var r = Planet.World.AllActiveObjects.OfType<Rabbit>().First();

            if (count > 0 && r.Statistics["Caught"].Value > 0)
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

            var r = Planet.World.AllActiveObjects.OfType<Rabbit>().First();
            WriteMessage($"Pop: {population} (including rabbit) | Caught: {r.Statistics["Caught"].Value}{Environment.NewLine}");
        }
    }
}
