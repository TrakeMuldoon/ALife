using ALife.Core;
using ALife.Core.Scenarios;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;

namespace ALife.Tests.Core.Collision
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

        public Agent CreateAgentOne(string genusName, Zone parentZone, Zone targetZone, Colour color, double startOrientation)
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
