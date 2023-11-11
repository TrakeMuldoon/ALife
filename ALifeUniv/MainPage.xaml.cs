using ALifeUni.ALife;
using ALifeUni.ALife.Brains;
using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Scenarios;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using ALifeUni.UI;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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
        readonly DispatcherTimer gameTimer = new DispatcherTimer();
        public List<LayerUISettings> VisualSettings;
        public int DrawingErrors = 0;

        public MainPage()
        {
            this.InitializeComponent();
            /* ***********************************
             * Set Scenario Here
             * ***********************************/
            Planet.CreateWorld(new FieldCrossingScenario());
            //Planet.CreateWorld(new MazeScenario());

            animCanvas.ClearColor = Colors.NavajoWhite;
            animCanvas.Height = Planet.World.Scenario.WorldHeight;
            animCanvas.Width = Planet.World.Scenario.WorldWidth;

            seed = Planet.World.Seed.ToString();
            SimSeed.Text = seed;

            VisualSettings = LayerUISettings.GetSettings();
            VisualSettingsList.ItemsSource = VisualSettings;

            startticks = DateTime.Now.Ticks;
            gameTimer.Tick += Dt_Tick;

            PlaySim_Go();
        }

        private void AnimCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
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
            UpdateZoneInfo();
            UpdateGeneology();
        }

        private Boolean showParents;
        private void AnimCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if(special != null
                && special.Shape is Circle ssc)
            {
                Vector2 agentCentre = ssc.CentrePoint.ToVector2();
                args.DrawingSession.DrawCircle(agentCentre, ssc.Radius + 1, Colors.Red);
            }

            foreach(LayerUISettings layer in VisualSettings)
            {
                DrawLayer(layer, args);
            }

            if(showParents)
            {
                DrawingLogic.DrawAncestry(args);
            }

            foreach(Point p in taps)
            {
                args.DrawingSession.FillCircle(new Vector2((float)p.X, (float)p.Y), 1, Colors.Peru);
            }
        }

        private void ShowGeneology_Checked(object sender, RoutedEventArgs e)
        {
            showParents = ((CheckBox)sender).IsChecked.Value;
        }

        private void UpdateZoneInfo()
        {
            Dictionary<string, int> zoneCount = new Dictionary<string, int>();
            foreach(Zone z in Planet.World.Zones.Values)
            {
                zoneCount.Add(z.Name, 0);
            }
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                WorldObject wo = Planet.World.AllActiveObjects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    zoneCount[ag.Zone.Name]++;
                }
            }
            foreach(string name in zoneCount.Keys)
            {
                sb.AppendLine(name + ":" + zoneCount[name]);
            }
            //sb.AppendLine("WORLD: " + Planet.World.AllActiveObjects.Where(wo => wo.Alive).Count());
            ZoneInfo.Text = sb.ToString();
            Turns.Text = Planet.World.Turns.ToString();
        }

        private void UpdateGeneology()
        {
            Dictionary<string, int> geneCount = new Dictionary<string, int>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                WorldObject wo = Planet.World.AllActiveObjects[i];
                if (wo is Agent ag
                    && ag.Alive)
                {
                    string gene = ag.IndividualLabel.Substring(0, 3);
                    if(!geneCount.ContainsKey(gene))
                    {
                        geneCount.Add(gene, 0);
                    }
                    ++geneCount[gene];
                }
            }

            GeneologyInfo.Text = "Genes Active: " + geneCount.Count;
        }

        private void DrawLayer(LayerUISettings ui, CanvasAnimatedDrawEventArgs args)
        {
            try
            {
                if(!ui.ShowLayer)
                {
                    return;
                }

                //Special Layer, Zones draw different
                if(ui.LayerName == ReferenceValues.CollisionLevelZone)
                {
                    foreach(Zone z in Planet.World.Zones.Values)
                    {
                        DrawingLogic.DrawZone(z, args);
                    }
                    return;
                }
                if(ui.LayerName == ReferenceValues.CollisionLevelDead)
                {
                    for(int i = 0; i < Planet.World.InactiveObjects.Count; i++)
                    {
                        WorldObject obj = Planet.World.InactiveObjects[i];
                        DrawingLogic.DrawInactiveObject(obj, ui, args);

                    }
                    return;
                }

                if(!Planet.World.CollisionLevels.ContainsKey(ui.LayerName))        //Layer doesn't exist
                {
                    return;
                }

                if(viewPast
                    && special != null)
                {
                    int compnumber = special.ExecutionOrder;
                    DrawingLogic.DrawPastState(ui, args, compnumber);
                }
                else
                {
                    //Default Draw Normal case
                    foreach(WorldObject wo in Planet.World.CollisionLevels[ui.LayerName].EnumerateItems())
                    {
                        DrawingLogic.DrawWorldObject(wo, ui, args);
                    }
                }
            }
            catch (Exception ex) {
                DrawingErrors++;
            }
        }

        WorldObject special;
        int numSpecialItemsCreated;
        private void AnimCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point tapPoint = e.GetPosition(animCanvas);
            BoundingBox bb = new BoundingBox(tapPoint.X - 5, tapPoint.Y - 5, tapPoint.X + 5, tapPoint.Y + 5);
            List<WorldObject> colls = Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical].QueryForBoundingBoxCollisions(bb);

            CoreWindow cw = Window.Current.CoreWindow;
            CoreVirtualKeyStates ks = cw.GetAsyncKeyState(VirtualKey.Control);

            if(ks.HasFlag(CoreVirtualKeyStates.Down))
            {
                if(colls.Count == 0)
                {
                    //EmptyObject eo = new EmptyObject(tapPoint, 5, ReferenceValues.CollisionLevelPhysical, (numSpecialItemsCreated++).ToString());
                    //Planet.World.AddObjectToWorld(eo);
                    Wall w = new Wall(tapPoint, 200, new Angle(10), "W" + numSpecialItemsCreated++);
                    Planet.World.AddObjectToWorld(w);
                }
                else
                {
                    colls[0].Die();
                }
            }
            else
            {
                MakeSpecial(colls);
            }
        }


        int SpecialSelectorIndex = -1;
        private void MakeSpecial(List<WorldObject> colls)
        {
            if(colls.Count < 1)
            {
                return;
            }

            //Reset the Special related stuff
            AgentPanel.TheAgent = null;
            WallPane.TheWall = null;
            special = null;

            if(++SpecialSelectorIndex >= colls.Count)
            {
                SpecialSelectorIndex = 0;
            }
            //If we do have a collisions, then set that agent to be "Special"
            special = colls[SpecialSelectorIndex];

            switch(special)
            {
                case Agent ag: AgentPanel.TheAgent = ag; break;
                case Wall wall: WallPane.TheWall = wall; break;
                default: break;
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
            if(int.TryParse(seed, out int seedValue))
            {
                Planet.World.Scenario.Reset();
                Planet.CreateWorld(seedValue, Planet.World.Scenario, (int)animCanvas.Height, (int)animCanvas.Width);
            }
            else
            {
                Planet.World.Scenario.Reset();
                Planet.CreateWorld(Planet.World.Scenario, (int)animCanvas.Height, (int)animCanvas.Width);
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
            FastForward(5000);
        }

        int skipValue = 0;
        private void SkipLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if(int.TryParse(tb.Text, out int skip))
            {
                skipValue = skip;
            }
            else
            {
                tb.Text = "0";
                skipValue = 0;
            }
        }

        private void SkipSpecificAhead_Click(object sender, RoutedEventArgs e)
        {
            FastForward(skipValue);
        }

        private async void FastForward(int turnsToSkip)
        {
            SkipProgress.Maximum = 100;
            SkipProgress.Minimum = 0;
            SkipProgress.Value = 0;

            DateTime start = DateTime.Now;

            bool restart = false;
            if(gameTimer.IsEnabled)
            {
                restart = true;
                gameTimer.Stop();
            }
            Queue<bool> viewables = new Queue<bool>();
            foreach(LayerUISettings set in VisualSettings)
            {
                viewables.Enqueue(set.ShowLayer);
                set.ShowLayer = false;
            }

            int skipPartLength = 1000;
            int progress = 0;
            DateTime previous = start;
            while((progress + skipPartLength) <= turnsToSkip)
            {
                await Task.Run(() => Planet.World.ExecuteManyTurns(skipPartLength));
                DateTime current = DateTime.Now;
                progress += skipPartLength;
                SkipProgress.Value = (double)progress / (double)turnsToSkip * 100.0;

                int numLeft = turnsToSkip - progress;
                TimeSpan totalElapsed = current - start;
                TimeSpan recentElapsed = current - previous;

                double tpsa = progress / (totalElapsed.TotalSeconds + 0.1);
                double tpsc = skipPartLength / (recentElapsed.TotalSeconds + 0.1);

                TimeSpan estTotal = new TimeSpan(0, 0, (int)(numLeft / tpsa));
                TimeSpan curTotal = new TimeSpan(0, 0, (int)(numLeft / tpsc));

                String info = String.Format("{0}/{1}:{2}\r\nETA>{3}:{4} ETC>{5}:{6}\r\nTPSA:{7:0.00}, TPSC:{8:0.00}"
                                            , progress, turnsToSkip, numLeft
                                            , (int)estTotal.TotalMinutes, estTotal.ToString("ss"), (int)curTotal.TotalMinutes, curTotal.ToString("ss")
                                            , tpsa, tpsc);

                SkipInfo.Text = info;

                previous = current;
            }
            if(progress < turnsToSkip)
            {
                Planet.World.ExecuteManyTurns(turnsToSkip - progress);
            }
            
            AgentPanel.updateInfo();

            foreach(LayerUISettings set in VisualSettings)
            {
                set.ShowLayer = viewables.Dequeue();
            }
            if(restart)
            {
                gameTimer.Start();
            }
        }

        #endregion

        #region Zoom controls

        private static readonly List<String> ZoomFactors = new List<String>() { "0.1", "0.25", "0.5", "0.75", "0.9", "1", "2", "3", "4", "5", "6", "7", "8" };
        private int ZoomIndex = 6;
        private float CurrentZoom = 1;

        private void ChangeZoomLevel()
        {
            //Increase the zoom
            ZoomIndex = Math.Clamp(ZoomIndex, 0, ZoomFactors.Count - 1);
            float newZoom = float.Parse(ZoomFactors[ZoomIndex]);

            double newHeight = Zoomer.ActualHeight * newZoom;
            double newVert = Zoomer.VerticalOffset / Zoomer.ExtentHeight * newHeight;
            double newHorz = Zoomer.HorizontalOffset / Zoomer.ExtentWidth * newHeight;

            animCanvas.DpiScale = Math.Clamp(newZoom, 0.01f, 2.5f);
            Zoomer.ChangeView(newHorz, newVert, newZoom);
            CurrentZoom = newZoom;
            ZoomDisplay.Text = newZoom.ToString();
        }


        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            DecreaseZoom();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            IncreaseZoom();
        }

        private void IncreaseZoom()
        {
            ZoomIndex += 1;
            ChangeZoomLevel();
        }

        private void DecreaseZoom()
        {
            ZoomIndex -= 1;
            ChangeZoomLevel();
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
            //Constantly updates the position the pan window should be as you drag along. 
            //Each time this is called constitutes a new drag.
            if(dragStart.HasValue)
            {
                int panMagnifyFactor = (int)(8 * CurrentZoom);

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

        int oldestIndex = -1;
        private void LongestLived_Click(object sender, RoutedEventArgs e)
        {
            oldestIndex = FindAndSelect(oldestIndex
                                        , (ag) => ag.Statistics["Age"].Value
                                        , true);
        }



        int shortBrainIndex = -1;
        private void ShortestBrain_Click(object sender, RoutedEventArgs e)
        {
            shortBrainIndex = FindAndSelect(shortBrainIndex
                                            , (ag) => ((BehaviourBrain)ag.MyBrain).Behaviours.Count()
                                            , false);
        }

        int mostChildrenIndex = -1;
        private void MostChildren_Click(object sender, RoutedEventArgs e)
        {
            mostChildrenIndex = FindAndSelect(mostChildrenIndex
                                              , (ag) => ag.NumChildren
                                              , true);
        }

        private int FindAndSelect(int currentIndex, Func<Agent, int> testerFunction, bool greaterThan)
        {
            int compValue = greaterThan ? int.MinValue : int.MaxValue;
            List<Agent> options = new List<Agent>();

            for(int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                WorldObject wo = Planet.World.AllActiveObjects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    int currentValue = testerFunction(ag);
                    if(currentValue == compValue)
                    {
                        options.Add(ag);
                    }
                    else if((greaterThan
                             && currentValue > compValue)
                             || (!greaterThan
                                 && currentValue < compValue))
                    {
                        options.Clear();
                        compValue = currentValue;
                        options.Add(ag);
                    }
                }
            }

            if(++currentIndex >= options.Count)
            {
                currentIndex = 0;
            }
            special = options[currentIndex];
            AgentPanel.TheAgent = options[currentIndex];

            return currentIndex;
        }

        List<Point> taps = new List<Point>();
        private void DotMaker_Click(object sender, RoutedEventArgs e)
        {
            string tex = Coords.Text;
            string[] coords = tex.Split(',');
            if(coords.Length != 2) return;

            coords[0] = coords[0].Trim();
            coords[1] = coords[1].Trim();

            double x = double.Parse(coords[0]);
            double y = double.Parse(coords[1]);

            Point p = new Point(x, y);
            taps.Clear();
            taps.Add(p);
        }

        private void CollisionTestPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if(CollisionTestPanel.Visibility == Visibility.Visible)
            {
                CollisionTestPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                CollisionTestPanel.Visibility = Visibility.Visible;
            }
        }
    }
}
