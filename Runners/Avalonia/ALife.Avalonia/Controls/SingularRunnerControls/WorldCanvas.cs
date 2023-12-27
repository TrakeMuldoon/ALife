using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALife.Avalonia.ALifeImplementations;
using ALife.Avalonia.ViewModels;
using ALife.Core;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ALPoint = ALife.Core.Geometry.Shapes.Point;

namespace ALife.Avalonia.Controls.SingularRunnerControls
{
    /// <summary>
    /// The Avalonia control for rendering the simulation.
    /// </summary>
    /// <seealso cref="Controls.Control"/>
    public class WorldCanvas : Control
    {
        /// <summary>
        /// The turn count property
        /// </summary>
        public static DirectProperty<WorldCanvas, RenderedSimulationController> SimulationProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, RenderedSimulationController>(nameof(Simulation), x => x.Simulation, (x, y) => x.Simulation = y);

        public static DirectProperty<WorldCanvas, int> TurnCountProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, int>(nameof(TurnCount), x => x.TurnCount, (x, y) => x.TurnCount = y);

        /// <summary>
        /// The timer
        /// </summary>
        public DispatcherTimer Timer;

        /// <summary>
        /// The renderer
        /// </summary>
        private AvaloniaRenderer _renderer;

        /// <summary>
        /// The simulation
        /// </summary>
        private RenderedSimulationController _simulation;

        private int _turnCount;

        /// <summary>
        /// The view model
        /// </summary>
        private SingularRunnerViewModel _vm;

        /// <summary>
        /// The movement
        /// </summary>
        private int movement = 0;

        /// <summary>
        /// Initializes the <see cref="WorldCanvas"/> class.
        /// </summary>
        static WorldCanvas()
        {
            AffectsRender<WorldCanvas>(TurnCountProperty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldCanvas"/> class.
        /// </summary>
        public WorldCanvas()
        {
            SetSimulationSpeed((int)SimulationSpeed.Normal);
        }

        /// <summary>
        /// Gets or sets the simulation.
        /// </summary>
        /// <value>The simulation.</value>
        public RenderedSimulationController Simulation
        {
            get { return _simulation; }
            set { SetAndRaise(SimulationProperty, ref _simulation, value); }
        }

        /// <summary>
        /// The simulation speed
        /// </summary>
        public SimulationSpeed SimulationSpeed { get; private set; }

        public int TurnCount
        {
            get => _turnCount;
            set => SetAndRaise(TurnCountProperty, ref _turnCount, value);
        }

        /// <summary>
        /// Executes the tick.
        /// </summary>
        /// <returns></returns>
        public void ExecuteTick()
        {
            Planet.World.ExecuteOneTurn();
            //TurnCount = Planet.World.Turns;
        }

        /// <summary>
        /// Renders the specified drawing context.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <returns></returns>
        public override void Render(DrawingContext drawingContext)
        {
            if(_simulation != null && _simulation.IsInitialized)
            {
                _renderer.SetContext(drawingContext);
                Planet p = Planet.World;
                _renderer.FillAARectangle(new ALPoint(0, 0), new ALPoint(p.WorldHeight, p.WorldWidth), System.Drawing.Color.PapayaWhip);
                _simulation.Render(_renderer);
                // TODO: for _whatever_ reason, this updates the FPS item, but _not_ the textblock...
                _vm.FramesPerSecond = _simulation.FpsCounter.AverageFramesPerTicks;

                Pen pen = new(Brushes.Green, 1, lineCap: PenLineCap.Square);
                Pen boundPen = new(Brushes.Black);

                Point shapePont = new(150 + movement, 150 + movement);
                Rect r = new(shapePont.X, shapePont.Y, 12, 20);
                drawingContext.DrawRectangle(boundPen, r);
                drawingContext.DrawEllipse(Brushes.Aqua, pen, shapePont, 5, 5);

                movement += 1;
                if(movement >= 300)
                {
                    movement = 0;
                }
            }
            base.Render(drawingContext);
        }

        /// <summary>
        /// Sets the simulation speed.
        /// </summary>
        /// <param name="speed">The speed.</param>
        public void SetSimulationSpeed(int speed)
        {
            if(Planet.HasWorld)
            {
                Planet.World.SimulationPerformance?.ClearBuffer();
            }
            if(_simulation != null)
            {
                _simulation.FpsCounter?.ClearBuffer();
            }

            if(Timer != null)
            {
                Timer.Stop();
            }
            Timer = new()
            {
                Interval = TimeSpan.FromSeconds(1 / (double)speed)
            };
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        /// <summary>
        /// Timers the tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if(IsEnabled && _simulation != null)
            {
                if(!_simulation.IsInitialized)
                {
                    _vm = Parent.DataContext as SingularRunnerViewModel;
                    _simulation.InitializeSimulation();
                    _renderer = new AvaloniaRenderer();

                    PointerPressed += WorldCanvas_PointerPressed;
                    Tapped += WorldCanvas_Tapped;
                }
                else
                {
                    ExecuteTick();
                    _vm.TicksPerSecond = Planet.World.SimulationPerformance.AverageFramesPerTicks;
                    _vm.TurnCount = Planet.World.Turns;
                    TurnCount = Planet.World.Turns;
                    UpdateZoneInfo();
                    UpdateGeneology();
                }
            }
        }

        /// <summary>
        /// Updates the geneology.
        /// </summary>
        private void UpdateGeneology()
        {
            Dictionary<string, int> geneCount = new Dictionary<string, int>();
            for(int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                WorldObject wo = Planet.World.AllActiveObjects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    string gene = ag.IndividualLabel.Substring(0, 3);
                    if(!geneCount.ContainsKey(gene))
                    {
                        geneCount.Add(gene, 0);
                    }
                    ++geneCount[gene];
                }
            }

            _vm.GenesActive = geneCount.Count;
        }

        /// <summary>
        /// Updates the zone information.
        /// </summary>
        private void UpdateZoneInfo()
        {
            Dictionary<string, int> zoneCount = new Dictionary<string, int>();
            foreach(Zone z in Planet.World.Zones.Values)
            {
                zoneCount.Add(z.Name, 0);
            }
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                WorldObject wo = Planet.World.AllActiveObjects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    zoneCount[ag.HomeZone.Name]++;
                }
            }

            int maxNameLength = zoneCount.Keys.Max(k => k.Length);
            int maxZoneCount = zoneCount.Values.Max();

            foreach(string name in zoneCount.Keys)
            {
                string nameSpaces = new string(' ', maxNameLength - name.Length);

                sb.AppendLine($"{nameSpaces}{name}: {zoneCount[name]}");
            }
            _vm.ZoneInfo = sb.ToString();
            _vm.AgentsActive = Planet.World.AllActiveObjects.Where(wo => wo.Alive && wo is Agent).Count();
        }

        /// <summary>
        /// Worlds the canvas pointer pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        /// The <see cref="Avalonia.Input.PointerPressedEventArgs"/> instance containing the event data.
        /// </param>
        /// <returns></returns>
        private void WorldCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            movement += 10;
        }

        /// <summary>
        /// Worlds the canvas tapped.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Avalonia.Input.TappedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void WorldCanvas_Tapped(object? sender, TappedEventArgs e)
        {
            movement -= 15;
        }
    }
}
