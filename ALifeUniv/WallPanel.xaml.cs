using ALifeUni.ALife.Objects;
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
            if(inUpdate)
                return;
            throw new NotImplementedException();
        }

        private void WallYPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(inUpdate)
                return;
            throw new NotImplementedException();
        }

        private void WallOrientation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(inUpdate)
                return;
            throw new NotImplementedException();
        }
    }
}
