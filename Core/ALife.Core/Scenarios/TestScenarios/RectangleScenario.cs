using ALife.Core.Collision;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using ALife.Core.WorldObjects.Prebuilt;
using System.Collections.Generic;
using ALife.Core.GeometryOld;
using ALife.Core.GeometryOld.Shapes;

namespace ALife.Core.Scenarios.TestScenarios
{
    [ScenarioRegistration("Test - Rectangular Agent", description: "Lorum Ipsum", debugModeOnly: true)]
    public class RectangleScenario : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Colour color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.HomeZone = parentZone;
            agent.TargetZone = targetZone;

            ALife.Core.GeometryOld.Shapes.Point centrePoint = parentZone.Distributor.NextObjectCentre(40, 80);

            IShape myShape = new ALife.Core.GeometryOld.Shapes.Rectangle(centrePoint, 40, 80, color);
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Colour = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "Eye1"
                                , new ReadOnlyEvoNumber(0, 20, -360, 360)  //Orientation Around Parent
                                , new ReadOnlyEvoNumber(0, 30, -360, 360)  //Relative Orientation
                                , new ReadOnlyEvoNumber(80, 3, 40, 120)      //Radius
                                , new ReadOnlyEvoNumber(25, 1, 15, 40)),      //Sweep
                new SquareSenseCluster(agent, "Square")
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>();

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, ActionCluster.NullInteraction),
                new RotateCluster(agent, ActionCluster.NullInteraction)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new TesterBrain(agent);
            IBrain newBrain = new BehaviourBrain(agent);

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
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
            Zone nullZone = new Zone("Null", "random", Colour.Black, new ALife.Core.GeometryOld.Shapes.Point(0, 0), 1000, 1000);
            Planet.World.AddZone(nullZone);

            //Geometry.Shapes.Point ap = new Geometry.Shapes.Point(30, 30);
            //Agent a = AgentFactory.CreateAgent("Agent", nullZone, nullZone, Colour.Red, 0);
            //a.Shape.CentrePoint = ap;

            //Geometry.Shapes.Point bp = new Geometry.Shapes.Point(60, 100);
            //Agent b = AgentFactory.CreateAgent("Agent", nullZone, nullZone, Colour.Red, 0);
            //b.Shape.CentrePoint = bp;

            ALife.Core.GeometryOld.Shapes.Point mp = new ALife.Core.GeometryOld.Shapes.Point(60, 60);
            MazeRunner mr = new MazeRunner(nullZone, nullZone);

            mr.Shape.CentrePoint = mp;

            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[mr.CollisionLevel];
            //collider.MoveObject(b);
            collider.MoveObject(mr);

            Planet.World.AddObjectToWorld(new Wall(new ALife.Core.GeometryOld.Shapes.Point(299, 78), 200, new Angle(35), "wa"));
        }

        public void GlobalEndOfTurnActions()
        {
            //Do Nothing
        }
    }
}
