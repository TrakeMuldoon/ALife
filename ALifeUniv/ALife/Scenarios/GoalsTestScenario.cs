using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Objects;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using ALifeUni.ALife.WorldObjects;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    class GoalsTestScenario : BaseScenario
    {
        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.Zone = parentZone;
            agent.TargetZone = targetZone;

            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10);

            IShape myShape = new Circle(centrePoint, 5);
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new GoalSenseCluster(agent, "Goals", targetZone)
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>();

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(agent, "IF ALWAYS THEN Rotate.TurnRight AT [0.02]"
                                                      , "IF ALWAYS THEN Move.GoLeft AT [0.9]");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public override void EndOfTurnTriggers(Agent me)
        {

        }

        public override void AgentUpkeep(Agent me)
        {

        }

        public override void PlanetSetup()
        {
            Zone nullZone = new Zone("Null", "random", Colors.Green, new Point(0, 0), 500, 500);
            Zone blueZone = new Zone("Blue", "random", Colors.Blue, new Point(200, 200), 50, 50);
            Planet.World.AddZone(nullZone);
            Planet.World.AddZone(blueZone);

            Agent a = AgentFactory.CreateAgent("Agent", nullZone, blueZone, Colors.Red, 0);
        }
    }
}
