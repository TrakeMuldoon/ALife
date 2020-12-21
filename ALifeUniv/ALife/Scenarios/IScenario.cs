using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ALifeUni.ALife
{
    public interface IScenario
    {
        Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation);
        void AgentUpkeep(Agent me);

        void EndOfTurnTriggers(Agent me);

        void PlanetSetup();
    }
}
