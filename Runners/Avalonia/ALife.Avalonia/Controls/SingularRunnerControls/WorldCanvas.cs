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

        /// <summary>
        /// The turn count property
        /// </summary>
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

        /// <summary>
        /// The turn count
        /// </summary>
        private int _turnCount;

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
            _renderer = new AvaloniaRenderer();
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
        /// Gets or sets the turn count.
        /// </summary>
        public int TurnCount
        {
            get => _turnCount;
            set => SetAndRaise(TurnCountProperty, ref _turnCount, value);
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
