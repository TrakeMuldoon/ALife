using ALifeUni.ALife;
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
            Planet.CreateWorld();
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
            
            //for(int i = 0; i < Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical])

            foreach (WorldObject wo in Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical].EnumerateItems())
            {
                //Agent Body
                args.DrawingSession.FillCircle(new Vector2(wo.CentrePoint.X, wo.CentrePoint.Y), wo.Radius, wo.Color);
                //Agent Orientation
                if (wo is Agent)
                {
                    Agent ag = (Agent)wo;
                    float newX = (float)(wo.CentrePoint.X + wo.Radius * Math.Cos(ag.OrientationInRads));
                    float newY = (float)(wo.CentrePoint.Y + wo.Radius * Math.Sin(ag.OrientationInRads));
                    args.DrawingSession.FillCircle(new Vector2(newX, newY), 1, Colors.DarkCyan);
                }

            }
        }


        //String blah = "1000";
        //GaussianBlurEffect blur = new GaussianBlurEffect();
        private void animCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
        }

        private void AnimCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {

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
            dt.Interval = new TimeSpan(5000);
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
            dt.Interval = new TimeSpan(100);
            if (!dt.IsEnabled)
            {
                dt.Start();
            }
        }


        private void FastPlaySim_Click(object sender, RoutedEventArgs e)
        {
            dt.Interval = new TimeSpan(10);
            if (!dt.IsEnabled)
            {
                dt.Start();
            }
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
    }
}
