using ALife.Core;
using ALife.Core.Scenarios;
using ALife.Core.Scenarios.FieldCrossings;
using ALife.Core.Scenarios.GardenScenario;
using ALife.Core.WorldObjects;
using ALife.Helpers;
using ALife.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;


namespace AvaloniaTest.Views
{
    internal class WorldCanvas : Control
    {
        static WorldCanvas()
        {
            AffectsRender<WorldCanvas>(TurnCountProperty);
        }

        private AvaloniaRenderer renderer;

        public WorldCanvas()
        {
            IScenario scenario = new FieldCrossingWallsScenario();
            Planet.CreateWorld(scenario);
            renderer = new AvaloniaRenderer();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1 / 60.0);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Planet.World.ExecuteOneTurn();
            TurnCount++;
        }

        public static readonly StyledProperty<int> TurnCountProperty =
            AvaloniaProperty.Register<WorldCanvas, int>(nameof(TurnCount));

        public int TurnCount
        {
            get => GetValue(TurnCountProperty);
            set => SetValue(TurnCountProperty, value);
        }

        int movement = 0;
        public override void Render(DrawingContext drawingContext)
        {
            renderer.SetContext(drawingContext);

            LayerUISettings uiSettings = new LayerUISettings("Physical", true);
            AgentUISettings agentUISettings = new AgentUISettings();

            foreach(WorldObject worldObject in Planet.World.AllActiveObjects)
            {
                RenderLogic.DrawWorldObject(worldObject, uiSettings, agentUISettings, renderer); 
            }

            int objects = Planet.World.AllActiveObjects.Count;
            Point p1 = new Point(objects, objects);
            Point p2 = new Point(p1.X + 50, p1.Y + 100);
            
            Pen pen = new Pen(Brushes.Green, 1, lineCap: PenLineCap.Square);
            Pen boundPen = new Pen(Brushes.Black);
            drawingContext.DrawLine(pen, p1, p2);
            Point shapePont = new Point(150 + movement, 150 + movement);
            
            Rect r = new Rect(shapePont.X, shapePont.Y, 12, 20);
            drawingContext.DrawRectangle(boundPen, r);
            drawingContext.DrawEllipse(Brushes.Aqua, pen, shapePont, 5, 5);

            movement += 1;
            if(movement == 300)
            {
                movement = 0;
            }

            base.Render(drawingContext);
        }
    }
}
