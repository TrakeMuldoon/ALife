using ALifeUni.ALife;
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
    public sealed partial class CollisionTestPanel : UserControl
    {
        public CollisionTestPanel()
        {
            this.InitializeComponent();
            this.Visibility = Visibility.Collapsed;
        }

        private void KillAll_Click(object sender, RoutedEventArgs e)
        {
            foreach(WorldObject wo in Planet.World.AllActiveObjects)
            {
                if(wo is Agent ag)
                {
                    ag.Die();
                }
                else
                {
                    Planet.World.RemoveWorldObject(wo);
                }

            }    
        }

        private void CreateTestDummies_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
