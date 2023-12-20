using System;
using ALife.Core;
using ALife.Core.Scenarios;
using ALife.Core.WorldObjects;
using ALife.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace ALife.Avalonia.Controls
{
    public class WorldCanvas : Control
    {
        public static DirectProperty<WorldCanvas, string> ScenarioNameProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, string>(nameof(ScenarioName), x => x.ScenarioName, (x, y) => x.ScenarioName = y);
        public static DirectProperty<WorldCanvas, int?> StartingSeedProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, int?>(nameof(StartingSeed), x => x.StartingSeed, (x, y) => x.StartingSeed = y);

        public static DirectProperty<WorldCanvas, int> TurnCountProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, int>(nameof(TurnCount), x => x.TurnCount);

        public static DirectProperty<WorldCanvas, bool> EnabledProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, bool>(nameof(Enabled), x => x.Enabled, (x, y) => x.Enabled = y);

        private AvaloniaRenderer renderer;

        private string _scenarioName;
        private int? _startingSeed;
        private int movement = 0;

        static WorldCanvas()
        {
            AffectsRender<WorldCanvas>(TurnCountProperty);
        }

        public WorldCanvas()
        {
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1 / 60.0)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private bool _configured = false;
        private bool _enabled = false;
        public bool Enabled
        {
            get { return _enabled; }
            set { SetAndRaise(EnabledProperty, ref _enabled, value); }
        }

        public string ScenarioName
        {
            get { return _scenarioName; }
            set { SetAndRaise(ScenarioNameProperty, ref _scenarioName, value); }
        }

        public int? StartingSeed
        {
            get { return _startingSeed; }
            set { SetAndRaise(StartingSeedProperty, ref _startingSeed, value); }
        }

        private int _turnCount;
        public int TurnCount
        {
            get { return _turnCount; }
            set { SetAndRaise(TurnCountProperty, ref _turnCount, value); }
        }

        public override void Render(DrawingContext drawingContext)
        {
            if(IsEnabled && _configured)
            {
                renderer.SetContext(drawingContext);

                LayerUISettings uiSettings = new("Physical", true);
                AgentUISettings agentUISettings = new()
                {
                    ShowSenses = true,
                    ShowSenseBoundingBoxes = true
                };

                foreach(WorldObject worldObject in Planet.World.AllActiveObjects)
                {
                    RenderLogic.DrawWorldObject(worldObject, uiSettings, agentUISettings, renderer);
                }

                int objects = Planet.World.AllActiveObjects.Count;
                Point p1 = new(objects, objects);
                Point p2 = new(p1.X + 50, p1.Y + 100);

                Pen pen = new(Brushes.Green, 1, lineCap: PenLineCap.Square);
                Pen boundPen = new(Brushes.Black);
                drawingContext.DrawLine(pen, p1, p2);
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

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if(IsEnabled)
            {
                if (!_configured)
                {
                    IScenario scenario = ScenarioRegister.GetScenario(ScenarioName);
                    if(StartingSeed != null)
                    {
                        Planet.CreateWorld(StartingSeed.Value, scenario);
                    }
                    else
                    {
                        Planet.CreateWorld(scenario);
                    }

                    renderer = new AvaloniaRenderer();


                    PointerPressed += WorldCanvas_PointerPressed;
                    Tapped += WorldCanvas_Tapped;
                    _configured = true;
                }
                else
                {
                    Planet.World.ExecuteOneTurn();
                    TurnCount++;
                }
            }
        }

        private void WorldCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            movement += 10;
        }

        private void WorldCanvas_Tapped(object? sender, TappedEventArgs e)
        {
            movement -= 15;
        }
    }
}
