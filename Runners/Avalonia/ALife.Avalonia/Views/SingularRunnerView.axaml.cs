using System;
using ALife.Avalonia.ViewModels;
using ALife.Rendering;
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
            SetSimulationRunState(true);
            UpdateSimulationSpeedControls();

            //VisualSettingsList.Items.Clear();
            //VisualSettingsList.ItemsSource = TheWorldCanvas.Simulation.Layers;
            //AgentUI.DataContext = TheWorldCanvas.Simulation.AgentUiSettings;
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

        /// <summary>
        /// If the simulation is paused, executes a single turn
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Execution_OneTurnButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.ExecuteTick();
        }

        /// <summary>
        /// Pauses the execution of the simulation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Execution_PauseButton_Click(object sender, RoutedEventArgs args)
        {
            SetSimulationRunState(false);
        }

        /// <summary>
        /// Plays the simulation
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Execution_PlayButton_Click(object sender, RoutedEventArgs args)
        {
            SetSimulationRunState(true);
        }

        /// <summary>
        /// Handles the Click event of the FF_FFButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void FF_FFButton_Click(object sender, RoutedEventArgs args)
        {
            // TODO: take ViewModel.FastForwardTicks and fast foward
        }

        /// <summary>
        /// Handles the Click event of the ReturntoLauncher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ReturntoLauncher_Click(object sender, RoutedEventArgs args)
        {
            MainWindowViewModel? windowMvm = (MainWindowViewModel)Parent.DataContext;
            windowMvm.CurrentViewModel = new LauncherViewModel();
        }

        /// <summary>
        /// Handles the Click event of the Seed_NewSeedButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Seed_NewSeedButton_Click(object sender, RoutedEventArgs args)
        {
            Random r = new();
            Seed.Text = r.Next().ToString();
            Seed_ResetWorldButton_Click(sender, args);
        }

        /// <summary>
        /// Handles the Click event of the Seed_ResetWorldButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Seed_ResetWorldButton_Click(object sender, RoutedEventArgs args)
        {
            if(!int.TryParse(Seed.Text, out int seed))
            {
                // we should never get here (Avalonia's bindings blocks us :) ), but just in case
                Random r = new();
                seed = r.Next();
                Seed.Text = seed.ToString();
            }

            SetSimulationRunState(true);
            TheWorldCanvas.SetSimulationSpeed((int)SimulationSpeed.Normal);
            UpdateSimulationSpeedControls();
            TheWorldCanvas.Simulation.StartingSeed = seed;
            //TheWorldCanvas.StartingSeed = seed;
            TheWorldCanvas.Simulation.InitializeSimulation();
        }

        public void ShowGeneology_Checked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the Click event of the Speed_FastButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_Button_Click(object sender, RoutedEventArgs args)
        {
            Button b = sender as Button;
            TheWorldCanvas.SetSimulationSpeed(int.Parse(b.Content.ToString()));
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Speed_InfiniteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_InfiniteButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed((int)SimulationSpeed.VeryVeryVeryFast);
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Zoom_InButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Zoom_InButton_Click(object sender, RoutedEventArgs args)
        {
            Zoom_Slider.Value += 100;
        }

        /// <summary>
        /// Handles the Click event of the Zoom_OutButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Zoom_OutButton_Click(object sender, RoutedEventArgs args)
        {
            Zoom_Slider.Value -= 100;
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

        /// <summary>
        /// Sets the state of the simulation run.
        /// </summary>
        /// <param name="isRunning">if set to <c>true</c> [is running].</param>
        private void SetSimulationRunState(bool isRunning)
        {
            TheWorldCanvas.IsEnabled = isRunning;
            if(ViewModel != null)
            {
                ViewModel.IsEnabled = isRunning;
            }

            // TODO: Binding for IsEnabled seems like it is delayed by a cycle, so we're manually doing it for now
            Execution_PauseButton.IsEnabled = isRunning;
            Execution_OneTurnButton.IsEnabled = !isRunning;
            Execution_PlayButton.IsEnabled = !isRunning;
        }

        /// <summary>
        /// Updates the simulation speed controls.
        /// </summary>
        private void UpdateSimulationSpeedControls()
        {
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SingularRunnerView() { // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        // Dispose(disposing: false); }
    }
}
