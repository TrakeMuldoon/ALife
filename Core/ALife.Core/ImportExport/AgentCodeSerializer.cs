using System.Text;
using System;
using System.Collections.Generic;
using ALife.Core;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Scenarios;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Brains.BehaviourBrains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;

namespace ALife.Core.ImportExport
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
            outputCode.AppendLine("int agentRadius = 5;");
            outputCode.AppendLine("ApplyCircleShapeToAgent(parentZone.Distributor, Colour.Blue, agentRadius, 0);");

            outputCode.AppendLine();
            outputCode.AppendLine("List<SenseCluster> agentSenses = new List<SenseCluster>()");
            outputCode.AppendLine("{");
            bool first = true;
            foreach(SenseCluster sc in theAgent.Senses)
            {
                if(!first)
                {
                    outputCode.Append(", ");
                }
                switch(sc)
                {
                    case EyeCluster eye: outputCode.AppendLine(EyeClusterDefinitionAsCode(eye)); break;
                    case GoalSenseCluster gsc: outputCode.AppendLine(GoalSenseClusterDefinitionAsCode(gsc)); break;
                    case ProximityCluster pc: outputCode.AppendLine(ProximityClusterDefinitionAsCode(pc)); break;
                    default: throw new NotImplementedException($"Unable to export: {sc.GetType().Name}");
                }
                first = false;
            }
            outputCode.AppendLine("};");

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
            first = true;
            foreach(StatisticInput si in theAgent.Statistics.Values)
            {
                string prepend = first ? "\t " : "\t, ";
                string nextLine = $"new StatisticInput(\"{si.Name}\", {si.StatisticMinimum}, {si.StatisticMaximum}, StatisticInputType.{si.Disposition}, {si.StartValue})";
                outputCode.AppendLine($"{prepend}{nextLine}");
                first = false;
            }
            outputCode.AppendLine("};");

            outputCode.AppendLine();
            outputCode.AppendLine("List<ActionCluster> agentActions = new List<ActionCluster>()");
            outputCode.AppendLine("{");
            first = true;
            foreach(ActionCluster ac in theAgent.Actions.Values)
            {
                string prepend = first ? "\t " : "\t, ";
                outputCode.AppendLine($"{prepend}new {ac.GetType().Name}(this)");
                first = false;
            }
            outputCode.AppendLine("};");

            outputCode.AppendLine();
            outputCode.AppendLine("this.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);");
            outputCode.AppendLine();
            outputCode.Append("IBrain newBrain = ");

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

            string brainString = sb.ToString();
            string trimBrainString = brainString.Substring(0,brainString.LastIndexOf(','));
            return $"{trimBrainString});{Environment.NewLine}\t";
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
                case AARectangle aar: result = $"new GoalSenseCluster(this, \"{gsc.Name}\", targetZone)"; break;
                default: throw new NotImplementedException($"Cannot have a target of shape: {gsc.TargetShape.GetType()}");
            }

            return result;
        }

        private static string EyeClusterDefinitionAsCode(EyeCluster eye)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> eyeProperties = eye.ExportEvoNumbersAsCode();

            sb.AppendLine($"new EyeCluster(this, \"{eye.Name}\", {eye.IncludeColor.ToString().ToLower()}");
            sb.AppendLine($"\t{eyeProperties["OrientationAroundParent"]}");
            sb.AppendLine($"\t{eyeProperties["RelativeOrientation"]}");
            sb.AppendLine($"\t{eyeProperties["Radius"]}");
            sb.AppendLine($"\t{eyeProperties["Sweep"]}");
            sb.AppendLine(")");
            return sb.ToString();
        }

        private static string ProximityClusterDefinitionAsCode(ProximityCluster pc)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> pcProperties = pc.ExportEvoNumbersAsCode();

            sb.AppendLine($"new ProximityCluster(this, \"{pc.Name}\"");
            sb.AppendLine($"\t{pcProperties["Radius"]}");
            sb.AppendLine(")");
            return sb.ToString();
        }

    }
}
