using System;
using ALife.Core;
using ALife.Core.Scenarios;
using ALife.Core.Scenarios.FieldCrossings;
using ALife.Core.WorldObjects;
using ALife.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace ALife.Avalonia
{
    internal class WorldCanvas : Control
    {
        public static readonly StyledProperty<int> TurnCountProperty =
            AvaloniaProperty.Register<WorldCanvas, int>(nameof(TurnCount));

        private readonly AvaloniaRenderer renderer;

        private int movement = 0;

        static WorldCanvas()
        {
            AffectsRender<WorldCanvas>(TurnCountProperty);
        }


        //TODO: This horrific work around makes me PHYSICALLY SICK.
        public static String CanvasScenarioName;
        public static int? StartingSeed;

        public WorldCanvas()
        {

            IScenario scenario = ScenarioRegister.GetScenario(CanvasScenarioName);
            if(StartingSeed != null)
            {
                Planet.CreateWorld(StartingSeed.Value, scenario);
            }
            else
            {
                Planet.CreateWorld(scenario);
            }

            renderer = new AvaloniaRenderer();

            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1 / 60.0)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            PointerPressed += WorldCanvas_PointerPressed;
            Tapped += WorldCanvas_Tapped;
        }

        private void WorldCanvas_Tapped(object? sender, TappedEventArgs e)
        {
            movement -= 15;
        }

        private void WorldCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            movement += 10;
        }

        public int TurnCount
        {
            get => GetValue(TurnCountProperty);
            set => SetValue(TurnCountProperty, value);
        }

        public override void Render(DrawingContext drawingContext)
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

            base.Render(drawingContext);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Planet.World.ExecuteOneTurn();
            TurnCount++;
        }
    }
}
