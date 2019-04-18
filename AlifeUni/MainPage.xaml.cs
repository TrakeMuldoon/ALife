﻿using ALifeUni.ALife;
using Microsoft.Graphics.Canvas;
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

        //Random rnd = new Random();
        //private Vector2 RndPosition()
        //{
        //    double x = rnd.NextDouble() * 500f;
        //    double y = rnd.NextDouble() * 500f;
        //    return new Vector2((float)x, (float)y);
        //}

        //private float RndRadius()
        //{
        //    return (float)rnd.NextDouble() * 150f;
        //}

        //private byte RndByte()
        //{
        //    return (byte)rnd.Next(256);
        //}

        //long numDraws = 0;
        private void animCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            foreach(WorldObject wo in Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical])
            {
                args.DrawingSession.DrawCircle(wo.CentrePoint, wo.Radius, wo.Color);
            }
            //++numDraws;
            //float radius = (float)(1 + Math.Sin(args.Timing.TotalTime.TotalSeconds)) * 10f;
            //blur.BlurAmount = radius;
            //args.DrawingSession.DrawImage(blur);
            //args.DrawingSession.DrawText(DateTime.Now.Ticks.ToString(), new Vector2(0, 0), Colors.Black);
            //decimal ticksPerDraw = (DateTime.Now.Ticks - startticks) / numDraws;
            //args.DrawingSession.DrawText(numDraws.ToString(), new Vector2(0, 20), Colors.Black);
            //args.DrawingSession.DrawText((DateTime.Now.Ticks - startticks).ToString(), new Vector2(0, 40), Colors.Black);
            //args.DrawingSession.DrawText(ticksPerDraw.ToString(), new Vector2(0, 60), Colors.Black);
        }


        //String blah = "1000";
        //GaussianBlurEffect blur = new GaussianBlurEffect();
        private void animCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            //CanvasCommandList cl = new CanvasCommandList(sender);
            //using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        clds.DrawText(blah, RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
            //        clds.DrawCircle(RndPosition(), RndRadius(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
            //        clds.DrawLine(RndPosition(), RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
            //    }
            //}
            //blur = new GaussianBlurEffect()
            //{
            //    Source = cl,
            //    BlurAmount = 10.0f
            //};
        }

    }
}
