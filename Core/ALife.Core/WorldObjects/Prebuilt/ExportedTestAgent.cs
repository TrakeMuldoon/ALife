﻿using ALife.Core.WorldObjects.Agents;


namespace ALife.Core.WorldObjects.Prebuilt
{
    public class ExportedTestAgent : Agent
    {
        public ExportedTestAgent(Zone parentZone, Zone targetZone) : base("ExportedTestAgent", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {


        }
    }
}

