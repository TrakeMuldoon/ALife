﻿using ALife.Avalonia.ALifeImplementations;
using ALife.Avalonia.ViewModels;
using ALife.Core;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALPoint = ALife.Core.Geometry.Shapes.Point;
using Point = Avalonia.Point;

namespace ALife.Avalonia.Controls.SingularRunnerControls
{
    /// <summary>
    /// The Avalonia control for rendering the simulation.
    /// </summary>
    /// <seealso cref="Controls.Control"/>
    public class WorldCanvas : Control
    {
        /// <summary>
        /// The enabled property
        /// </summary>
        public static DirectProperty<WorldCanvas, bool> EnabledProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, bool>(nameof(Enabled), x => x.Enabled, (x, y) => x.Enabled = y);

        /// <summary>
        /// The scenario name property
        /// </summary>
        public static DirectProperty<WorldCanvas, string> ScenarioNameProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, string>(nameof(ScenarioName), x => x.ScenarioName, (x, y) => x.ScenarioName = y);

        /// <summary>
        /// The starting seed property
        /// </summary>
        public static DirectProperty<WorldCanvas, int?> StartingSeedProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, int?>(nameof(StartingSeed), x => x.StartingSeed, (x, y) => x.StartingSeed = y);

        /// <summary>
        /// The turn count property
        /// </summary>
        public static DirectProperty<WorldCanvas, int> TurnCountProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, int>(nameof(TurnCount), x => x.TurnCount);

        /// <summary>
        /// The timer
        /// </summary>
        public DispatcherTimer Timer;

        /// <summary>
        /// The enabled
        /// </summary>
        private bool _enabled = false;

        /// <summary>
        /// The renderer
        /// </summary>
        private AvaloniaRenderer _renderer;

        /// <summary>
        /// The simulation
        /// </summary>
        private RenderedSimulationController _simulation;

        /// <summary>
        /// The turn count
        /// </summary>
        private int _turnCount;

        /// <summary>
        /// The view model
        /// </summary>
        private SingularRunnerViewModel _vm;

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
            _enabled = false;
            _simulation = new();
            SetSimulationSpeed((int)SimulationSpeed.Normal);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WorldCanvas"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return _enabled; }
            set { SetAndRaise(EnabledProperty, ref _enabled, value); }
        }

        /// <summary>
        /// Gets or sets the name of the scenario.
        /// </summary>
        /// <value>The name of the scenario.</value>
        public string ScenarioName
        {
            get { return _simulation.ScenarioName; }
            set { SetAndRaise(ScenarioNameProperty, ref _simulation.ScenarioName, value); }
        }

        /// <summary>
        /// Gets the simulation.
        /// </summary>
        /// <value>The simulation.</value>
        public RenderedSimulationController Simulation => _simulation;

        /// <summary>
        /// The simulation speed
        /// </summary>
        public SimulationSpeed SimulationSpeed { get; private set; }

        /// <summary>
        /// Gets or sets the starting seed.
        /// </summary>
        /// <value>The starting seed.</value>
        public int? StartingSeed
        {
            get { return _simulation.StartingSeed; }
            set { SetAndRaise(StartingSeedProperty, ref _simulation.StartingSeed, value); }
        }

        /// <summary>
        /// Gets or sets the turn count.
        /// </summary>
        /// <value>The turn count.</value>
        public int TurnCount
        {
            get { return _turnCount; }
            set { SetAndRaise(TurnCountProperty, ref _turnCount, value); }
        }

        /// <summary>
        /// Executes the tick.
        /// </summary>
        /// <returns></returns>
        public void ExecuteTick()
        {
            Planet.World.ExecuteOneTurn();
            TurnCount = Planet.World.Turns;
        }

        /// <summary>
        /// Renders the specified drawing context.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <returns></returns>
        public override void Render(DrawingContext drawingContext)
        {
            if(!_simulation.IsInitialized)
            {
                base.Render(drawingContext);
                return;
            }
            
            _renderer.SetContext(drawingContext);
            Planet p = Planet.World;
            _renderer.FillAARectangle(new ALPoint(0, 0), new ALPoint(p.WorldWidth, p.WorldHeight), Colour.PapayaWhip);
            _simulation.Render(_renderer);
            // TODO: for _whatever_ reason, this updates the FPS item, but _not_ the textblock...
            _vm.FramesPerSecond = _simulation.FpsCounter.AverageFramesPerTicks;

            if(special != null)
            {
                RenderSpecial(drawingContext);
            }
            
            RenderDebug(drawingContext);

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
            if(IsEnabled)
            {
                if(!_simulation.IsInitialized)
                {
                    _vm = (SingularRunnerViewModel)Parent.DataContext;
                    _simulation.InitializeSimulation();
                    _renderer = new AvaloniaRenderer();

                    PointerPressed += WorldCanvas_PointerPressed;
                    Tapped += WorldCanvas_Tapped;
                }
                else
                {
                    ExecuteTick();
                    _vm.TicksPerSecond = Planet.World.SimulationPerformance.AverageFramesPerTicks;
                    _vm.TurnCount = TurnCount;
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

        private int movement = 0;
        private void RenderDebug(DrawingContext drawingContext)
        {
            RenderDebugIcon(drawingContext);
            RenderDebugPoints(drawingContext);
        }

        private void RenderDebugIcon(DrawingContext drawingContext)
        {
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

        List<Point> points = new List<Point>();
        private void RenderDebugPoints(DrawingContext drawingContext)
        {
            Pen pen = new(Brushes.Red, 1, lineCap: PenLineCap.Square);
            foreach(Point point in points) 
            {
                drawingContext.DrawEllipse(Brushes.DarkKhaki, pen, point, 4, 4);
            }
        }

        private void RenderSpecial(DrawingContext drawingContext)
        {
            Pen pen = new Pen(Brushes.HotPink);
            if(special is Agent)
            {
                Agent ag = (Agent)special;
                Circle cir = ag.Shape as Circle; //This is the only supported agent shape temporarily
                Point centre = new Point(cir.CentrePoint.X, cir.CentrePoint.Y);
                    
                drawingContext.DrawEllipse(null, pen, centre, cir.Radius + 8, cir.Radius + 3);
            }
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

            Point tapPoint = e.GetPosition(this);
            BoundingBox bb = new BoundingBox(tapPoint.X - 5, tapPoint.Y - 5, tapPoint.X + 5, tapPoint.Y + 5);
            List<WorldObject> colls = Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical].QueryForBoundingBoxCollisions(bb);

            MakeSpecial(colls);
        }

        private WorldObject? special;
        private int specialCounter = 0;
        private void MakeSpecial(List<WorldObject> colls)
        {
            if(colls.Count < 1)
            {
                //They clicked in empty space
                return;
            }

            //The counter has rolled too far
            if(!(specialCounter < colls.Count))
            {
                specialCounter = 0;
            }

            //Select whatever object is at that index
            special = colls[specialCounter];

            //move the counter up, so a second click with select the next object in the list
            specialCounter += 1;
        }
    }
}
