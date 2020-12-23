using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    class ZoneRunnerScenario : BaseScenario
    {
        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            return base.CreateAgent(genusName, parentZone, targetZone, color, startOrientation);
        }

        public override void EndOfTurnTriggers(Agent me)
        {
            base.EndOfTurnTriggers(me);
        }

        public override void AgentUpkeep(Agent me)
        {
            base.AgentUpkeep(me);
        }

        public override void PlanetSetup()
        {
            base.PlanetSetup();
        }
    }
}
