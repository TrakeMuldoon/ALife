using ALifeUni.ALife;
using ALifeUni.ALife.UtilityClasses;
using ALifeUni.UI;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ALifeUni
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        long startticks;
        DispatcherTimer gameTimer = new DispatcherTimer();


        public MainPage()
        {
            this.InitializeComponent();
            Planet.CreateWorld((int)animCanvas.Height, (int)animCanvas.Width);
            startticks = DateTime.Now.Ticks;
            animCanvas.ClearColor = Colors.NavajoWhite;
            gameTimer.Tick += Dt_Tick;

            PlaySim_Go();
        }

        private void Dt_Tick(object sender, object e)
        {
            //Planet.World.ExecuteManyTurns(10);
            Planet.World.ExecuteOneTurn();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.animCanvas.RemoveFromVisualTree();
            this.animCanvas = null;
        }

        private void animCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {

            if (special != null)
            {
                Vector2 agentCentre = new Vector2((float)special.CentrePoint.X, (float)special.CentrePoint.Y);
                //Agent Body
                args.DrawingSession.DrawCircle(agentCentre, special.Radius + 1, Colors.Blue);
            }
            
            foreach (WorldObject wo in Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical].EnumerateItems())
            {
                DrawingLogic.DrawWorldObject(wo, args);
            }

            foreach (Point p in taps)
            {
                args.DrawingSession.FillCircle(new Vector2((float)p.X, (float)p.Y), 2, Colors.Peru);
            }
        }

        private void animCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
        }

        List<Point> taps = new List<Point>();
        WorldObject special;
        private void AnimCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point tapPoint = e.GetPosition(animCanvas);
            BoundingBox bb = new BoundingBox(tapPoint.X, tapPoint.Y, tapPoint.X, tapPoint.Y);
            List<WorldObject> colls = Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical].QueryForBoundingBoxCollisions(bb);

            //Check if we have any collisions for the tap
            if(colls.Count > 0)
            {
                //If we do have a collisions, then set that agent to be "Special"
                WorldObject clicked = colls[0];
                if(clicked != special)
                {
                    special = clicked;
                }
                else
                {
                    //click again to unselect
                    special = null;
                }
            }
        }

        private void PauseSim_Click(object sender, RoutedEventArgs e)
        {
            if (gameTimer.IsEnabled)
            {
                gameTimer.Stop();
            }
        }

        private void OneTurnSim_Click(object sender, RoutedEventArgs e)
        {
            if (!gameTimer.IsEnabled)
            {
                Planet.World.ExecuteOneTurn();
            }
        }


        private void SlowPlaySim_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Interval = new TimeSpan(0,0,0,0,500);
            if (!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }

        private void PlaySim_Click(object sender, RoutedEventArgs e)
        {
            PlaySim_Go();
        }

        private void PlaySim_Go()
        {
            gameTimer.Interval = new TimeSpan(0,0,0,0,100);
            if (!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }

        private void FastPlaySim_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Interval = new TimeSpan(0,0,0,0,1);
            if (!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }
        private void SkipAhead_Click(object sender, RoutedEventArgs e)
        {
            Planet.World.ExecuteManyTurns(200);
        }

        private void ZoomFactor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Increase the zoom
            //TODO: Link this to the Dpi checkbox, I just realized that they're only linked from the checkbox side and not from the textchanged side. I'm dumb
            TextBox sen = (TextBox)sender;
            float newZoom;
            if (float.TryParse(sen.Text, out newZoom))
            {
                Zoomer.ChangeView(0, 0, newZoom);
            }
        }

        private void MatchDPI_Checked(object sender, RoutedEventArgs e)
        {
            //Increase the DPI as the zoom increases, to a maximum of 8, otherwise it breaks
            float newZoom;
            if (float.TryParse(ZoomFactor.Text, out newZoom))
            {
                float DpiValue = Math.Min(newZoom,8);
                DpiValue = Math.Max(DpiValue, 0);
                
                animCanvas.DpiScale = DpiValue;
            }
        }

        private void MatchDPI_Unchecked(object sender, RoutedEventArgs e)
        {
            //When the check gets unchecked, just set the DPI scale to 1, which is shitty when zoomed in.
            animCanvas.DpiScale = 1;
        }

        Point? dragStart;
        private void AnimCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            dragStart = e.GetCurrentPoint(animCanvas).Position;
        }

        private void AnimCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            
            int panMagnifyFactor = 0;
            float newZoom;
            if (float.TryParse(ZoomFactor.Text, out newZoom))
            {
                panMagnifyFactor = (int)(12 * newZoom);
            }

            //Constantly updates the position the pan window should be as you drag along. 
            //Each time this is called constitutes a new drag.
            if(dragStart.HasValue)
            {
                Point current = e.GetCurrentPoint(animCanvas).Position;
                Point moveDelta = new Point((current.X - dragStart.Value.X) * panMagnifyFactor
                                            , (current.Y - dragStart.Value.Y) * panMagnifyFactor);
                Point offset = new Point(Zoomer.HorizontalOffset + moveDelta.X, Zoomer.VerticalOffset + moveDelta.Y);
                
                Zoomer.ChangeView(offset.X, offset.Y, null);
                dragStart = current;
            }
        }

        private void AnimCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //When a drag is released, null it out
            if (dragStart.HasValue)
            {
                dragStart = null;
            }
        }


    }
}
