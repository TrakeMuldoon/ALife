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


        public MainPage()
        {
            this.InitializeComponent();
            Planet.CreateWorld();
            startticks = DateTime.Now.Ticks;
            animCanvas.ClearColor = Colors.NavajoWhite;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.animCanvas.RemoveFromVisualTree();
            this.animCanvas = null;
        }

        private void animCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {

            foreach (WorldObject wo in Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical])
            {
                //using (CanvasSolidColorBrush brush = new CanvasSolidColorBrush(sender.Device, Colors.Red))
                //{
                //    args.DrawingSession.DrawCircle(wo.CentrePoint, wo.Radius, brush);
                //}
                args.DrawingSession.FillCircle(wo.CentrePoint, wo.Radius, wo.Color);
                
            }
        }


        //String blah = "1000";
        //GaussianBlurEffect blur = new GaussianBlurEffect();
        private void animCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
        }

        private void AnimCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Planet.World.ExecuteOneTurn();
        }
    }
}
