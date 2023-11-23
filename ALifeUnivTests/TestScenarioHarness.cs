using ALifeUni.ALife;
using ALifeUni.ALife.Scenarios;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUnivTests
{
    class TestScenarioHarness : IScenario
    {
        public string Name
        {
            get { return "TestScenario"; }
        }

        public int WorldWidth { get { return 1000; } }

        public int WorldHeight { get { return 1000; } }

        public bool FixedWidthHeight { get { return false; } }

        public void AgentUpkeep(Agent me) { }

        public void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
        }

        public Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            throw new NotImplementedException();
        }

        public void EndOfTurnTriggers(Agent me)
        {
        }

        public void GlobalEndOfTurnActions()
        {
        }

        public void PlanetSetup()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
        }
    }
}
