using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife
{
    public interface IScenario
    {

        /****************/
        /* AGENT STUFF */
        /****************/

        Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation);

        void AgentUpkeep(Agent me);

        void EndOfTurnTriggers(Agent me);

        void CollisionBehaviour(Agent me, List<WorldObject> collisions);

        /****************/
        /* PLANET STUFF */
        /****************/

        string Name
        {
            get;
        }

        int WorldWidth
        {
            get;
        }
        int WorldHeight
        {
            get;
        }
        bool FixedWidthHeight
        {
            get;
        }

        void PlanetSetup();

        void GlobalEndOfTurnActions();

        void Reset();
    }
}
