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
    class BaseScenario : IScenario
    {
        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.Zone = parentZone;
            agent.TargetZone = targetZone;

            //TODO: FIX SO THAT THE SHAPE IS A PARAMETER
            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10); //TODO: HARDCODED AGENT RADIUS

            IShape myShape = new Circle(centrePoint, 5);                        //TODO: HARDCODED AGENT RADIUS
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "Eye1"
                                , new ROEvoNumber(0, 20, -360, 360, -360, 360, 0)  //Orientation Around Parent
                                , new ROEvoNumber(0, 30, -360, 360, -360, 360, 0)  //Relative Orientation
                                , new ROEvoNumber(80, 3, 40, 120, 40, 120, 0)      //Radius
                                , new ROEvoNumber(25, 1, 15, 40, 15, 40, 0)),      //Sweep
                new ProximityCluster(agent, "Proximity1")
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                //new ColorCluster(agent),
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(agent, "IF Age.Value GreaterThan [10] THEN Move.GoForward AT [0.2]", "*", "*", "*", "*");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void EndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["DeathTimer"].Value > 1899)
            {
                me.Die();
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.Zone.Name)
                {
                    if(me.Statistics["ZoneEscapeTimer"].Value > 200)
                    {
                        me.Die();
                    }
                }
                else if(z.Name == me.TargetZone.Name)
                {
                    ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

                    Point myPoint = me.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
                    me.Shape.CentrePoint = myPoint;
                    collider.MoveObject(me);

                    me.Reproduce();
                    //foreach(Zone zon in Planet.World.Zones.Values)
                    //{
                    //    if(zon.Name != Zone.Name)
                    //    {
                    //        //Reproduce(zon, zon.OppositeZone, new Angle(zon.OrientationDegrees), zon.OppositeZone.Color);
                    //    }
                    //}

                    //You have a new countdown
                    me.Statistics["DeathTimer"].Value = 0;
                    me.Statistics["ZoneEscapeTimer"].Value = 0;
                }
            }
        }

        public virtual void AgentUpkeep(Agent me)
        {
            //Increment or Decrement end of turn values
            me.Statistics["Age"].IncreasePropertyBy(1);
            me.Statistics["DeathTimer"].IncreasePropertyBy(1);
            me.Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);
        }

        public virtual void PlanetSetup()
        {
            Planet instance = Planet.World;
            double height = instance.WorldHeight;
            double width = instance.WorldWidth;

            Zone red = new Zone("Red(Blue)", "Random", Colors.Red, new Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(Red)", "Random", Colors.Blue, new Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;
            blue.OppositeZone = red;
            blue.OrientationDegrees = 180;

            Zone green = new Zone("Green(Orange)", "Random", Colors.Green, new Point(0, 0), width, 100);
            Zone orange = new Zone("Orange(Green)", "Random", Colors.Orange, new Point(0, height - 40), width, 40);
            green.OppositeZone = orange;
            green.OrientationDegrees = 90;
            orange.OppositeZone = green;
            orange.OrientationDegrees = 270;

            instance.AddZone(red);
            instance.AddZone(blue);
            instance.AddZone(green);
            instance.AddZone(orange);

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
                Agent bag = AgentFactory.CreateAgent("Agent", blue, red, Colors.Red, 180);
                Agent gag = AgentFactory.CreateAgent("Agent", green, orange, Colors.Orange, 90);
                Agent oag = AgentFactory.CreateAgent("Agent", orange, green, Colors.Green, 270);
            }

            Point rockCP = new Point((width / 2) + (width / 15), height / 2);
            Circle cir = new Circle(rockCP, 30);
            FallingRock fr = new FallingRock(rockCP, cir, Colors.Black);
            instance.AddObjectToWorld(fr);

            //Point rockRCP = new Point((width / 2), (height / 2) - (height / 10));
            //Rectangle rec = new Rectangle(rockRCP, 80, 40, Colors.Black);
            //FallingRock frR = new FallingRock(rockRCP, rec, Colors.Black);
            //instance.AddObjectToWorld(frR);
        }
    }
}
