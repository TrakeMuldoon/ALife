using ALife.Core;

namespace ALife.Rendering
{
    /// <summary>
    /// A Rendered Simulation Controller
    /// TODO: Right now this is pretty threadbare. It should be better populated over time. The goal would be to make the actual WorldCanvas control as light-weight as possible to allow us to move to other rendering engines if/as needed.
    /// </summary>
    /// <seealso cref="ALife.Core.SimulationController"/>
    public class RenderedSimulationController : SimulationController
    {
        /// <summary>
        /// The agent UI settings
        /// </summary>
        public readonly AgentUISettings agentUiSettings = new();

        /// <summary>
        /// The UI settings
        /// </summary>
        public readonly LayerUISettings uiSettings = new("Physical", true);

        /// <summary>
        /// Renders the specified renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public void Render(AbstractRenderer renderer)
        {
            for(int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                RenderLogic.DrawWorldObject(Planet.World.AllActiveObjects[i], uiSettings, agentUiSettings, renderer);
            }
        }
    }
}
