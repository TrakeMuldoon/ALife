using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ALifeUni.ALife;
using ALifeUni.ALife.Brains;
using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Scenarios;
using ALifeUni.ALife.Shapes;
using ALifeUni.UI;
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
        public static string ScenarioName = string.Empty;
        public static int? ScenarioSeed = null;

        public AgentUISettings AgentVisualSettings;
        public int DrawingErrors = 0;
        public List<LayerUISettings> VisualSettings;
        private readonly DispatcherTimer gameTimer = new DispatcherTimer();
        private readonly long startticks;
        private int mostChildrenIndex = -1;

        private int numSpecialItemsCreated;

        private int oldestIndex = -1;

        private string seed = string.Empty;

        private int shortBrainIndex = -1;

        private bool showParents;

        private WorldObject special;

        private int SpecialSelectorIndex = -1;

        public MainPage()
        {
            InitializeComponent();

            var scenario = ScenarioFactory.GetScenario(ScenarioName);

            if (ScenarioSeed != null)
            {
                Planet.CreateWorld(ScenarioSeed.Value, scenario);
            }
            else
            {
                Planet.CreateWorld(scenario);
            }

            animCanvas.ClearColor = Colors.NavajoWhite;
            animCanvas.Height = Planet.World.Scenario.WorldHeight;
            animCanvas.Width = Planet.World.Scenario.WorldWidth;

            seed = Planet.World.Seed.ToString();
            SimSeed.Text = seed;

            AgentVisualSettings = new AgentUISettings();
            AgentUI.DataContext = AgentVisualSettings;
            VisualSettings = LayerUISettings.GetDefaultSettings();
            VisualSettingsList.ItemsSource = VisualSettings;

            startticks = DateTime.Now.Ticks;
            gameTimer.Tick += Dt_Tick;

            StartSimWithInterval(100);
        }

        private void AnimCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
        }

        private void AnimCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if (FASTFORWARDING)
            {
                return;
            }

            if (special != null
                && special.Shape is Circle ssc)
            {
                var agentCentre = ssc.CentrePoint.ToVector2();
                args.DrawingSession.DrawCircle(agentCentre, ssc.Radius + 1, Colors.Red);
            }

            foreach (var layer in VisualSettings)
            {
                DrawLayer(layer, AgentVisualSettings, args);
            }

            if (showParents)
            {
                DrawingLogic.DrawAncestry(args);
            }
        }

        private void AnimCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tapPoint = e.GetPosition(animCanvas);
            var bb = new BoundingBox(tapPoint.X - 5, tapPoint.Y - 5, tapPoint.X + 5, tapPoint.Y + 5);
            var colls = Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical].QueryForBoundingBoxCollisions(bb);

            var cw = Window.Current.CoreWindow;
            var ks = cw.GetAsyncKeyState(VirtualKey.Control);

            if (ks.HasFlag(CoreVirtualKeyStates.Down))
            {
                if (colls.Count == 0)
                {
                    //EmptyObject eo = new EmptyObject(tapPoint, 5, ReferenceValues.CollisionLevelPhysical, (numSpecialItemsCreated++).ToString());
                    //Planet.World.AddObjectToWorld(eo);
                    var w = new Wall(tapPoint, 200, new Angle(10), "W" + numSpecialItemsCreated++);
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

        private void CollisionTestPanelButton_Click(object sender, RoutedEventArgs e)
        {
            CollisionTestPopup.IsOpen = !CollisionTestPopup.IsOpen;
        }

        private void DrawLayer(LayerUISettings ui, AgentUISettings aui, CanvasAnimatedDrawEventArgs args)
        {
            if (!ui.ShowObjects)
            {
                return;
            }

            try
            {
                //Special Layer, Zones draw different
                if (ui.LayerName == ReferenceValues.CollisionLevelZone)
                {
                    foreach (var z in Planet.World.Zones.Values)
                    {
                        DrawingLogic.DrawZone(z, args);
                    }
                    return;
                }
                //Special Layer, DeadLayer draws different
                if (ui.LayerName == ReferenceValues.CollisionLevelDead)
                {
                    for (var i = 0; i < Planet.World.InactiveObjects.Count; i++)
                    {
                        var obj = Planet.World.InactiveObjects[i];
                        DrawingLogic.DrawInactiveObject(obj, ui, args);
                    }
                    return;
                }

                if (!Planet.World.CollisionLevels.ContainsKey(ui.LayerName))        //Layer doesn't exist
                {
                    return;
                }

                if (special != null && viewPast)
                {
                    //This is for when the 'x' key is depressed, it means we view what the selected agent saw when it's
                    //turn happened.
                    //Because each agent executes in order, any agents BEFORE the execution order of the selected agent, are in the correct
                    //spots, but any agents with a HIGHER execution order will have been moved.
                    //So they must be drawn in their previous location.
                    DrawingLogic.DrawPastState(ui, aui, args, special.ExecutionOrder);
                }
                else
                {
                    //Default Draw Normal case
                    foreach (var wo in Planet.World.CollisionLevels[ui.LayerName].EnumerateItems())
                    {
                        DrawingLogic.DrawWorldObject(wo, ui, aui, args);
                    }
                }
            }
            catch (Exception)
            {
                DrawingErrors++;
            }
        }

        private void Dt_Tick(object sender, object e)
        {
            Planet.World.ExecuteOneTurn();
            AgentPanel.updateInfo();
            WorldSummary.UpdateWorldSummary();
        }

        private int FindAndSelect(int currentIndex, Func<Agent, int> testerFunction, bool greaterThan)
        {
            var compValue = greaterThan ? int.MinValue : int.MaxValue;
            var options = new List<Agent>();

            for (var i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                var wo = Planet.World.AllActiveObjects[i];
                if (wo is Agent ag
                    && ag.Alive)
                {
                    var currentValue = testerFunction(ag);
                    if (currentValue == compValue)
                    {
                        options.Add(ag);
                    }
                    else if ((greaterThan
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

            if (++currentIndex >= options.Count)
            {
                currentIndex = 0;
            }
            special = options[currentIndex];
            AgentPanel.TheAgent = options[currentIndex];

            return currentIndex;
        }

        private void LongestLived_Click(object sender, RoutedEventArgs e)
        {
            oldestIndex = FindAndSelect(oldestIndex
                                        , (ag) => ag.Statistics["Age"].Value
                                        , true);
        }

        private void MakeSpecial(List<WorldObject> colls)
        {
            if (colls.Count < 1)
            {
                return;
            }

            //Reset the Special related stuff
            AgentPanel.TheAgent = null;
            WallPane.TheWall = null;
            WallPanelPopup.IsOpen = false;
            special = null;

            if (++SpecialSelectorIndex >= colls.Count)
            {
                SpecialSelectorIndex = 0;
            }
            //If we do have a collisions, then set that agent to be "Special"
            special = colls[SpecialSelectorIndex];

            switch (special)
            {
                case Agent ag:
                    AgentPanel.TheAgent = ag;
                    break;

                case Wall wall:
                    WallPane.TheWall = wall;
                    WallPanelPopup.IsOpen = true;
                    break;

                default:
                    break;
            }
        }

        private void MostChildren_Click(object sender, RoutedEventArgs e)
        {
            mostChildrenIndex = FindAndSelect(mostChildrenIndex
                                              , (ag) => ag.NumChildren
                                              , true);
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
            animCanvas.RemoveFromVisualTree();
            animCanvas = null;
        }

        private void RandResetSim_Click(object sender, RoutedEventArgs e)
        {
            seed = Planet.World.NumberGen.Next().ToString();
            ResetSimulation();
        }

        private void ResetSim_Click(object sender, RoutedEventArgs e)
        {
            ResetSimulation();
        }

        private void ResetSimulation()
        {
            var newCopy = IScenarioHelpers.FreshInstanceOf(Planet.World.Scenario);

            if (int.TryParse(seed, out var seedValue))
            {
                Planet.CreateWorld(seedValue, newCopy, (int)animCanvas.Height, (int)animCanvas.Width);
            }
            else
            {
                Planet.CreateWorld(newCopy, (int)animCanvas.Height, (int)animCanvas.Width);
            }

            seed = Planet.World.Seed.ToString();
            SimSeed.Text = seed;
            SimSeed_TextChanged(SimSeed, null);
            special = null;
        }

        private void ShortestBrain_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                shortBrainIndex = FindAndSelect(shortBrainIndex
                                                , (ag) => ((BehaviourBrain)ag.MyBrain).Behaviours.Count()
                                                , false);
            }
            catch (Exception)
            {
                //TODO: Create a more consumer friendly error handler. Currently all errors or swallowed, because the application is only run in debug.
            }
        }

        private void ShowGeneology_Checked(object sender, RoutedEventArgs e)
        {
            showParents = ((CheckBox)sender).IsChecked.Value;
        }

        private void SimSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            tb.BorderBrush = tb.Text != seed ? new SolidColorBrush(Colors.Red) : (Brush)new SolidColorBrush(Colors.Black);
            seed = tb.Text;
        }

        #region Speed controls

        private bool FASTFORWARDING = false;

        private int skipValue = 0;

        private async void FastForward(int turnsToSkip)
        {
            try
            {
                FASTFORWARDING = true;
                SkipProgress.Maximum = 100;
                SkipProgress.Minimum = 0;
                SkipProgress.Value = 0;

                var start = DateTime.Now;

                var restart_timer = false;
                if (gameTimer.IsEnabled)
                {
                    restart_timer = true;
                    gameTimer.Stop();
                }

                var skipPartLength = 1000;
                var progress = 0;
                var previous = start;
                while ((progress + skipPartLength) <= turnsToSkip)
                {
                    await Task.Run(() => Planet.World.ExecuteManyTurns(skipPartLength));
                    var current = DateTime.Now;
                    progress += skipPartLength;
                    SkipProgress.Value = progress / (double)turnsToSkip * 100.0;

                    var numLeft = turnsToSkip - progress;
                    var totalElapsed = current - start;
                    var recentElapsed = current - previous;

                    var tpsa = progress / (totalElapsed.TotalSeconds + 0.1);
                    var tpsc = skipPartLength / (recentElapsed.TotalSeconds + 0.1);

                    var estTotal = new TimeSpan(0, 0, (int)(numLeft / tpsa));
                    var curTotal = new TimeSpan(0, 0, (int)(numLeft / tpsc));

                    var info = string.Format("{0}/{1}:{2}\r\nETA>{3}:{4} ETC>{5}:{6}\r\nTPSA:{7:0.00}, TPSC:{8:0.00}"
                                             , progress, turnsToSkip, numLeft
                                                 , (int)estTotal.TotalMinutes, estTotal.ToString("ss"), (int)curTotal.TotalMinutes, curTotal.ToString("ss")
                                                 , tpsa, tpsc);

                    SkipInfo.Text = info;

                    previous = current;
                }
                if (progress < turnsToSkip)
                {
                    Planet.World.ExecuteManyTurns(turnsToSkip - progress);
                }

                if (restart_timer)
                {
                    gameTimer.Start();
                }
            }
            finally
            {
                FASTFORWARDING = false;
            }
            AgentPanel.updateInfo();
            WorldSummary.UpdateWorldSummary();
        }

        private void FastPlaySim_Click(object sender, RoutedEventArgs e)
        {
            StartSimWithInterval(1);
        }

        private void OneTurnSim_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            Dt_Tick(sender, e);
        }

        private void PauseSim_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
        }

        private void PlaySim_Click(object sender, RoutedEventArgs e)
        {
            StartSimWithInterval(100);
        }

        private void SkipAhead_Click(object sender, RoutedEventArgs e)
        {
            FastForward(200);
        }

        private void SkipFarAhead_Click(object sender, RoutedEventArgs e)
        {
            FastForward(5000);
        }

        private void SkipLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (int.TryParse(tb.Text, out var skip))
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

        private void SlowPlaySim_Click(object sender, RoutedEventArgs e)
        {
            StartSimWithInterval(500);
        }

        private void StartSimWithInterval(int interval)
        {
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            if (!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }

        #endregion Speed controls

        #region Zoom controls

        private static readonly List<string> ZoomFactors = new List<string>() { "0.1", "0.25", "0.5", "0.75", "0.9", "1", "2", "3", "4", "5", "6", "7", "8" };
        private float CurrentZoom = 1;
        private int ZoomIndex = 6;

        private void ChangeZoomLevel()
        {
            //Increase the zoom
            ZoomIndex = Math.Clamp(ZoomIndex, 0, ZoomFactors.Count - 1);
            var newZoom = float.Parse(ZoomFactors[ZoomIndex]);

            var newHeight = Zoomer.ActualHeight * newZoom;
            var newVert = Zoomer.VerticalOffset / Zoomer.ExtentHeight * newHeight;
            var newHorz = Zoomer.HorizontalOffset / Zoomer.ExtentWidth * newHeight;

            animCanvas.DpiScale = Math.Clamp(newZoom, 0.01f, 2.5f);
            _ = Zoomer.ChangeView(newHorz, newVert, newZoom);
            CurrentZoom = newZoom;
            ZoomDisplay.Text = newZoom.ToString();
        }

        private void DecreaseZoom()
        {
            ZoomIndex -= 1;
            ChangeZoomLevel();
        }

        private void IncreaseZoom()
        {
            ZoomIndex += 1;
            ChangeZoomLevel();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            IncreaseZoom();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            DecreaseZoom();
        }

        #endregion Zoom controls

        #region Mouse Controls

        private Point? dragStart;

        private void AnimCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            //Constantly updates the position the pan window should be as you drag along.
            //Each time this is called constitutes a new drag.
            if (dragStart.HasValue)
            {
                var panMagnifyFactor = (int)(8 * CurrentZoom);

                var current = e.GetCurrentPoint(animCanvas).Position;
                var moveDelta = new Point((current.X - dragStart.Value.X) * panMagnifyFactor * -1
                                           , (current.Y - dragStart.Value.Y) * panMagnifyFactor * -1);
                var offset = new Point(Zoomer.HorizontalOffset + moveDelta.X, Zoomer.VerticalOffset + moveDelta.Y);

                _ = Zoomer.ChangeView(offset.X, offset.Y, null);
                dragStart = current;
            }
        }

        private void AnimCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            dragStart = e.GetCurrentPoint(animCanvas).Position;
        }

        private void AnimCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //When a drag is released, null it out
            if (dragStart.HasValue)
            {
                dragStart = null;
            }
        }

        #endregion Mouse Controls

        #region Keyboard Controls

        private bool viewPast;

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Q:
                    IncreaseZoom();
                    break;

                case VirtualKey.E:
                    DecreaseZoom();
                    break;

                case VirtualKey.X:
                    viewPast = true;
                    break;

                default:
                    break;
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.X:
                    viewPast = false;
                    break;

                default:
                    break;
            }
        }

        #endregion Keyboard Controls

        private void WallPanelButton_Click(object sender, RoutedEventArgs e)
        {
            WallPanelPopup.IsOpen = !WallPanelPopup.IsOpen;
        }
    }
}
