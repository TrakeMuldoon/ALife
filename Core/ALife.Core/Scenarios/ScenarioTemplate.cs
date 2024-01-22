using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ALife.Core.Scenarios
{
/*
    [ScenarioRegistration("[ScenarioName]",
description:
    @"
[Scenario description]

Failure cases:
[What kills an agent]

Success Cases:
[What causes an agent to reproduce]
")]
*/
    //[SuggestedSeed([SeedNumber], "Seed Description")]
    public class ScenarioTemplate : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color colour, double startOrientation)
        {
            //TODO: Fully Comment This
            throw new NotImplementedException();
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            //TODO: Fully Comment This
            //Default, nothing happens
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //TODO: Fully Comment This
            //Default, nothing
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        //TODO: Fully Comment This
        public virtual int WorldWidth => throw new NotImplementedException();

        //TODO: Fully Comment This
        public virtual int WorldHeight => throw new NotImplementedException();

        //TODO: Fully Comment This
        public virtual bool FixedWidthHeight
        {
            get { return false; }
        }

        //TODO: Fully Comment This
        public virtual void PlanetSetup()
        {
            throw new NotImplementedException();
        }

        //TODO: Fully Comment This
        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
