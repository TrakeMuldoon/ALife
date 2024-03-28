using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Brains.BehaviourBrains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using Avalonia.Controls;
using System;
using System.Text;

namespace ALife.Avalonia.Controls.SingularRunnerControls
{
    public partial class AgentInfoPanel : Window
    {
        WorldObject worldObject { get; set; }
        Agent theAgent { get; set; }

        public AgentInfoPanel()
        {
            InitializeComponent();
            DataContextChanged += AgentInfoPanel_DataContextChanged;
            Closing += AgentInfoPanel_Closing;
            ZIndex = 999;
        }

        private void AgentInfoPanel_Closing(object? sender, WindowClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void AgentInfoPanel_DataContextChanged(object? sender, System.EventArgs e)
        {
            worldObject = (WorldObject)DataContext;
            NameLabel.Text = worldObject.IndividualLabel;

            if(worldObject is Agent)
            {
                theAgent = (Agent)worldObject;
                updateInfo();
            }
            else
            {
                theAgent = null;
            }
        }

        private void NewAgentSet()
        {
            //bool nnPopupOpen = NeuralNetworkBrainViewer.IsOpen;
            //if(theAgent.MyBrain is NeuralNetworkBrain)
            //{
            //    AgentNeuralBrain.TheAgent = theAgent;
            //    NeuralNetworkBrainViewer.IsOpen = nnPopupOpen;
            //    NeuralNetworkPopupButton.Visibility = Windows.UI.Xaml.Visibility.Visible;

            //}
            //else //Currently there are no other special cases. Eventually this will get unweildy and a new solution will be required.

            //{
            //    NeuralNetworkBrainViewer.IsOpen = false;
            //    NeuralNetworkPopupButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //}
        }

        public void updateInfo()
        {
            if(theAgent == null)
            {
                return;
            }
            AgentName.Text = theAgent.IndividualLabel;
            IShape sh = theAgent.Shape;
            AgentLocation.Text = $"{Math.Round(sh.CentrePoint.X, 4)}, {Math.Round(sh.CentrePoint.Y, 4)}::{sh.Orientation.Degrees}{Environment.NewLine}Generation:{theAgent.Generation} Children: {theAgent.NumChildren}";
            senseBuilder();
            propertiesBuilder();
            actionsBuilder();
            brainBuilder();
        }

        private void senseBuilder()
        {
            StringBuilder sb = new StringBuilder();
            foreach(SenseCluster sc in theAgent.Senses)
            {
                foreach(Input si in sc.SubInputs)
                {
                    sb.Append(si.Name + ": " + si.GetValueAsString() + Environment.NewLine);
                }
            }
            Senses.Text = sb.ToString();
        }

        private void propertiesBuilder()
        {
            StringBuilder sb = new StringBuilder();
            foreach(PropertyInput pi in theAgent.Properties.Values)
            {
                sb.Append(pi.Name + ": " + pi.GetValueAsString() + Environment.NewLine);
            }
            foreach(StatisticInput si in theAgent.Statistics.Values)
            {
                sb.Append(si.Name + ": " + si.GetValueAsString() + Environment.NewLine);
            }
            Properties.Text = sb.ToString();
        }

        private void actionsBuilder()
        {
            StringBuilder sb = new StringBuilder();
            foreach(ActionCluster ac in theAgent.Actions.Values)
            {
                sb.Append(ac.Name + ":" + ac.LastTurnString() + Environment.NewLine);
                foreach(ActionPart ap in ac.SubActions.Values)
                {
                    sb.Append("   " + ap.Name + ": " + ap.IntensityLastTurn + Environment.NewLine);
                }
                Actions.Text = sb.ToString();
            }
        }

        private void brainBuilder()
        {
            StringBuilder sb = new StringBuilder();
            switch(theAgent.MyBrain)
            {
                case BehaviourBrain bb: WriteBehaviourBrainText(bb, sb); break;
                case NeuralNetworkBrain nn: WriteNeuralNetworkBrain(nn, sb); break;
                default: sb.Append("unknown brain type"); break;
            }

            BrainDisplay.Text = sb.ToString();
        }

        private void WriteNeuralNetworkBrain(NeuralNetworkBrain bb, StringBuilder sb)
        {
            sb.Append("Use NeuralNetwork Brain Viewer Button");
        }

        private void WriteBehaviourBrainText(BehaviourBrain bb, StringBuilder sb)
        {
            foreach(Behaviour beh in bb.Behaviours)
            {
                sb.Append(beh.PassedThisTurn ? "!!" : "XX");
                string behave = beh.AsEnglish;
                behave = behave.Replace(" AND", Environment.NewLine + "\t" + "AND");
                behave = behave.Replace(" THEN", Environment.NewLine + "\t\t" + "THEN");
                sb.Append(" : " + behave + Environment.NewLine);
            }
        }

        private void clearInfo()
        {
            AgentName.Text = "xx";
            Senses.Text = "xx";
            Properties.Text = "xx";
            Actions.Text = "xx";
            //BrainDisplay.Text = "xx";
        }
    }
}
