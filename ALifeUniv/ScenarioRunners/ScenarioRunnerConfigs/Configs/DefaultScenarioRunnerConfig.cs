using System;
using System.Linq;
using ALifeUni.ALife;
using ALifeUni.ALife.WorldObjects.Agents;

namespace ALifeUni.ScenarioRunners.ScenarioRunnerConfigs.Configs
{
    /// <summary>
    /// The default scenario runner config for all scenarios if no other config is available.
    /// </summary>
    /// <seealso cref="ALifeUni.NewScenarioRunners.ScenarioRunnerConfigs.AbstractScenarionRunnerConfig"/>
    public class DefaultScenarioRunnerConfig : AbstractScenarionRunnerConfig
    {
        public override bool ShouldEndSimulation(Action<string> WriteMessage)
        {
            var population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            if (population == 0)
            {
                WriteMessage($"|> All Dead{Environment.NewLine}");
                return true;
            }
            return false;
        }

        public override void SimulationSuccessInformation(Action<string> WriteMessage)
        {
            var count = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            WriteMessage($"\tSurviving: {count}{Environment.NewLine}");
        }

        public override void UpdateStatusDetails(Action<string> WriteMessage)
        {
            var population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            WriteMessage($"Pop: {population}{Environment.NewLine}");
        }
    }
}
