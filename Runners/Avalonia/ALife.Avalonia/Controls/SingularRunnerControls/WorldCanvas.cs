using ALife.Avalonia.ALifeImplementations;
using ALife.Avalonia.ViewModels;
using ALife.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

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
        public static DirectProperty<WorldCanvas, int> TurnCountProperty = AvaloniaProperty.RegisterDirect<WorldCanvas, int>(nameof(TurnCount), x => x.TurnCount);

        /// <summary>
        /// The timer
        /// </summary>
        public DispatcherTimer Timer;

        /// <summary>
        /// The renderer
        /// </summary>
        private AvaloniaRenderer _renderer;

        /// <summary>
        /// The turn count
        /// </summary>
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
        }

        /// <summary>
        /// Gets the simulation.
        /// </summary>
        /// <value>The simulation.</value>
        public RenderedSimulationController Simulation => ViewModel.Simulation;

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
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public SingularRunnerViewModel ViewModel => Parent.DataContext as SingularRunnerViewModel;

        /// <summary>
        /// Executes the tick.
        /// </summary>
        /// <returns></returns>
        public void ExecuteTick(object? sender, RenderedSimulationTickEventArgs args)
        {
            TurnCount = args.Turn;
        }

        /// <summary>
        /// Renders the specified drawing context.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <returns></returns>
        public override void Render(DrawingContext drawingContext)
        {
            if(ViewModel != null)
            {
                if(Simulation.IsInitialized)
                {
                    _renderer.SetContext(drawingContext);
                    Simulation.Render(_renderer);

                    // TODO: for _whatever_ reason, this updates the FPS item, but _not_ the textblock...
                    _vm.FramesPerSecond = ViewModel.Simulation.FpsCounter.AverageFramesPerTicks;

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
