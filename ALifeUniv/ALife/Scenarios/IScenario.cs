using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife
{
    public interface IScenario
    {
        Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation);

        void AgentUpkeep(Agent me);

        void EndOfTurnTriggers(Agent me);

        void CollisionBehaviour(Agent me, List<WorldObject> collisions);

        void PlanetSetup();

        void GlobalEndOfTurnActions();
    }
}
