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
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni
{
    public sealed partial class NeuralBrainView : UserControl
    {
        public NeuralBrainView()
        {
            this.InitializeComponent();
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

        private void brainCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
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
