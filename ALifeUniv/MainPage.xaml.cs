using ALifeUni.ALife;
using ALifeUni.ALife.UtilityClasses;
using ALifeUni.UI;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel.Calls;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ALifeUni
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        long startticks;
        DispatcherTimer gameTimer = new DispatcherTimer();
        public List<LayerUISettings> UIGrid;

        public MainPage()
        {
            this.InitializeComponent();

            animCanvas.ClearColor = Colors.NavajoWhite;

            Planet.CreateWorld((int)animCanvas.Height, (int)animCanvas.Width);
            seed = Planet.World.Seed.ToString();
            SimSeed.Text = seed;

            UIGrid = LayerUISettings.GetSettings();
            VisualSettingsGrid.ItemsSource = UIGrid;

            startticks = DateTime.Now.Ticks;
            gameTimer.Tick += Dt_Tick;

            PlaySim_Go();
        }

        private void animCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;
            this.animCanvas.RemoveFromVisualTree();
            this.animCanvas = null;
        }

        private void Dt_Tick(object sender, object e)
        {
            Planet.World.ExecuteOneTurn();
            AgentPanel.updateInfo();
        }

        private void animCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {

            if(special != null)
            {
                Vector2 agentCentre = new Vector2((float)special.CentrePoint.X, (float)special.CentrePoint.Y);
                args.DrawingSession.DrawCircle(agentCentre, special.Radius + 1, Colors.Blue);
            }

            foreach(LayerUISettings layer in UIGrid)
            {
                DrawLayer(layer, args);
            }
        }

        private void DrawLayer(LayerUISettings ui, CanvasAnimatedDrawEventArgs args)
        {
            if(ui.LayerName == ReferenceValues.CollisionLevelZone
                && ui.ShowLayer)
            {
                foreach(Zone z in Planet.World.Zones.Values)
                {
                    DrawingLogic.DrawZone(z, args);
                }
                return;
            }

            if(!ui.ShowLayer                                                       //Not showing it
                || !Planet.World.CollisionLevels.ContainsKey(ui.LayerName))        //Layer doesn't exist
            {
                return;
            }

            if(viewPast
                && special != null)
            {
                int compnumber = special.ExecutionOrder;
                foreach(WorldObject wo in Planet.World.CollisionLevels[ui.LayerName].EnumerateItems())
                {
                    if(compnumber < wo.ExecutionOrder)
                    {
                        DrawingLogic.DrawWorldObject(wo, ui, args);
                        continue;
                    }

                    if(!(wo is Agent))
                    {
                        //TODO: Should ALL objects have shadows??
                        DrawingLogic.DrawWorldObject(wo, ui, args);
                        continue;
                    }

                    Agent ag = (Agent)wo;

                    if (compnumber == wo.ExecutionOrder)
                    {
                        DrawingLogic.DrawAgentShadow(ag.Shadow, ui, args);
                        continue;
                    }
                    
                    DrawingLogic.DrawWorldObject(ag.Shadow, ui, args);
                }
            }
            else
            {
                foreach(WorldObject wo in Planet.World.CollisionLevels[ui.LayerName].EnumerateItems())
                {
                    DrawingLogic.DrawWorldObject(wo, ui, args);
                }
            }
        }

        WorldObject special;
        int numCreated;
        private void AnimCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point tapPoint = e.GetPosition(animCanvas);
            BoundingBox bb = new BoundingBox(tapPoint.X, tapPoint.Y, tapPoint.X, tapPoint.Y);
            List<WorldObject> colls = Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical].QueryForBoundingBoxCollisions(bb);

            CoreWindow cw = Window.Current.CoreWindow;
            CoreVirtualKeyStates ks = cw.GetAsyncKeyState(VirtualKey.Control);

            if(ks.HasFlag(CoreVirtualKeyStates.Down))
            {
                if(colls.Count == 0)
                {
                    EmptyObject eo = new EmptyObject(tapPoint, 5, ReferenceValues.CollisionLevelPhysical, (numCreated++).ToString());
                    Planet.World.AddObjectToWorld(eo);
                }
                else
                {
                    Planet.World.RemoveWorldObject(colls[0]);
                }
            }
            else
            {
                if(colls.Count > 0)
                {
                    MakeSpecial(colls);
                }
            }
        }

        private void MakeSpecial(List<WorldObject> colls)
        {

            //If we do have a collisions, then set that agent to be "Special"
            WorldObject clicked = colls[0];
            if(clicked != special)
            {
                special = clicked;
                if(clicked is Agent)
                {
                    AgentPanel.TheAgent = (Agent)clicked;
                }
            }
            else
            {
                //click again to unselect
                special = null;
                AgentPanel.TheAgent = null;
            }
        }

        private String seed = String.Empty;
        private void SimSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if(tb.Text != seed)
            {

                tb.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                tb.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            seed = tb.Text;
        }

        private void ResetSim_Click(object sender, RoutedEventArgs e)
        {
            int seedValue;
            if(int.TryParse(seed, out seedValue))
            {
                Planet.CreateWorld(seedValue, (int)animCanvas.Height, (int)animCanvas.Width);            
            }
            else
            {
                Planet.CreateWorld((int)animCanvas.Height, (int)animCanvas.Width);
            }

            seed = Planet.World.Seed.ToString();
            SimSeed.Text = seed;
            SimSeed_TextChanged(SimSeed, null);
            special = null;
        }

        #region speed controls

        private void PauseSim_Click(object sender, RoutedEventArgs e)
        {
            if(gameTimer.IsEnabled)
            {
                gameTimer.Stop();
            }
        }

        private void OneTurnSim_Click(object sender, RoutedEventArgs e)
        {
            if(gameTimer.IsEnabled)
            {
                gameTimer.Stop();
            }
            Planet.World.ExecuteOneTurn();
            AgentPanel.updateInfo();
        }

        private void SlowPlaySim_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            if(!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }

        private void PlaySim_Click(object sender, RoutedEventArgs e)
        {
            PlaySim_Go();
        }

        private void PlaySim_Go()
        {
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            if(!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }

        private void FastPlaySim_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            if(!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }
        private void SkipAhead_Click(object sender, RoutedEventArgs e)
        {
            Planet.World.ExecuteManyTurns(200);
            AgentPanel.updateInfo();
        }

        private void SkipFarAhead_Click(object sender, RoutedEventArgs e)
        {
            List<LayerUISettings> settingsStorage = new List<LayerUISettings>();
            foreach(LayerUISettings set in UIGrid)
            {
                settingsStorage.Add(set);
            }
            UIGrid.Clear();

            Planet.World.ExecuteManyTurns(5000);
            AgentPanel.updateInfo();

            foreach(LayerUISettings set in settingsStorage)
            {
                UIGrid.Add(set);
            }
        }

        #endregion

        #region Zoom controls
        private void ZoomFactor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Increase the zoom
            TextBox sen = (TextBox)sender;
            float newZoom;
            if(float.TryParse(sen.Text, out newZoom))
            {
                if(newZoom > 8)
                {
                    sen.Text = "8";
                    return;
                }
                if(newZoom < 1)
                {
                    sen.Text = "1";
                    return;
                }

                double newHeight = Zoomer.ActualHeight * newZoom;
                double newVert = Zoomer.VerticalOffset / Zoomer.ExtentHeight * newHeight;
                double newHorz = Zoomer.HorizontalOffset / Zoomer.ExtentWidth * newHeight;

                animCanvas.DpiScale = (int)newZoom;
                Zoomer.ChangeView(newHorz, newVert, newZoom);
            }
        }

        private void IncreaseZoom()
        {
            float newZoom;
            if(float.TryParse(ZoomFactor.Text, out newZoom))
            {
                ZoomFactor.Text = (newZoom + 1).ToString();
            }
        }

        private void DecreaseZoom()
        {
            float newZoom;
            if(float.TryParse(ZoomFactor.Text, out newZoom))
            {
                ZoomFactor.Text = (newZoom - 1).ToString();
            }
        }

        private void MatchDPI_Checked(object sender, RoutedEventArgs e)
        {
            //Increase the DPI as the zoom increases, to a maximum of 8, otherwise it breaks
            float newZoom;
            if(float.TryParse(ZoomFactor.Text, out newZoom))
            {
                float DpiValue = Math.Min(newZoom, 8);
                DpiValue = Math.Max(DpiValue, 0);

                animCanvas.DpiScale = DpiValue;
            }
        }

        private void MatchDPI_Unchecked(object sender, RoutedEventArgs e)
        {
            //When the check gets unchecked, just set the DPI scale to 1, which is shitty when zoomed in.
            animCanvas.DpiScale = 1;
        }
        #endregion

        #region Mouse Controls

        Point? dragStart;
        private void AnimCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            dragStart = e.GetCurrentPoint(animCanvas).Position;
        }

        private void AnimCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

            int panMagnifyFactor = 0;
            float newZoom;
            if(float.TryParse(ZoomFactor.Text, out newZoom))
            {
                panMagnifyFactor = (int)(8 * newZoom);
            }

            //Constantly updates the position the pan window should be as you drag along. 
            //Each time this is called constitutes a new drag.
            if(dragStart.HasValue)
            {
                Point current = e.GetCurrentPoint(animCanvas).Position;
                Point moveDelta = new Point((current.X - dragStart.Value.X) * panMagnifyFactor * -1
                                            , (current.Y - dragStart.Value.Y) * panMagnifyFactor * -1);
                Point offset = new Point(Zoomer.HorizontalOffset + moveDelta.X, Zoomer.VerticalOffset + moveDelta.Y);

                Zoomer.ChangeView(offset.X, offset.Y, null);
                dragStart = current;
            }
        }

        private void AnimCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //When a drag is released, null it out
            if(dragStart.HasValue)
            {
                dragStart = null;
            }
        }

        #endregion

        #region Keyboard Controls
        Boolean viewPast;
        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch(args.VirtualKey)
            {
                case VirtualKey.Q: IncreaseZoom(); break;
                case VirtualKey.E: DecreaseZoom(); break;
                case VirtualKey.X: viewPast = true; break;
                default: break;
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch(args.VirtualKey)
            {
                case VirtualKey.X: viewPast = false; break;
                default: break;
            }
        }
        #endregion
    }
}
