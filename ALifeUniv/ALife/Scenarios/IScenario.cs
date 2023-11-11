using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public interface IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        string Name
        {
            get;
        }

        /******************/
        /*   AGENT STUFF  */
        /******************/

        Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation);

        void AgentUpkeep(Agent me);

        void EndOfTurnTriggers(Agent me);

        void CollisionBehaviour(Agent me, List<WorldObject> collisions);

        /******************/
        /*  PLANET STUFF  */
        /******************/

        int WorldWidth
        {
            get;
        }
        int WorldHeight
        {
            get;
        }
        /* It's not clear what I was thinking when I created this. Re-evaluate at a later date */
        bool FixedWidthHeight
        {
            get;
        }

        void PlanetSetup();

        void GlobalEndOfTurnActions();


        /* This is called when the scenario is reset, any variables being tracked inside the Scenario itself must be reset */
        void Reset();
    }
}
