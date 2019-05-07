using ALifeUni.ALife;
using ALifeUni.ALife.UtilityClasses;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ALifeUni
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        long startticks;
        DispatcherTimer dt = new DispatcherTimer();


        public MainPage()
        {
            this.InitializeComponent();
            Planet.CreateWorld((int)animCanvas.Height, (int)animCanvas.Width);
            startticks = DateTime.Now.Ticks;
            animCanvas.ClearColor = Colors.NavajoWhite;
            dt.Tick += Dt_Tick;

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
                Vector2 agentCentre = new Vector2((float)wo.CentrePoint.X, (float)wo.CentrePoint.Y);
                //Agent Body
                args.DrawingSession.FillCircle(agentCentre, wo.Radius, wo.Color);
                //Agent Orientation
                if (wo is Agent)
                {
                    Agent ag = (Agent)wo;
                    float newX = (float)(wo.CentrePoint.X + wo.Radius * Math.Cos(ag.Orientation.Radians));
                    float newY = (float)(wo.CentrePoint.Y + wo.Radius * Math.Sin(ag.Orientation.Radians));
                    args.DrawingSession.FillCircle(new Vector2(newX, newY), 1, Colors.DarkCyan);
                }

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

            if(colls.Count > 0)
            {
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
            //taps.Add(tapPoint);
        }

        private void PauseSim_Click(object sender, RoutedEventArgs e)
        {
            if (dt.IsEnabled)
            {
                dt.Stop();
            }
        }

        private void SlowPlaySim_Click(object sender, RoutedEventArgs e)
        {
            dt.Interval = new TimeSpan(0,0,0,0,500);
            if (!dt.IsEnabled)
            {
                dt.Start();
            }
        }

        private void PlaySim_Click(object sender, RoutedEventArgs e)
        {
            PlaySim_Go();
        }

        private void PlaySim_Go()
        {
            dt.Interval = new TimeSpan(0,0,0,0,100);
            if (!dt.IsEnabled)
            {
                dt.Start();
            }
        }

        private void FastPlaySim_Click(object sender, RoutedEventArgs e)
        {
            dt.Interval = new TimeSpan(0,0,0,0,1);
            if (!dt.IsEnabled)
            {
                dt.Start();
            }
        }
        private void SkipAhead_Click(object sender, RoutedEventArgs e)
        {
            Planet.World.ExecuteManyTurns(200);
        }


        private void ZoomFactor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Zoomer.ChangeView(10, 10, 10000);
            TextBox sen = (TextBox)sender;
            float newZoom;
            if (float.TryParse(sen.Text, out newZoom))
            {
                Zoomer.ChangeView(0, 0, newZoom);
            }
        }

        private void MatchDPI_Checked(object sender, RoutedEventArgs e)
        {
            float newZoom;
            if (float.TryParse(ZoomFactor.Text, out newZoom))
            {
                animCanvas.DpiScale = newZoom;
            }
        }

        private void MatchDPI_Unchecked(object sender, RoutedEventArgs e)
        {
            animCanvas.DpiScale = 1;
        }

        Point? dragStart;
        private void AnimCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            dragStart = e.GetCurrentPoint(animCanvas).Position;
        }

        private void AnimCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            
            int panMagnifyFactor = 10;
            float newZoom;
            if (float.TryParse(ZoomFactor.Text, out newZoom))
            {
                panMagnifyFactor = (int)(8 * newZoom);
            }

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
            if (dragStart.HasValue)
            {
                //Point end = e.GetCurrentPoint(animCanvas).Position;
                //Point delta = new Point(end.X - dragStart.Value.X, end.Y - dragStart.Value.Y);
                //Zoomer.ChangeView(delta.X, delta.Y, null);
                dragStart = null;
            }
        }


    }
}
