using ALifeUni.ALife;
using ALifeUni.ALife.Brains;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni
{
    public sealed partial class NeuralBrainView : UserControl
    {
        int canvasHeight;
        int canvasWidth;
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
                    //TODO: RESET STUFF
                }
            }
        }

        private void brainCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if(brain == null)
            {
                return;
            }
            args.DrawingSession.FillCircle(new Vector2(0, 0), 20, Colors.Green);
            args.DrawingSession.FillCircle(new Vector2(0, canvasHeight), 20, Colors.Blue);
            args.DrawingSession.FillCircle(new Vector2(canvasWidth, canvasHeight), 20, Colors.Yellow);
            args.DrawingSession.FillCircle(new Vector2(canvasWidth, 0), 20, Colors.Purple);

            int heightSpacer = (int)(canvasHeight / brain.Layers.Count);
            for(int i = 0; i < brain.Layers.Count; ++i)
            {
                Layer layer = brain.Layers[i];
                int widthSpacer = (int)(canvasWidth / layer.Neurons.Count);
                for(int j = 0; j < layer.Neurons.Count; ++j)
                {
                    Neuron nn = layer.Neurons[j];
                    Vector2 neuronCenter = new Vector2(widthSpacer * (j + 1),heightSpacer * (i + 1));
                    args.DrawingSession.DrawCircle(neuronCenter, 5, Colors.Red);
                }
            }
        }

        private void brainCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void brainCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void brainCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

        }

        private void brainCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }

        private void brainCanvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }

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
    }
}
