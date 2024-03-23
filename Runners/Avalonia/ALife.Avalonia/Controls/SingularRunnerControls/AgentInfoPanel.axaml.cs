using ALife.Core.WorldObjects;
using Avalonia.Controls;

namespace ALife.Avalonia.Controls.SingularRunnerControls
{
    public partial class AgentInfoPanel : Window
    {
        WorldObject worldObject { get; set; }
        public AgentInfoPanel()
        {
            InitializeComponent();
            DataContextChanged += AgentInfoPanel_DataContextChanged;
            Closing += AgentInfoPanel_Closing;
        }

        private void AgentInfoPanel_Closing(object? sender, WindowClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void AgentInfoPanel_DataContextChanged(object? sender, System.EventArgs e)
        {
            worldObject = (WorldObject)DataContext;

            AgentName.Content = worldObject.IndividualLabel;
        }
    }
}
