using ALifeUni.ALife;
using ALifeUni.ALife.Shapes;
using Microsoft.UI.Xaml.Controls;
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
    public sealed partial class ShapeChanger : UserControl
    {
        public ShapeChanger()
        {
            this.InitializeComponent();
        }

        private ICollisionMap<WorldObject> collider;
        private IShape myShape;
        private WorldObject shapeOwner;
        public WorldObject ShapeOwner
        {
            private get
            {
                return shapeOwner;
            }
            set
            {
                shapeOwner = value;
                myShape = shapeOwner.Shape;
                collider = Planet.World.CollisionLevels[shapeOwner.CollisionLevel];
                UpdateValues();
            }
        }

        bool inUpdate = false;
        private void UpdateValues()
        {
            inUpdate = true;
            XVal.Value = myShape.CentrePoint.X;
            YVal.Value = myShape.CentrePoint.Y;
            Orientation.Value = myShape.Orientation.Degrees;
            inUpdate = false;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Coord_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;
            Point newPoint = new Point(XVal.Value, YVal.Value);
            myShape.CentrePoint = newPoint;
            collider.MoveObject(ShapeOwner);
            myShape.Reset();
            inUpdate = false;
        }

        private void Orientation_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;

            Orientation.Value = Orientation.Value % 360 + (Orientation.Value < 0 ? 360 : 0);
            myShape.Orientation.Degrees = Orientation.Value;

            collider.MoveObject(ShapeOwner);
            myShape.Reset();
            inUpdate = false;
        }

        private void CirRadius_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;

            Circle cc = myShape as Circle;
            cc.Radius = (float) CirRadius.Value;

            collider.MoveObject(ShapeOwner);
            inUpdate = false;
        }

        private void SecRadius_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;
            
            Sector sec = myShape as Sector;
            sec.Radius = (float)SecRadius.Value;

            collider.MoveObject(ShapeOwner);
            myShape.Reset();
            inUpdate = false;
        }

        private void SecSweep_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;

            SecRadius.Value = SecRadius.Value % 360 + (SecRadius.Value < 0 ? 360 : 0);

            Sector sec = myShape as Sector;
            sec.SweepAngle.Degrees = SecRadius.Value;

            collider.MoveObject(ShapeOwner);
            myShape.Reset();
            inUpdate = false;
        }

        private void RecFBLength_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;

            Rectangle rec = myShape as Rectangle;
            rec.FBLength = RecFBLength.Value;

            collider.MoveObject(ShapeOwner);
            myShape.Reset();
            inUpdate = false;
        }

        private void RecRLWidth_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;

            Rectangle rec = myShape as Rectangle;
            rec.RLWidth = RecRLWidth.Value;

            collider.MoveObject(ShapeOwner);
            myShape.Reset();
            inUpdate = false;
        }
    }
}
