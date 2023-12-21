using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System.Collections.Generic;
using Windows.UI;


namespace ALifeUni.ALife.WorldObjects.Prebuilt
{
    public class ExportedTestAgent : Agent
    {
        public ExportedTestAgent(Zone parentZone, Zone targetZone) : base("ExportedTestAgent", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {


    }
}
}

