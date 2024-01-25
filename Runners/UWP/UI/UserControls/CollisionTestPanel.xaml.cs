using ALife.Core;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects;
using System;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni.UI.UserControls
{
    public sealed partial class CollisionTestPanel : UserControl
    {
        public int NumberBoxValue = 12;

        public CollisionTestPanel()
        {
            this.InitializeComponent();
            this.Visibility = Visibility.Collapsed;
        }

        private void KillAll_Click(object sender, RoutedEventArgs e)
        {
            foreach(WorldObject wo in Planet.World.AllActiveObjects)
            {
                wo.Die();
            }
        }

        private void CreateTestDummies_Click(object sender, RoutedEventArgs e)
        {
            Windows.Foundation.Point p1 = new Windows.Foundation.Point(50, 50);
            Circle c1 = new Circle(p1.ToALifePoint(), 10);
            c1.Colour = Colour.Green;

            EmptyObject eo = new EmptyObject(c1, ReferenceValues.CollisionLevelPhysical);
            Planet.World.AddObjectToWorld(eo);
            GreenShape.ShapeOwner = eo;

            Windows.Foundation.Point p2 = new Windows.Foundation.Point(100, 100);
            Rectangle r1 = new Rectangle(p2.ToALifePoint(), 20, 10, Colour.Red);
            r1.Orientation.Degrees = 45;

            EmptyObject e2 = new EmptyObject(r1, ReferenceValues.CollisionLevelPhysical);
            Planet.World.AddObjectToWorld(e2);
            RedShape.ShapeOwner = e2;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            string point1 = String.Format("Point point1 = new Point({0}, {1});", GreenShape.XValue, GreenShape.YValue);
            string shape1 = BuildShapeConstructor(1, GreenShape);
            string ori1 = "shape1.Orientation.Degrees = " + (int)GreenShape.OrientationVal + ";";

            string point2 = String.Format("Point point2 = new Point({0}, {1});", RedShape.XValue, RedShape.YValue);
            string shape2 = BuildShapeConstructor(2, RedShape);
            string ori2 = "shape2.Orientation.Degrees = " + (int)RedShape.OrientationVal + ";";

            sb.AppendLine("ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, \"TestGrid\");");
            sb.AppendLine(point1);
            sb.AppendLine(shape1);
            sb.AppendLine(ori1);
            sb.AppendLine("ShapeWrapper wrap1 = new ShapeWrapper(\"shape1\", shape1);");
            sb.AppendLine();
            sb.AppendLine(point2);
            sb.AppendLine(shape2);
            sb.AppendLine(ori2);
            sb.AppendLine("ShapeWrapper wrap2 = new ShapeWrapper(\"shape2\", shape2);");
            sb.AppendLine();
            sb.AppendLine("if(!collMap.Insert(wrap1)) { Assert.Fail(\"Failed To Insert wrap 1\"); }");
            sb.AppendLine("if(!collMap.Insert(wrap2)) { Assert.Fail(\"Failed To Insert wrap 2\"); }");
            sb.AppendLine();
            sb.AppendLine("List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);");
            sb.AppendLine("List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);");
            sb.AppendLine();

            if(IsCollision.IsChecked.Value)
            {
                sb.AppendLine("VerifyCollisionTarget(collision1, wrap2);");
                sb.AppendLine("VerifyCollisionTarget(collision2, wrap1);");
            }
            else
            {
                sb.AppendLine("VerifyCollisionTarget(collision1);");
                sb.AppendLine("VerifyCollisionTarget(collision2);");
            }

            string newTest = sb.ToString();
            DataPackage dp = new DataPackage();
            dp.SetText(newTest);
            Clipboard.SetContent(dp);
        }

        private string BuildShapeConstructor(int itemNum, ShapeChanger shapeSpec)
        {
            switch(shapeSpec.ShapeString)
            {
                case "Circle":
                    return String.Format("Circle shape{0} = new Circle(point{0}, {1});"
                                                     , itemNum, shapeSpec.CircleRadius);
                case "Rectangle":
                    return String.Format("Rectangle shape{0} = new Rectangle(point{0}, {1}, {2}, Colors.Pink);"
                                                        , itemNum, shapeSpec.RectangleFB, shapeSpec.RectangleRL);
                case "Sector":
                    return String.Format("Sector shape{0} = new Sector(point{0}, {1}, new Angle({2}), Colors.Pink);"
                                                    , itemNum, shapeSpec.SectorRadius, shapeSpec.SectorSweep);
                default: throw new Exception("Invalid shape value in the shape constructor.");
            }
        }
    }
}
