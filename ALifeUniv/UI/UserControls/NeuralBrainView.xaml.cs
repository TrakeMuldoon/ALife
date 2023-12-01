using ALifeUni.ALife;
using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Utility;
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
                if(theAgent != null)
                {
                    AgentName.Text = theAgent.IndividualLabel;
                    brain = theAgent.MyBrain as NeuralNetworkBrain;
                    BuildNodeLocationDictionary();
                }
            }
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


            int neuronCounter = 0;
            //This backup exists because there is a race condition when another agent is selected WHILE
            // while the neural net is being drawn. Keeping the original brain temporarily in context solves the error.
            Dictionary<Neuron, Vector2> backup = NodeMap;
            foreach(var (neuron, point) in backup)
            {

                foreach(Dendrite den in neuron.UpstreamDendrites)
                {
                    Color dendriteColour = den.CurrentValue > 0 ? Colors.Gray : Colors.Purple;
                    dendriteColour = den.CurrentValue > 0.5 ? Colors.Black : dendriteColour;
                    Vector2 targetPoint = backup[den.TargetNeuron];
                    args.DrawingSession.DrawLine(point, targetPoint, dendriteColour);
                }

                if(SelectedNeuron == neuron)
                {
                    args.DrawingSession.FillCircle(point, NEURON_VIS_RADIUS + 4, Colors.Orange);
                }
                args.DrawingSession.FillCircle(point, NEURON_VIS_RADIUS, Colors.Moccasin);

                Color neuronColour = neuron.Value > 0 ? Colors.Blue : Colors.Red;

                args.DrawingSession.DrawCircle(point, NEURON_VIS_RADIUS, neuronColour);
                neuronColour.A = (byte) (Math.Abs(neuron.Value) * 255);
                args.DrawingSession.FillCircle(point, NEURON_VIS_RADIUS, neuronColour);

                CanvasTextFormat ctf = new CanvasTextFormat();
                ctf.FontSize = 10;

                //gross unreadable temporary code;
                float textY = ++neuronCounter % 2 == 0 ? point.Y + 10 : point.Y + 20;
                float textX = point.X - 13;

                Vector2 textPoint = new Vector2(textX, textY);
                args.DrawingSession.DrawText(neuron.Name, textPoint, Colors.Black, ctf);
            }
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

        private void BuildNodeLocationDictionary()
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
    }
}
