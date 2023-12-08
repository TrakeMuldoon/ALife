using ALifeUni.ALife.Agents;
using ALifeUni.ALife.Agents.AgentActions;
using ALifeUni.ALife.Agents.Brains;
using ALifeUni.ALife.Agents.Brains.BehaviourBrains;
using ALifeUni.ALife.Agents.Properties;
using ALifeUni.ALife.Agents.Senses;
using ALifeUni.ALife.Shapes;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class GoalsTestScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public virtual string Name
        {
            get { return "GoalsSenseTest"; }
        }

        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.HomeZone = parentZone;
            agent.TargetZone = targetZone;

            Point centrePoint = parentZone.Distributor.NextObjectCentre(10, 10);

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

        public virtual void AgentUpkeep(Agent me)
        {
            //Do nothing
        }

        public virtual void EndOfTurnTriggers(Agent me)
        {
            //Do Nothing
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //Do Nothing
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth { get { return 800; } }

        public virtual int WorldHeight { get { return 800; } }

        public bool FixedWidthHeight => throw new System.NotImplementedException();

        public virtual void PlanetSetup()
        {
            Zone nullZone = new Zone("Null", "random", Colors.Green, new Point(0, 0), 500, 500);
            Zone blueZone = new Zone("Blue", "random", Colors.Blue, new Point(200, 200), 50, 50);
            Planet.World.AddZone(nullZone);
            Planet.World.AddZone(blueZone);

            Agent a = AgentFactory.CreateAgent("Agent", nullZone, blueZone, Colors.Red, 0);
        }

        public void GlobalEndOfTurnActions()
        {
            //Do Nothing
        }
    }
}
