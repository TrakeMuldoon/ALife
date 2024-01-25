using System;

namespace ALifeUni.ScenarioRunners.ScenarioRunnerConfigs
{
    /// <summary>
    /// A configuration for a scenario when using the scenario runner
    /// </summary>
    public abstract class AbstractScenarionRunnerConfig
    {
        public ScenarioState ScenarioState { get; set; } = ScenarioState.InProgress;

        /// <summary>
        /// This function will be called at the end of every batch. Use the Planet.World instance to determine if the
        /// simulation should end. Use WriteMessage (No automatic newline) to write a message if desired, when the
        /// simulation should end.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        /// <returns>A bool indicating whether or not the simulation should end</returns>
        public bool ShouldEndSimulation(Action<string> WriteMessage)
        {
            var result = ShouldEndSimulationInternal(WriteMessage);
            if(result)
            {
                ScenarioState = ScenarioState.Complete;
            }
            return result;
        }

        /// <summary>
        /// This function will be called at the end of every batch. Use the Planet.World instance to determine if the
        /// simulation should end. Use WriteMessage (No automatic newline) to write a message if desired, when the
        /// simulation should end.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        /// <returns>A bool indicating whether or not the simulation should end</returns>
        public abstract bool ShouldEndSimulationInternal(Action<string> WriteMessage);

        /// <summary>
        /// At the end of every successful simulation, a message can be printed with additional interesting or relevant
        /// information. Use Planet.World instance to find the information.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        public void SimulationSuccessInformation(Action<string> WriteMessage)
        {
            SimulationSuccessInformationInternal(WriteMessage);
        }

        /// <summary>
        /// At the end of every successful simulation, a message can be printed with additional interesting or relevant
        /// information. Use Planet.World instance to find the information.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        public abstract void SimulationSuccessInformationInternal(Action<string> WriteMessage);

        /// <summary>
        /// At the end of every set of batched turns (usually 10), a message can be printed with additional interesting
        /// or relevant information. Use Planet.World instance to find the information.
        /// </summary>
        /// <param name="WriteMessage">An Action to write a message</param>
        public abstract void UpdateStatusDetails(Action<string> WriteMessage);
    }
}
