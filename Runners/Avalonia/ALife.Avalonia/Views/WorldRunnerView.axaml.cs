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
    public partial class WorldRunnerView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldRunnerView"/> class.
        /// </summary>
        public WorldRunnerView()
        {
            InitializeComponent();
            SetSimulationRunState(true);
            UpdateSimulationSpeedControls();

            VisualSettingsList.Items.Clear();
            VisualSettingsList.ItemsSource = TheWorldCanvas.Simulation.Layers;
            AgentUI.DataContext = TheWorldCanvas.Simulation.AgentUiSettings;
        }

        /// <summary>
        /// Gets the vm.
        /// </summary>
        /// <value>The vm.</value>
        public WorldRunnerViewModel ViewModel => (WorldRunnerViewModel)DataContext;

        public void _Click(object sender, RoutedEventArgs args)
        {
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
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.Normal);
            UpdateSimulationSpeedControls();
            TheWorldCanvas.StartingSeed = seed;
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
        public void Speed_FastButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.Fast);
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Speed_NormalButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_NormalButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.Normal);
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Speed_SlowButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_SlowButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.Slow);
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Speed_VeryFastButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_VeryFastButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.VeryFast);
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Speed_VerySlowButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_VerySlowButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.VerySlow);
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Speed_VeryVeryFastButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_VeryVeryFastButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.VeryVeryFast);
            UpdateSimulationSpeedControls();
        }

        /// <summary>
        /// Handles the Click event of the Speed_VeryVeryVeryFastButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Speed_VeryVeryVeryFastButton_Click(object sender, RoutedEventArgs args)
        {
            TheWorldCanvas.SetSimulationSpeed(SimulationSpeed.VeryVeryVeryFast);
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
            // TODO: Binding for IsEnabled seems like it is delayed by a cycle, so we're manually doing it for now
            Speed_FastButton.IsEnabled = true;
            Speed_NormalButton.IsEnabled = true;
            Speed_SlowButton.IsEnabled = true;
            Speed_VeryFastButton.IsEnabled = true;
            Speed_VeryVeryFastButton.IsEnabled = true;
            Speed_VeryVeryVeryFastButton.IsEnabled = true;
            Speed_VerySlowButton.IsEnabled = true;

            switch(TheWorldCanvas.SimulationSpeed)
            {
                case SimulationSpeed.Fast:
                    Speed_FastButton.IsEnabled = false;
                    break;

                case SimulationSpeed.Normal:
                    Speed_NormalButton.IsEnabled = false;
                    break;

                case SimulationSpeed.Slow:
                    Speed_SlowButton.IsEnabled = false;
                    break;

                case SimulationSpeed.VeryFast:
                    Speed_VeryFastButton.IsEnabled = false;
                    break;

                case SimulationSpeed.VerySlow:
                    Speed_VerySlowButton.IsEnabled = false;
                    break;

                case SimulationSpeed.VeryVeryFast:
                    Speed_VeryVeryFastButton.IsEnabled = false;
                    break;

                case SimulationSpeed.VeryVeryVeryFast:
                    Speed_VeryVeryVeryFastButton.IsEnabled = false;
                    break;
            }
        }
    }
}
