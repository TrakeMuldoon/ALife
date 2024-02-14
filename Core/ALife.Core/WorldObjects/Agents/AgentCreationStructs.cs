using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.AgentCreationStructs
{
    public struct AgentCabinet 
    {
        public List<SenseCluster> AgentSenses;
        public List<PropertyInput> AgentProperties;
        public List<StatisticInput> AgentStatistics;
        public List<ActionCluster> AgentActions;
    }

    public struct AgentConstructor
    {
        public string GenusName;
        public Zone ParentZone;
        public Zone TargetZone;
        public Colour AgentColour;
        public Colour? DebugColour;
        public double StartOrientation;
    }

    public struct AgentCreator
    {
        public AgentConstructor BasicInformation;
        public Func<Agent, AgentCabinet> PropertiesCreatorFunction;
        public Func<Agent, IBrain> BrainCreatorFunction;
        public Action<Agent> AgentEndOfTurnActivities;
    }
}
