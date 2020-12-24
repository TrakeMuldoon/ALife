using ALifeUni.ALife;
using ALifeUni.ALife.Objects;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni
{
    public sealed partial class WallPanel : UserControl
    {
        private Wall theWall;
        public Wall TheWall
        {
            get => theWall;
            set
            {
                theWall = value;
                clearInfo();
                if(theWall != null)
                {
                    updateInfo();
                }
            }
        }

        bool inUpdate = false;
        public void updateInfo()
        {
            if(theWall == null)
            {
                return;
            }
            inUpdate = true;
            WallXPos.Text = theWall.Shape.CentrePoint.X.ToString();
            WallYPos.Text = theWall.Shape.CentrePoint.Y.ToString();
            WallLength.Text = (theWall.Shape as Rectangle).FBLength.ToString();
            WallOrientation.Text = theWall.Shape.Orientation.Degrees.ToString();
            inUpdate = false;
        }

        private void clearInfo()
        {
            inUpdate = true;
            WallXPos.Text = "";
            WallYPos.Text = "";
            WallOrientation.Text = "";
            inUpdate = false;
        }

        public WallPanel()
        {
            this.InitializeComponent();
        }

        private void WallXPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(inUpdate) return;

            TextBox tb = (TextBox)sender;
            if(theWall.Shape.CentrePoint.X.ToString() == tb.Text) return;
            if(String.IsNullOrEmpty(tb.Text)) return;

            double result;
            if(double.TryParse(tb.Text, out result))
            {
                theWall.Shape.CentrePoint = new Point(result, theWall.Shape.CentrePoint.Y);
                FinishChange();
            }
        }

        private void WallYPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(inUpdate) return;
            
            TextBox tb = (TextBox)sender;
            if(theWall.Shape.CentrePoint.Y.ToString() == tb.Text) return;
            if(String.IsNullOrEmpty(tb.Text)) return;

            double result;
            if(double.TryParse(tb.Text, out result))
            {
                theWall.Shape.CentrePoint = new Point(theWall.Shape.CentrePoint.X, result);
                FinishChange();
            }
        }

        private void WallOrientation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(inUpdate) return;

            TextBox tb = (TextBox)sender;
            if(theWall.Shape.Orientation.Degrees.ToString() == tb.Text) return;


            if(String.IsNullOrEmpty(tb.Text)) return;

            double result;
            if(double.TryParse(tb.Text, out result))
            {
                theWall.Shape.Orientation.Degrees = result;
                FinishChange();
            }
        }

        private void WallLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(inUpdate) return;
            
            TextBox tb = (TextBox)sender;
            Rectangle rec = (Rectangle)theWall.Shape;
            if(rec.FBLength.ToString() == tb.Text) return;
            if(String.IsNullOrEmpty(tb.Text)) return;

            double result;
            if(double.TryParse(tb.Text, out result))
            {
                rec.FBLength = result;
                FinishChange();
            }
        }

        private void FinishChange()
        {
            theWall.Shape.Reset();
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[theWall.CollisionLevel];
            collider.MoveObject(theWall);
        }
    }
}
