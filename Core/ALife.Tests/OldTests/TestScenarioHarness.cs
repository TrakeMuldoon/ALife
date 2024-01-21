using ALife.Core;
using ALife.Core.Scenarios;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using System.Drawing;

namespace ALife.Tests.OldTests
{
    internal class TestScenarioHarness : IScenario
    {
        public string Name
        {
            get { return "TestScenario"; }
        }

        public int WorldWidth { get { return 1000; } }

        public int WorldHeight { get { return 1000; } }

        public bool FixedWidthHeight { get { return false; } }

        public void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
        }

        public Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            throw new NotImplementedException();
        }

        public void AgentEndOfTurnTriggers(Agent me)
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
