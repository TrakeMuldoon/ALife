using ALifeUni.ALife.Collision;
using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.Utility.WorldObjects;
using ALifeUni.ALife.WorldObjects.Agents;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Brains.BehaviourBrains;
using ALifeUni.ALife.WorldObjects.Agents.CustomAgents;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    [ScenarioRegistration("Rectangular Agent Test", description: "Lorum Ipsum", debugModeOnly: true)]
    public class RectangleScenario : IScenario
    {
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

            Point centrePoint = parentZone.Distributor.NextObjectCentre(40, 80);

            IShape myShape = new Rectangle(centrePoint, 40, 80, color);
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "Eye1"
                                , new ROEvoNumber(0, 20, -360, 360)  //Orientation Around Parent
                                , new ROEvoNumber(0, 30, -360, 360)  //Relative Orientation
                                , new ROEvoNumber(80, 3, 40, 120)      //Radius
                                , new ROEvoNumber(25, 1, 15, 40)),      //Sweep
                new SquareSenseCluster(agent, "Square")
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>();

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new TesterBrain(agent);
            IBrain newBrain = new BehaviourBrain(agent);

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentUpkeep(Agent me)
        {
            //Do nothing
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
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

        public bool FixedWidthHeight => true;

        public virtual void PlanetSetup()
        {
            Zone nullZone = new Zone("Null", "random", Colors.Black, new Point(0, 0), 1000, 1000);
            Planet.World.AddZone(nullZone);

            //Point ap = new Point(30, 30);
            //Agent a = AgentFactory.CreateAgent("Agent", nullZone, nullZone, Colors.Red, 0);
            //a.Shape.CentrePoint = ap;

            //Point bp = new Point(60, 100);
            //Agent b = AgentFactory.CreateAgent("Agent", nullZone, nullZone, Colors.Red, 0);
            //b.Shape.CentrePoint = bp;

            Point mp = new Point(60, 60);
            MazeRunner mr = new MazeRunner(nullZone, nullZone);

            mr.Shape.CentrePoint = mp;

            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[mr.CollisionLevel];
            //collider.MoveObject(b);
            collider.MoveObject(mr);

            Planet.World.AddObjectToWorld(new Wall(new Point(299, 78), 200, new Angle(35), "wa"));
        }

        public void GlobalEndOfTurnActions()
        {
            //Do Nothing
        }
    }
}
