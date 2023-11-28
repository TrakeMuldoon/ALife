﻿using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class ScenarioTemplate : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public virtual string Name => throw new NotImplementedException();

        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            //TODO: Fully Comment This
            throw new NotImplementedException();
        }
        public virtual void AgentUpkeep(Agent me)
        {
            //TODO: Fully Comment This
            //Default, no upkeep
        }

        public virtual void EndOfTurnTriggers(Agent me)
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
