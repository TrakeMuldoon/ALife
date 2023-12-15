using ALifeUni.ALife.Scenarios;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains.BehaviourBrains;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System.Text;
using System;
using ALifeUni.ALife.WorldObjects.Agents;
using System.Collections.Generic;
using ALifeUni.ALife.Shapes;

namespace ALifeUni.ALife.ImportExport
{
    public static class AgentCodeSerializer 
    {
        public static string ExportAgentDefinitionAsCode(Agent theAgent)
        {
            int seed = Planet.World.Seed;
            string scenarioName = ScenarioRegister.GetScenarioDetails(Planet.World.Scenario.GetType()).Name;
            int turns = Planet.World.Turns;

            string agentName = $"{scenarioName}({seed}):{turns}";
            return ExportAgentDefinitionAsCode(theAgent, agentName);
        }

        public static string ExportAgentDefinitionAsCode(Agent theAgent, string name)
        {

            StringBuilder outputCode = new StringBuilder();
            outputCode.AppendLine(@"
int agentRadius = 5;
ApplyCircleShapeToAgent(parentZone.Distributor, Colors.Blue, agentRadius, 0);");

            outputCode.AppendLine();
            outputCode.AppendLine("List<SenseCluster> agentSenses = new List<SenseCluster>()");
            outputCode.AppendLine("{");
            foreach(SenseCluster sc in theAgent.Senses)
            {
                switch(sc)
                {
                    case EyeCluster eye: outputCode.AppendLine(EyeClusterDefinitionAsCode(eye)); break;
                    case GoalSenseCluster gsc: outputCode.AppendLine(GoalSenseClusterDefinitionAsCode(gsc)); break;
                    case ProximityCluster pc: outputCode.AppendLine(ProximityClusterDefinitionAsCode(pc)); break;
                    default: throw new NotImplementedException($"Unable to export: {sc.GetType().Name}");
                }
            }
            outputCode.AppendLine("}");

            outputCode.AppendLine();
            if(theAgent.Properties.Count > 0)
            {
                throw new NotImplementedException($"Unable to export agents with Properties. It's honestly never come up yet.");
            }
            else
            {
                outputCode.AppendLine("List<PropertyInput> agentProperties = new List<PropertyInput>();");
            }

            outputCode.AppendLine();
            outputCode.AppendLine("List<StatisticInput> agentStatistics = new List<StatisticInput>()");
            outputCode.AppendLine("{");
            foreach(StatisticInput si in theAgent.Statistics.Values)
            {
                string nextLine = $"\tnew StatisticInput(\"{si.Name}\", {si.StatisticMinimum}, {si.StatisticMaximum}, {si.StartValue}, {si.Disposition});";
                outputCode.AppendLine(nextLine);
            }
            outputCode.AppendLine("}");

            outputCode.AppendLine();
            outputCode.AppendLine("List<ActionCluster> agentActions = new List<ActionCluster>()");
            outputCode.AppendLine("{");
            foreach(ActionCluster ac in theAgent.Actions.Values)
            {
                outputCode.AppendLine($"new {ac.GetType().Name}(this);");
            }
            outputCode.AppendLine("}");

            outputCode.AppendLine();
            outputCode.AppendLine("this.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);");
            outputCode.AppendLine();
            outputCode.AppendLine("IBrain newBrain =");

            switch(theAgent.MyBrain)
            {
                case NeuralNetworkBrain nnb: outputCode.AppendLine(OutputNeuralNetworkBrainAsCode(nnb)); break;
                case BehaviourBrain bb: outputCode.AppendLine(OutputBehaviourBrainAsCode(bb)); break;
                default: throw new NotImplementedException($"Unable to export: {theAgent.MyBrain.GetType().Name}");
            }

            outputCode.AppendLine("this.CompleteInitialization(null, 1, newBrain);");
            outputCode.AppendLine();

            return outputCode.ToString();
        }

        private static string OutputBehaviourBrainAsCode(BehaviourBrain myBrain)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\tnew BehaviourBrain(this,");

            foreach(Behaviour beh in myBrain.Behaviours)
            {
                string behave = beh.AsEnglish;
                behave = behave.Replace(" AND", $"\" +{Environment.NewLine}\t\t\t\"AND");
                behave = behave.Replace(" THEN", $"\" +{Environment.NewLine}\t\t\t\t\"THEN");
                sb.AppendLine($"\t\t\"{behave}\",");
            }

            string finalBrain = sb.ToString().TrimEnd(',');
            return $"{finalBrain}{Environment.NewLine}\t}}";
        }

        private static string OutputNeuralNetworkBrainAsCode(IBrain myBrain)
        {
            throw new NotImplementedException();
        }

        private static string GoalSenseClusterDefinitionAsCode(GoalSenseCluster gsc)
        {
            string result;
            switch(gsc.TargetShape)
            {
                case AARectangle aar: result = $"new GoalSenseCluster(agent, \"{gsc.Name}\", targetZone)"; break;
                default: throw new NotImplementedException($"Cannot have a target of shape: {gsc.TargetShape.GetType()}");
            }

            return result;
        }

        private static string EyeClusterDefinitionAsCode(EyeCluster eye)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> eyeProperties = eye.ExportEvoNumbersAsCode();

            sb.AppendLine($"new EyeCluster(this, \"{eye.Name}\"");
            sb.AppendLine($"\t{eyeProperties["OrientationAroundParent"]}");
            sb.AppendLine($"\t{eyeProperties["RelativeOrientation"]}");
            sb.AppendLine($"\t{eyeProperties["Radius"]}");
            sb.AppendLine($"\t{eyeProperties["Sweep"]}),");

            return sb.ToString();

        }

        private static string ProximityClusterDefinitionAsCode(ProximityCluster pc)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> pcProperties = pc.ExportEvoNumbersAsCode();

            sb.AppendLine($"new ProximityCluster(this, {pc.Name}");
            sb.AppendLine($"\t{pcProperties["Radius"]}");

            return sb.ToString();
        }

    }
}