using ALifeUni.ALife;
using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Shapes;
using System;
using System.Text;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni
{
    public sealed partial class AgentInfoPanel : UserControl
    {
        private Agent theAgent;
        public Agent TheAgent
        {
            get => theAgent;
            set
            {
                theAgent = value;
                clearInfo();
                NeuralNetworkBrainViewer.IsOpen = false;
                NeuralNetworkPopupButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                if(theAgent != null)
                {
                    updateInfo();
                }
            }
        }

        public AgentInfoPanel()
        {
            this.InitializeComponent();
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
                case NeuralNetworkBrain nn: PrepareNeuralNetworkBrain(nn, sb); break;
                default: sb.Append("unknown brain type"); break;
            }

            BrainDisplay.Text = sb.ToString();
        }

        private void PrepareNeuralNetworkBrain(NeuralNetworkBrain bb, StringBuilder sb)
        {
            sb.Append("Use NeuralNetwork Brain Viewer Button");
            NeuralNetworkPopupButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
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
            BrainDisplay.Text = "xx";
        }


        private void NeuralNetworkPopupButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NeuralNetworkBrainViewer.IsOpen = !NeuralNetworkBrainViewer.IsOpen;
        }
    }
}
