using System;
using ALife.Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Avalonia.Views
{
    /// <summary>
    /// </summary>
    /// <seealso cref="Avalonia.Controls.UserControl"/>
    public partial class SingularRunnerView : UserControl, IDisposable
    {
        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularRunnerView"/> class.
        /// </summary>
        public SingularRunnerView()
        {
            InitializeComponent();

            VisualSettingsList.Items.Clear();
            VisualSettingsList.ItemsSource = ViewModel.Simulation.Layers;
            AgentUI.DataContext = ViewModel.Simulation.AgentUiSettings;
        }

        /// <summary>
        /// Gets the vm.
        /// </summary>
        /// <value>The vm.</value>
        public SingularRunnerViewModel ViewModel => (SingularRunnerViewModel)DataContext;

        public void _Click(object sender, RoutedEventArgs args)
        {
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void ShowGeneology_Checked(object sender, RoutedEventArgs args)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    // Managed items go here
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
    }
}
