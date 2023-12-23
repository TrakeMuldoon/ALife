using System.Timers;
using ALife.Core;
using ALife.Core.WorldObjects;

namespace ALife.Rendering
{
    /// <summary>
    /// A Rendered Simulation Controller
    /// TODO: Right now this is pretty threadbare. It should be better populated over time. The goal would be to make the actual WorldCanvas control as light-weight as possible to allow us to move to other rendering engines if/as needed.
    /// </summary>
    /// <seealso cref="ALife.Core.SimulationController"/>
    public class RenderedSimulationController : SimulationController, IDisposable
    {
        /// <summary>
        /// The agent UI settings
        /// </summary>
        public AgentUISettings AgentUiSettings = new();

        /// <summary>
        /// The drawing errors
        /// </summary>
        public int DrawingErrors = 0;

        /// <summary>
        /// The is simulation enabled
        /// </summary>
        public bool IsSimulationEnabled = false;

        /// <summary>
        /// The UI settings
        /// </summary>
        public List<LayerUISettings> Layers = LayerUISettings.GetDefaultSettings();

        /// <summary>
        /// The FPS counter
        /// </summary>
        private PerformanceCounter _fpsCounter = new();

        /// <summary>
        /// The timer
        /// </summary>
        private System.Timers.Timer? _timer;

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderedSimulationController"/> class.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="startingSeed">The starting seed.</param>
        /// <param name="width">The width. Defaults to the default world width for the scenario.</param>
        /// <param name="height">The height. Defaults to the default world height for the scenario.</param>
        public RenderedSimulationController(string scenarioName, int? startingSeed, int? width = null, int? height = null) : base(scenarioName, startingSeed, width, height)
        {
        }

        /// <summary>
        /// Occurs when [on simulation speed changed event].
        /// </summary>
        public event EventHandler<SimulationSpeedChangedEventArgs> OnSimulationSpeedChangedEvent;

        /// <summary>
        /// Occurs when [on simulation tick event].
        /// </summary>
        public event EventHandler<RenderedSimulationTickEventArgs> OnSimulationTickEvent;

        /// <summary>
        /// Gets the current simulation speed.
        /// </summary>
        /// <value>The current simulation speed.</value>
        public SimulationSpeed CurrentSimulationSpeed { get; private set; }

        /// <summary>
        /// Gets the FPS counter.
        /// </summary>
        /// <value>The FPS counter.</value>
        public PerformanceCounter FpsCounter => _fpsCounter;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Executes the tick.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        public void ExecuteTick(object? source, ElapsedEventArgs e)
        {
            if(IsSimulationEnabled)
            {
            }
            ExecuteTick();

            EventHandler<RenderedSimulationTickEventArgs> raiseEvent = OnSimulationTickEvent;
            if(raiseEvent != null)
            {
                RenderedSimulationTickEventArgs args = new(e.SignalTime, Planet.World.Turns);
                raiseEvent(this, args);
            }
        }

        /// <summary>
        /// Renders the specified renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public void Render(AbstractRenderer renderer)
        {
            _fpsCounter.Update();
            IEnumerable<LayerUISettings> renderableLayers = Layers.Where(x => x.ShowObjects);
            foreach(LayerUISettings uiSettings in renderableLayers)
            {
                RenderLayer(renderer, uiSettings, AgentUiSettings);
            }
        }

        /// <summary>
        /// Renders the layer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="ui">The UI.</param>
        /// <param name="aui">The aui.</param>
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

        /// <summary>
        /// Sets the simulation speed.
        /// </summary>
        /// <param name="speed">The speed.</param>
        public void SetSimulationSpeed(SimulationSpeed speed, bool triggerSpeedChangedEvent = true)
        {
            if(Planet.HasWorld)
            {
                Planet.World.SimulationPerformance?.ClearBuffer();
            }
            _fpsCounter.ClearBuffer();

            if(_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            SimulationSpeed oldSpeed = CurrentSimulationSpeed;
            CurrentSimulationSpeed = speed;
            _timer = new(speed.ToTargetMS());
            _timer.Elapsed += ExecuteTick;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            if(triggerSpeedChangedEvent)
            {
                EventHandler<SimulationSpeedChangedEventArgs> raiseEvent = OnSimulationSpeedChangedEvent;
                if(raiseEvent != null)
                {
                    SimulationSpeedChangedEventArgs args = new(oldSpeed, speed);
                    raiseEvent(this, args);
                }
            }
        }

        /// <summary>
        /// Starts the simulation.
        /// </summary>
        public void StartSimulation()
        {
            SetSimulationSpeed(SimulationSpeedExtensions.DEFAULT_SPEED, false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    _timer?.Stop();
                    _timer?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
    }
}
