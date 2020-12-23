using System;
using Windows.UI;

namespace ALifeUni.ALife
{
    public interface IScenario
    {
        Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation);
        void AgentUpkeep(Agent me);

        void EndOfTurnTriggers(Agent me);

        void PlanetSetup();

        void GlobalEndOfTurnActions();
    }
}
