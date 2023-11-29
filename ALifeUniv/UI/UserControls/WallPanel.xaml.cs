using ALifeUni.ALife;
using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.ComponentModel;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni.UI.UserControls
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
                if(theWall != null)
                {
                    updateInfo();
                }
            }
        }

        public void updateInfo()
        {
            if(theWall == null)
            {
                return;
            }
            WallName.Text = theWall.IndividualLabel;
            WallXPos.Text = theWall.Shape.CentrePoint.X.ToString();
            WallYPos.Text = theWall.Shape.CentrePoint.Y.ToString();
            WallLength.Text = theWall.RShape.FBLength.ToString();
            WallOrientation.Text = theWall.Shape.Orientation.Degrees.ToString();
            UpdateDeclaration();
            Errors.Text = String.Empty;
        }

        private void ClearInfo()
        {
            TheWall = null;
            WallName.Text = "";
            WallXPos.Text = "";
            WallYPos.Text = "";
            WallLength.Text = "";
            WallOrientation.Text = "";
            Errors.Text = String.Empty;
        }

        private void UpdateDeclaration()
        {
            String newdec = String.Format("walls.Add(new Wall(new Point({0}, {1}), {2}, new Angle({3}), ));"
                                            , WallXPos.Text
                                            , WallYPos.Text
                                            , WallLength.Text
                                            , WallOrientation.Text);
            NewDeclaration.Text = newdec;
        }


        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            string newdec = NewDeclaration.Text;
            DataPackage dp = new DataPackage();
            dp.SetText(newdec);
            Clipboard.SetContent(dp);
        }

        public WallPanel()
        {
            this.InitializeComponent();
        }

        private void WallXPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(theWall == null) return;

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
            if(theWall == null) return;

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
            if(theWall == null) return;

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
            if(theWall == null) return;

            TextBox tb = (TextBox)sender;
            Rectangle rec = theWall.RShape;
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
            UpdateDeclaration();
        }

        private int customCreated = 0;
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Point centre = new Point(Double.Parse(WallXPos.Text), Double.Parse(WallYPos.Text));
                int Length = Int32.Parse(WallLength.Text);
                Angle angle = new Angle(Int32.Parse(WallOrientation.Text));
                Wall w = new Wall(centre, Length, angle, $"Custom_{++customCreated}");
                Planet.World.AddObjectToWorld(w);
                TheWall = w;
            }
            catch(Exception ex)
            {
                Errors.Text = ex.ToString();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearInfo();
        }
    }
}
