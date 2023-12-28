using ALife.Core;
using ALife.Core.Collision;
using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni.UI.UserControls
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

        public int XValue { get { return (int)XVal.Value; } }
        public int YValue { get { return (int)YVal.Value; } }
        public string ShapeString { get { return ShapeChooser.SelectedValue.ToString(); } }
        public double OrientationVal { get { return Orientation.Value; } }
        public double CircleRadius { get { return CirRadius.Value; } }
        public double SectorRadius { get { return SecRadius.Value; } }
        public double SectorSweep { get { return SecSweep.Value; } }
        public double RectangleFB { get { return RecFBLength.Value; } }
        public double RectangleRL { get { return RecRLWidth.Value; } }


        bool inUpdate = false;
        private void UpdateValues()
        {
            inUpdate = true;
            XVal.Value = myShape.CentrePoint.X;
            YVal.Value = myShape.CentrePoint.Y;
            Orientation.Value = myShape.Orientation.Degrees;
            CircleStats.Visibility = Visibility.Collapsed;
            SectorStats.Visibility = Visibility.Collapsed;
            RectangleStats.Visibility = Visibility.Collapsed;
            switch(myShape)
            {
                case Circle cc:
                    CircleStats.Visibility = Visibility.Visible;
                    ShapeChooser.SelectedValue = "Circle";
                    CirRadius.Value = cc.Radius;
                    break;
                case Sector sec:
                    SectorStats.Visibility = Visibility.Visible;
                    ShapeChooser.SelectedValue = "Sector";
                    SecRadius.Value = sec.Radius;
                    SecSweep.Value = sec.SweepAngle.Degrees;
                    break;
                case Rectangle rec:
                    RectangleStats.Visibility = Visibility.Visible;
                    ShapeChooser.SelectedValue = "Rectangle";
                    RecFBLength.Value = rec.FBLength;
                    RecRLWidth.Value = rec.RLWidth;
                    break;
                default:
                    throw new NotImplementedException("Shape not known. What you do?");
            }
            inUpdate = false;
        }

        private void ShapeChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(inUpdate) { return; }

            if(!(ShapeOwner is EmptyObject))
            {
                throw new NotImplementedException("Cannot change shape of anything but 'EmptyObject'. If you don't know what that means, then something is wrong.");
            }

            double keyValue;
            switch(myShape)
            {
                case Circle cc: keyValue = cc.Radius; break;
                case Rectangle rec: keyValue = rec.FBLength; break;
                case Sector sec: keyValue = sec.Radius; break;
                default: throw new NotImplementedException("What Shape? Why?");
            }
            IShape newShape = null;
            switch(ShapeChooser.SelectedValue)
            {
                case "Circle": newShape = new Circle(myShape.CentrePoint, (float)keyValue); break;
                case "Sector": newShape = new Sector(myShape.CentrePoint, (float)keyValue, new Angle(20), myShape.Color); break;
                case "Rectangle": newShape = new Rectangle(myShape.CentrePoint, keyValue, keyValue + 5, myShape.Color); break;
            }
            newShape.Color = myShape.Color;
            EmptyObject eoOwner = ShapeOwner as EmptyObject;
            eoOwner.SetShape(newShape);
            myShape = shapeOwner.Shape;
            myShape.Reset();
            UpdateValues();
        }


        private void Coord_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(inUpdate) { return; }
            inUpdate = true;
            Windows.Foundation.Point newPoint = new Windows.Foundation.Point(XVal.Value, YVal.Value);
            myShape.CentrePoint = newPoint.ToALifePoint();
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
            cc.Radius = (float)CirRadius.Value;

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

            SecSweep.Value = SecSweep.Value % 360 + (SecSweep.Value < 0 ? 360 : 0);

            Sector sec = myShape as Sector;
            sec.SweepAngle.Degrees = SecSweep.Value;

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
