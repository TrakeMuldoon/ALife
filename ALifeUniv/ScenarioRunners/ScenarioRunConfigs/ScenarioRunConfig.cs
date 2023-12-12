using ALifeUni.ALife.WorldObjects.Agents;
using ALifeUni.ALife;
using System;
using System.Linq;
using ALifeUni.ALife.WorldObjects.Agents.CustomAgents;

namespace ALifeUni.ScenarioRunners.ScenarioRunConfigs
{
    public abstract class ScenarioRunConfig
    {
        /// <summary>
        /// This function will be called at the end of every batch. Use the Planet.World instance to determine if the simulation should end.
        /// Use WriteMessage (No automatic newline) to write a message if desired, when the simulation should end.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        /// <returns>A bool indicating whether or not the simulation should end</returns>
        public abstract bool ShouldEndSimulation(Action<string> WriteMessage);

        /// <summary>
        /// At the end of every set of batched turns (usually 10), a message can be printed with additional interesting or relevant information.
        /// Use Planet.World instance to find the information.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        public abstract void UpdateStatusDetails(Action<string> WriteMessage);

        /// <summary>
        /// At the end of every successful simulation, a message can be printed with additional interesting or relevant information.
        /// Use Planet.World instance to find the information.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        public abstract void SimulationSuccessInformation(Action<string> WriteMessage);
    }

    public class DefaultScenarioRunConfig : ScenarioRunConfig
    {
        public override bool ShouldEndSimulation(Action<string> WriteMessage)
        {
            int population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            if(population == 0)
            {
                WriteMessage($"|> All Dead{Environment.NewLine}");
                return true;
            }
            return false;
        }

        public override void UpdateStatusDetails(Action<string> WriteMessage)
        {
            int population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            WriteMessage($"Pop: {population}{Environment.NewLine}");
        }

        public override void SimulationSuccessInformation(Action<string> WriteMessage)
        {
            int count = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            WriteMessage($"\tSurviving: {count}{Environment.NewLine}");
        }
    }

    public class RabbitScenarioConfig : ScenarioRunConfig
    {
        public override bool ShouldEndSimulation(Action<string> WriteMessage)
        {
            int population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            if(population == 1)
            {
                WriteMessage($"|> Only the Rabbit Remains!{Environment.NewLine}");
                return true;
            }
            return false;
        }

        public override void UpdateStatusDetails(Action<string> WriteMessage)
        {
            int population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();

            Rabbit r = Planet.World.AllActiveObjects.OfType<Rabbit>().First();
            WriteMessage($"Pop: {population} (including rabbit) | Caught: {r.Statistics["Caught"].Value}{Environment.NewLine}");
        }

        public override void SimulationSuccessInformation(Action<string> WriteMessage)
        {
            int count = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            WriteMessage($"\tSurviving: {count}{Environment.NewLine}");
        }
    }
}
