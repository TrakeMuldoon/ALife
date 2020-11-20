using ALifeUni.ALife;
using ALifeUni.ALife.Brains.BehaviourBrains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
                if(theAgent != null)
                {
                    updateInfo();
                }
            }
        }

        public void updateInfo()
        {
            if(theAgent == null)
            {
                return;
            }
            AgentName.Text = theAgent.IndividualLabel;
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
            if(theAgent.myBrain is BehaviourBrain)
            {
                BehaviourBrain brain = (BehaviourBrain)theAgent.myBrain;
                foreach(Behaviour beh in brain.Behaviours)
                {
                    sb.Append(beh.PassedThisTurn ? "!!" : "XX");
                    string behave = beh.AsEnglish;
                    behave = behave.Replace(" THEN", Environment.NewLine + "\\t" + "THEN");
                    sb.Append(" : " + behave + Environment.NewLine);
                }
            }
            else
            {
                sb.Append("unknown brain type");
            }
            BrainDisplay.Text = sb.ToString();
        }

        private void clearInfo()
        {
            AgentName.Text = "xx";
            Senses.Text = "xx";
            Properties.Text = "xx";
            Actions.Text = "xx";
            BrainDisplay.Text = "xx";
        }

        public AgentInfoPanel()
        {
            this.InitializeComponent();
        }



    }
}
