using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Brains.FlatNeuronBrain;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System.Collections.Generic;

namespace ALife.Core.Scenarios.TestScenarios
{
    public class PerformanceBenchmarkScenario : IScenario
    {
        private readonly int _agentCount;

        public PerformanceBenchmarkScenario(int agentCount)
        {
            _agentCount = agentCount;
        }

        public int WorldWidth => 10000;
        public int WorldHeight => 10000;
        public bool FixedWidthHeight => false;

        public Agent CreateAgentOne(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation)
        {
            Agent agent = new Agent(genusName, AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical);
            agent.HomeZone = parentZone;
            agent.TargetZone = targetZone;

            Point centrePoint = parentZone.Distributor.NextObjectCentre(10, 10);
            IShape myShape = new Circle(centrePoint, 5);
            agent.StartOrientation = startOrientation;
            myShape.Orientation = new Angle(startOrientation);
            myShape.Colour = colour;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "Eye1"
                    , new ReadOnlyEvoNumber(0, 20, -360, 360)
                    , new ReadOnlyEvoNumber(0, 30, -360, 360)
                    , new ReadOnlyEvoNumber(80, 3, 40, 120)
                    , new ReadOnlyEvoNumber(25, 1, 15, 40)),
                new ProximityCluster(agent, "Proximity1"
                    , new ReadOnlyEvoNumber(20, 4, 10, 40))
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>();

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new ColorCluster(agent),
                new MoveCluster(agent, ActionCluster.NullInteraction),
                new RotateCluster(agent, ActionCluster.NullInteraction)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new FlatNeuralNetworkBrain(agent, new List<int> { 7, 9 });
            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public void AgentEndOfTurnTriggers(Agent me)
        {
            // No death - agents always survive
        }

        public void PlanetSetup()
        {
            Zone arena = new Zone("Arena", "random", Colour.Black, new Point(0, 0), WorldWidth, WorldHeight);
            Planet.World.AddZone(arena);

            for(int i = 0; i < _agentCount; i++)
            {
                CreateAgentOne("Agent", arena, null, Colour.Blue, 0);
            }
        }

        public void GlobalEndOfTurnActions()
        {
            // Nothing
        }
    }
}
