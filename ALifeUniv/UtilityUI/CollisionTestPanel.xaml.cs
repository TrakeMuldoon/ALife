using ALifeUni.ALife;
using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni
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
            Point p1 = new Point(50, 50);
            Circle c1 = new Circle(p1, 10);
            c1.Color = Colors.Green;

            EmptyObject eo = new EmptyObject(c1, ReferenceValues.CollisionLevelPhysical);
            Planet.World.AddObjectToWorld(eo);
            GreenShape.ShapeOwner = eo;

            Point p2 = new Point(100, 100);
            Rectangle r1 = new Rectangle(p2, 20, 10, Colors.Red);
            r1.Orientation.Degrees = 45;

            EmptyObject e2 = new EmptyObject(r1, ReferenceValues.CollisionLevelPhysical);
            Planet.World.AddObjectToWorld(e2);
            RedShape.ShapeOwner = e2;
        }
    }
}
