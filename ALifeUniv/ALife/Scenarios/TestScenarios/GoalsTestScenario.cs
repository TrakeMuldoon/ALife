using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Shapes;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class GoalsTestScenario : AbstractScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public override string Name
        {
            get { return "GoalsSenseTest"; }
        }

        /******************/
        /*   AGENT STUFF  */
        /******************/

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

            //IBrain newBrain = new BehaviourBrain(agent, "IF ALWAYS THEN Rotate.TurnRight AT [0.02]"
            //                                             , "IF ALWAYS THEN Move.GoLeft AT [0.9]");
            IBrain newBrain = new BehaviourBrain(agent, "IF Goals.RelativeAngle.Value GreaterThan [0] THEN Rotate.TurnRight AT [0.04]"
                                                        , "IF Goals.RelativeAngle.Value LessThan [0] THEN Rotate.TurnLeft AT [0.04]"
                                                        , "IF Goals.RelativeAngle.Value AbsLessThan [10] THEN Move.GoForward AT [0.5]");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public override int WorldWidth { get { return 800; } }

        public override int WorldHeight { get { return 800; } }

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
