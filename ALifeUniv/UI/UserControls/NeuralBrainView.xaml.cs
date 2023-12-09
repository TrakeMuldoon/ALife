using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Brains.NeuralNetworkBrains;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni.UI.UserControls
{
    public sealed partial class NeuralBrainView : UserControl
    {
        private int canvasHeight;
        private int canvasWidth;
        private const int NEURON_VIS_RADIUS = 8;
        private Dictionary<Neuron, Vector2> NodeMap;
        private Neuron SelectedNeuron = null;

        public NeuralBrainView()
        {
            this.InitializeComponent();
            canvasHeight = (int)brainCanvas.Height;
            canvasWidth = (int)brainCanvas.Width;
            brainCanvas.ClearColor = Colors.NavajoWhite;
        }

        private NeuralNetworkBrain brain;
        private Agent theAgent;
        public Agent TheAgent
        {
            get => theAgent;
            set
            {
                theAgent = value;
                AgentName.Text = "No Agent Selected";
                NodeMap = null;
                NeuronToDownstreamDendrites = null;
                SelectedNeuron = null;
                if(theAgent != null)
                {
                    AgentName.Text = theAgent.IndividualLabel.Length < 25 ? theAgent.IndividualLabel : TruncateAgentName(theAgent.IndividualLabel);
                    brain = theAgent.MyBrain as NeuralNetworkBrain;
                    InitializeNodeLocationDictionary();
                    InitializeDownstreamDendriteMap();
                }
            }
        }

        private String TruncateAgentName(String str)
        {
            return $"{str.Substring(0, 3)}...{str.Length - 13}...{str.Substring(str.Length - 10)}";
        }

        private void brainCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if(brain == null)
            {
                //TODO: Create Error Message Somewhere
                return;
            }
            if(!(brain is NeuralNetworkBrain))
            {
                //TODO: Create Error Message Somewhere
                return;
            }

            args.DrawingSession.FillCircle(new Vector2(0, 0), 30, Colors.Green);
            args.DrawingSession.FillCircle(new Vector2(0, canvasHeight), 30, Colors.Blue);
            args.DrawingSession.FillCircle(new Vector2(canvasWidth, canvasHeight), 30, Colors.Yellow);
            args.DrawingSession.FillCircle(new Vector2(canvasWidth, 0), 30, Colors.Purple);

            try
            {
                if(SelectedNeuron != null)
                {
                    DrawWithSpecialNeuron(args);
                }
                else
                {
                    DrawAllNeurons(args);
                }
            }
            catch(Exception)
            {
                throw;
                //Swallow. It's just a drawing error. We'll get it right next time.
                //TODO: Determine why NullRefs happen when we switch SpecialNodes. I suspect race condition... but where?
            }


        }

        private void DrawAllNeurons(CanvasAnimatedDrawEventArgs args)
        {
            //This is just used to stagger the neuron names
            int neuronCounter = 0;

            //This backup exists because there is a race condition when another agent is selected WHILE
            // while the neural net is being drawn. Keeping the original brain temporarily in context solves the error.
            Dictionary<Neuron, Vector2> backup = NodeMap;
            foreach(var (neuron, point) in backup)
            {
                DrawDendrites(args, backup, neuron, point);

                DrawNeuron(args, ++neuronCounter % 2 == 0, neuron, point);
            }
        }

        private void DrawWithSpecialNeuron(CanvasAnimatedDrawEventArgs args)
        {
            //This is just used to stagger the neuron names
            int neuronCounter = 0;

            //This backup exists because there is a race condition when another agent is selected WHILE
            // while the neural net is being drawn. Keeping the original brain temporarily in context solves the error.
            Dictionary<Neuron, Vector2> backup = NodeMap;

            DrawDownstreamDendrites(args, SelectedNeuron, backup);

            foreach(var (neuron, point) in backup)
            {
                bool drawTextUp = ++neuronCounter % 2 == 0;

                if(SelectedNeuron == neuron)
                {
                    DrawDendrites(args, backup, neuron, point);
                    DrawUpstreamValues(args, backup, neuron);
                }

                DrawNeuron(args, drawTextUp, neuron, point);
            }
        }

        private static void DrawUpstreamValues(CanvasAnimatedDrawEventArgs args, Dictionary<Neuron, Vector2> backup, Neuron neuron)
        {
            int denCount = 0;
            foreach(Dendrite den in neuron.UpstreamDendrites)
            {
                bool drawTextUp = denCount++ % 2 == 0;
                Vector2 UpstreamCentrePoint = backup[den.TargetNeuron];

                float textX = UpstreamCentrePoint.X - 13;
                float textY = drawTextUp ? UpstreamCentrePoint.Y - 30 : UpstreamCentrePoint.Y - 50;
                Vector2 textPoint = new Vector2(textX, textY);
                CanvasTextFormat ctf = new CanvasTextFormat() { FontSize = 12 };

                DrawBackedText(args, den.TargetNeuron.Value.ToString("0.00"), textPoint, ctf);

                //Move text to below the Neuron, for the DenValue
                textPoint.X += 5;
                textPoint.Y += 70;
                DrawBackedText(args, den.CurrentValue.ToString("0.00"), textPoint, ctf);
            }
        }

        private static Vector2 DrawBackedText(CanvasAnimatedDrawEventArgs args, string text, Vector2 textPoint, CanvasTextFormat ctf)
        {
            Vector2 textRoot = new Vector2(textPoint.X, textPoint.Y);

            args.DrawingSession.DrawText(text, textRoot, Colors.Black, ctf);
            textRoot.X += 1;
            textRoot.Y += 1;
            args.DrawingSession.DrawText(text, textRoot, Colors.Green, ctf);
            return textPoint;
        }

        private static void DrawNeuron(CanvasAnimatedDrawEventArgs args, bool textHigh, Neuron neuron, Vector2 point)
        {
            //Draw background to cover the Dendrite ends
            args.DrawingSession.FillCircle(point, NEURON_VIS_RADIUS, Colors.Moccasin);

            //Select colour according to value
            Color neuronColour = neuron.Value > 0 ? Colors.Blue : Colors.Red;

            //Draw Neuron Outline 
            args.DrawingSession.DrawCircle(point, NEURON_VIS_RADIUS, neuronColour);

            //Alpha according to value
            neuronColour.A = (byte)(Math.Abs(neuron.Value) * 255);
            args.DrawingSession.FillCircle(point, NEURON_VIS_RADIUS, neuronColour);

            //Draw name of Neuron beneath it.
            CanvasTextFormat ctf = new CanvasTextFormat() { FontSize = 10 };

            //This code staggers the names so they don't overlap.
            float textY = textHigh ? point.Y + 10 : point.Y + 30;
            float textX = point.X - 13;
            Vector2 textPoint = new Vector2(textX, textY);

            //Draw the text
            args.DrawingSession.DrawText(neuron.Name, textPoint, Colors.Black, ctf);
        }

        private void DrawDownstreamDendrites(CanvasAnimatedDrawEventArgs args, Neuron selectedNeuron, Dictionary<Neuron, Vector2> nodemap)
        {
            if(!NeuronToDownstreamDendrites.ContainsKey(selectedNeuron))
            {
                //Action node selected. There are no downstream dendrites to draw
                return;
            }
            Vector2 homePoint = nodemap[selectedNeuron];
            foreach(var (den, parentNeuron) in NeuronToDownstreamDendrites[selectedNeuron])
            {
                Vector2 targetPoint = nodemap[parentNeuron];
                Color dendriteColour = GetDendriteColor(den);
                args.DrawingSession.DrawLine(homePoint, targetPoint, dendriteColour);
            }
        }

        private static void DrawDendrites(CanvasAnimatedDrawEventArgs args, Dictionary<Neuron, Vector2> nodemap, Neuron neuron, Vector2 point)
        {
            foreach(Dendrite den in neuron.UpstreamDendrites)
            {
                Vector2 targetPoint = nodemap[den.TargetNeuron];
                Color dendriteColour = GetDendriteColor(den);
                args.DrawingSession.DrawLine(point, targetPoint, dendriteColour);
            }
        }

        private static Color GetDendriteColor(Dendrite den)
        {
            Color dendriteColour = Colors.Black;
            if(den.CurrentValue < -0.5)
            {
                dendriteColour = Colors.Purple;
            }
            else if(den.CurrentValue < 0)
            {
                dendriteColour = Colors.Salmon;
            }
            else if(den.CurrentValue < 0.5)
            {
                dendriteColour = Colors.Gray;
            }
            //Else > 0.5 = default black;
            return dendriteColour;
        }

        int forgiveness = 0;
        private void brainCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point tapPoint = e.GetPosition(brainCanvas);
            foreach(var (neuron, point) in NodeMap)
            {
                double dist = ExtraMath.DistanceBetweenTwoPoints(tapPoint, new Point(point.X, point.Y));
                if(dist < NEURON_VIS_RADIUS)
                {
                    SelectedNeuron = neuron;
                    forgiveness = 0;
                    return;
                }
            }
            if(++forgiveness >= 2)
            {
                forgiveness = 0;
                SelectedNeuron = null;
            }
        }

        private void brainCanvas_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            forgiveness = 0;
            SelectedNeuron = null;
        }

        private void brainCanvas_PointerPressed(object sender, PointerRoutedEventArgs e) { }
        private void brainCanvas_PointerMoved(object sender, PointerRoutedEventArgs e) { }
        private void brainCanvas_PointerReleased(object sender, PointerRoutedEventArgs e) { }
        private void brainCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) { }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Popup parentPopup = FindFirstParentOfType<Popup>(this);
            if(parentPopup != null)
            {
                parentPopup.IsOpen = false;
            }
        }

        private T FindFirstParentOfType<T>(FrameworkElement element) where T : FrameworkElement
        {
            if(element.Parent == null)
            {
                return null;
            }
            Type parentType = element.Parent.GetType();
            if(element.Parent is T)
            {
                return element.Parent as T;
            }
            if(!(element.Parent is FrameworkElement))
            {
                throw new Exception("Unexpected Type of Parent");
            }
            return FindFirstParentOfType<T>(element.Parent as FrameworkElement);
        }

        private void InitializeNodeLocationDictionary()
        {
            NodeMap = new Dictionary<Neuron, Vector2>();
            int heightSpacer = (int)(canvasHeight / (brain.Layers.Count));
            for(int i = brain.Layers.Count - 1; i > -1; --i)
            {
                Layer layer = brain.Layers[i];
                int widthSpacer = (int)(canvasWidth / (layer.Neurons.Count + 1));
                for(int j = 0; j < layer.Neurons.Count; ++j)
                {
                    Neuron nn = layer.Neurons[j];
                    int x = (int)(widthSpacer * (j + 1));
                    int y = (int)(heightSpacer * (i + 0.5));
                    Vector2 neuronCentre = new Vector2(x, y);
                    NodeMap.Add(nn, neuronCentre);
                }
            }
        }


        private Dictionary<Neuron, List<(Dendrite, Neuron)>> NeuronToDownstreamDendrites;
        private void InitializeDownstreamDendriteMap()
        {
            NeuronToDownstreamDendrites = new Dictionary<Neuron, List<(Dendrite, Neuron)>>();

            foreach(Layer lay in brain.Layers)
            {
                foreach(Neuron n in lay.Neurons)
                {
                    foreach(Dendrite den in n.UpstreamDendrites)
                    {
                        Neuron targetNeuron = den.TargetNeuron;
                        if(!NeuronToDownstreamDendrites.ContainsKey(targetNeuron))
                        {
                            NeuronToDownstreamDendrites.Add(targetNeuron, new List<(Dendrite, Neuron)>());
                        }
                        NeuronToDownstreamDendrites[targetNeuron].Add((den, n));
                    }
                }
            }
        }
    }
}
