using ALife.Core;
using ALife.Core.WorldObjects;

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
        /// The FPS counter
        /// </summary>
        private PerformanceCounter _fpsCounter = new();

        /// <summary>
        /// Gets the FPS counter.
        /// </summary>
        /// <value>
        /// The FPS counter.
        /// </value>
        public PerformanceCounter FpsCounter => _fpsCounter;

        /// <summary>
        /// The agent UI settings
        /// </summary>
        public AgentUISettings AgentUiSettings = new();

        /// <summary>
        /// The drawing errors
        /// </summary>
        public int DrawingErrors = 0;

        /// <summary>
        /// The UI settings
        /// </summary>
        public List<LayerUISettings> Layers = LayerUISettings.GetDefaultSettings();

        /// <summary>
        /// Renders the specified renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public void Render(AbstractRenderer renderer)
        {
            _fpsCounter.Update();
            IEnumerable<LayerUISettings> renderableLayers = Layers.Where(x => x.ShowObjects);
            foreach(LayerUISettings ui in renderableLayers)
            {
                RenderLayer(renderer, ui, AgentUiSettings);
            }
        }

        public void RenderLayer(AbstractRenderer renderer, LayerUISettings ui, AgentUISettings aui)
        {
            try
            {
                //Special Layer, Zones draw different
                if(ui.LayerName == ReferenceValues.CollisionLevelZone)
                {
                    foreach(Zone z in Planet.World.Zones.Values)
                    {
                        RenderLogic.DrawZone(z, renderer);
                    }
                    return;
                }
                //Special Layer, DeadLayer draws different
                if(ui.LayerName == ReferenceValues.CollisionLevelDead)
                {
                    for(int i = 0; i < Planet.World.InactiveObjects.Count; i++)
                    {
                        WorldObject obj = Planet.World.InactiveObjects[i];
                        RenderLogic.DrawInactiveObject(obj, ui, renderer);
                    }
                    return;
                }

                if(!Planet.World.CollisionLevels.ContainsKey(ui.LayerName))        //Layer doesn't exist
                {
                    return;
                }

                /* TODO restore this code
                 *
                if(special != null && viewPast)
                {
                    //This is for when the 'x' key is depressed, it means we view what the selected agent saw when it's
                    //turn happened.
                    //Because each agent executes in order, any agents BEFORE the execution order of the selected agent, are in the correct
                    //spots, but any agents with a HIGHER execution order will have been moved.
                    //So they must be drawn in their previous location.
                    RenderLogic.DrawPastState(ui, aui, renderer, special.ExecutionOrder);
                }
                else
                {
                    //Default Draw Normal case
                    foreach(WorldObject wo in Planet.World.CollisionLevels[ui.LayerName].EnumerateItems())
                    {
                        RenderLogic.DrawWorldObject(wo, ui, aui, renderer);
                    }
                }
                 */

                //Default Draw Normal case
                foreach(WorldObject wo in Planet.World.CollisionLevels[ui.LayerName].EnumerateItems())
                {
                    RenderLogic.DrawWorldObject(wo, ui, aui, renderer);
                }
            }
            catch(Exception)
            {
                DrawingErrors++;
            }
        }
    }
}
