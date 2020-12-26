using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using ALifeUni.ALife.Objects;

namespace ALifeUni.ALife.Scenarios
{
    class MazeScenario : BaseScenario
    {
        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
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
                new EyeCluster(agent, "EyeLeft"
                                , new ROEvoNumber(-30, 20, -360, 360, -360, 360, 0)  //Orientation Around Parent
                                , new ROEvoNumber(10, 30, -360, 360, -360, 360, 0)   //Relative Orientation
                                , new ROEvoNumber(80, 3, 40, 120, 40, 120, 0)        //Radius
                                , new ROEvoNumber(25, 1, 15, 40, 15, 40, 0)),        //Sweep
                new EyeCluster(agent, "EyeRight"
                                , new ROEvoNumber(30, 20, -360, 360, -360, 360, 0)   //Orientation Around Parent
                                , new ROEvoNumber(-10, 30, -360, 360, -360, 360, 0)  //Relative Orientation
                                , new ROEvoNumber(80, 3, 40, 120, 40, 120, 0)        //Radius
                                , new ROEvoNumber(25, 1, 15, 40, 15, 40, 0)),        //Sweep
                new ProximityCluster(agent, "Proximity1")
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                //new ColorCluster(agent),
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public override void EndOfTurnTriggers(Agent me)
        {
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
                    throw new Exception("SUCCESS!!!!!!!!?");
                    ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

                    Point myPoint = me.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
                    me.Shape.CentrePoint = myPoint;
                    collider.MoveObject(me);

                    me.Reproduce();

                    //You have a new countdown
                    me.Statistics["DeathTimer"].Value = 0;
                    me.Statistics["ZoneEscapeTimer"].Value = 0;
                }
            }
        }

        public override void AgentUpkeep(Agent me)
        {
            //Increment or Decrement end of turn values
            me.Statistics["Age"].IncreasePropertyBy(1);
            me.Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);
        }

        public override void PlanetSetup()
        {
            Planet instance = Planet.World;
            double height = instance.WorldHeight;
            double width = instance.WorldWidth;

            Zone red = new Zone("Red(Blue)", "Random", Colors.Red, new Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(Red)", "Random", Colors.Blue, new Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;

            instance.AddZone(red);
            instance.AddZone(blue);

            //int numAgents = 50;
            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
            }

            List<Wall> walls = new List<Wall>();
            walls.Add(new Wall(new Point(260, 410), 850, new Angle(75),     "w1-1"));
            walls.Add(new Wall(new Point(260, 1590), 850, new Angle(105),   "w1-2"));
            walls.Add(new Wall(new Point(480, 410), 850, new Angle(105),    "w1-3"));
            walls.Add(new Wall(new Point(480, 1590), 850, new Angle(75),    "w1-4"));

            walls.Add(new Wall(new Point(735, 200), 200, new Angle(80),     "w5"));
            walls.Add(new Wall(new Point(735, 500), 200, new Angle(100),    "w6"));
            walls.Add(new Wall(new Point(735, 800), 200, new Angle(80),     "w7"));
            walls.Add(new Wall(new Point(735, 1100), 200, new Angle(100),   "w8"));
            walls.Add(new Wall(new Point(735, 1400), 200, new Angle(80),    "w9"));
            walls.Add(new Wall(new Point(735, 1700), 200, new Angle(100),   "w10"));

            walls.Add(new Wall(new Point(900, 200),  175, new Angle(20),    "w11"));
            walls.Add(new Wall(new Point(900, 400),  175, new Angle(50),    "w12"));
            walls.Add(new Wall(new Point(900, 600),  175, new Angle(340),   "w13"));
            walls.Add(new Wall(new Point(900, 800),  175, new Angle(20),    "w14"));
            walls.Add(new Wall(new Point(900, 1000), 175, new Angle(50),    "w15"));
            walls.Add(new Wall(new Point(900, 1200), 175, new Angle(340),   "w16"));
            walls.Add(new Wall(new Point(900, 1400), 175, new Angle(20),    "w17"));
            walls.Add(new Wall(new Point(900, 1600), 175, new Angle(50),    "w18"));
            walls.Add(new Wall(new Point(900, 1800), 175, new Angle(340),   "w19"));

            walls.Add(new Wall(new Point(1100, 390), 800, new Angle(80),    "w20"));
            walls.Add(new Wall(new Point(1120, 1450), 1100, new Angle(95),  "w21"));
            walls.Add(new Wall(new Point(1240, 390), 800, new Angle(100),   "w22"));
            walls.Add(new Wall(new Point(1220, 1450), 1100, new Angle(85),  "w23"));

            walls.Add(new Wall(new Point(1400, 100), 100, new Angle(350),     "w3-1"));
            walls.Add(new Wall(new Point(1400, 200), 100, new Angle(350),     "w3-2"));
            walls.Add(new Wall(new Point(1400, 300), 100, new Angle(350),     "w3-3"));
            walls.Add(new Wall(new Point(1400, 400), 100, new Angle(350),     "w3-4"));
            walls.Add(new Wall(new Point(1400, 500), 100, new Angle(350),     "w3-5"));
            walls.Add(new Wall(new Point(1400, 600), 100, new Angle(350),     "w3-6"));
            walls.Add(new Wall(new Point(1400, 700), 100, new Angle(350),     "w3-7"));
            walls.Add(new Wall(new Point(1400, 800), 100, new Angle(350),     "w3-8"));
            walls.Add(new Wall(new Point(1400, 900), 100, new Angle(350),     "w3-9"));
            walls.Add(new Wall(new Point(1400, 1000), 100, new Angle(350),    "w3-10"));
            walls.Add(new Wall(new Point(1400, 1100), 100, new Angle(350),    "w3-11"));
            walls.Add(new Wall(new Point(1400, 1200), 100, new Angle(350),    "w3-12"));
            walls.Add(new Wall(new Point(1400, 1300), 100, new Angle(350),    "w3-13"));
            walls.Add(new Wall(new Point(1400, 1400), 100, new Angle(350),    "w3-14"));
            walls.Add(new Wall(new Point(1400, 1500), 100, new Angle(350),    "w3-15"));
            walls.Add(new Wall(new Point(1400, 1600), 100, new Angle(350),    "w3-16"));
            walls.Add(new Wall(new Point(1400, 1700), 100, new Angle(350),    "w3-17"));
            walls.Add(new Wall(new Point(1400, 1800), 100, new Angle(350),    "w3-18"));
            walls.Add(new Wall(new Point(1400, 1900), 100, new Angle(350),    "w3-19"));

            walls.Add(new Wall(new Point(1550, 740), 1500, new Angle(85),   "w4-1"));
            walls.Add(new Wall(new Point(1560, 1800), 450, new Angle(105),  "w4-2"));
            walls.Add(new Wall(new Point(1680, 1800), 450, new Angle(75),   "w4-3"));
            walls.Add(new Wall(new Point(1680, 740), 1500, new Angle(95),   "w4-4"));

            walls.Add(new Wall(new Point(1810, 100), 100, new Angle(340),    "w5-1"));
            walls.Add(new Wall(new Point(1810, 180), 100, new Angle(20),     "w5-2"));
            walls.Add(new Wall(new Point(1810, 300), 100, new Angle(340),    "w5-3"));
            walls.Add(new Wall(new Point(1810, 380), 100, new Angle(20),     "w5-4"));
            walls.Add(new Wall(new Point(1810, 500), 100, new Angle(340),    "w5-5"));
            walls.Add(new Wall(new Point(1810, 580), 100, new Angle(20),     "w5-6"));
            walls.Add(new Wall(new Point(1810, 700), 100, new Angle(340),    "w5-7"));
            walls.Add(new Wall(new Point(1810, 780), 100, new Angle(20),     "w5-8"));
            walls.Add(new Wall(new Point(1810, 900), 100, new Angle(340),    "w5-9"));
            walls.Add(new Wall(new Point(1810, 980), 100, new Angle(20),     "w5-10"));
            walls.Add(new Wall(new Point(1810, 1100), 100, new Angle(340),   "w5-11"));
            walls.Add(new Wall(new Point(1810, 1180), 100, new Angle(20),    "w5-12"));
            walls.Add(new Wall(new Point(1810, 1300), 100, new Angle(340),   "w5-13"));
            walls.Add(new Wall(new Point(1810, 1380), 100, new Angle(20),    "w5-14"));
            walls.Add(new Wall(new Point(1810, 1500), 100, new Angle(340),   "w5-15"));
            walls.Add(new Wall(new Point(1810, 1580), 100, new Angle(20),    "w5-16"));
            walls.Add(new Wall(new Point(1810, 1700), 100, new Angle(340),   "w5-17"));
            walls.Add(new Wall(new Point(1810, 1780), 100, new Angle(20),    "w5-18"));
            walls.Add(new Wall(new Point(1810, 1900), 100, new Angle(340),   "w5-19"));
            walls.Add(new Wall(new Point(1810, 1980), 100, new Angle(200),   "w5-20"));

            for(int i = 0; i < 20; i++)
            {
                int val = i * 100;
                walls.Add(new Wall(new Point(1930, val + 23), 100, new Angle(342), "w6-" + val));
                walls.Add(new Wall(new Point(1930, val + 57), 100, new Angle(18),  "w6-" + val + ".1"));
            }


            foreach(Wall w in walls)
            {
                List<Wall> splitsies = Wall.WallSplitter(w);
                foreach(Wall smallWall in splitsies)
                {
                    Planet.World.AddObjectToWorld(smallWall);
                }
                //Planet.World.AddObjectToWorld(w);
            }
        }

        int bestXNum = 5;
        public override void GlobalEndOfTurnActions()
        {
            List<Agent> winners = Planet.World.BestXAgents;
            foreach(Agent ag in Planet.World.AllActiveObjects.OfType<Agent>())
            {
                if(winners.Count < bestXNum)
                {
                    winners.Add(ag);
                    winners.Sort((a, b) => b.Shape.CentrePoint.X.CompareTo(a.Shape.CentrePoint.X));
                    continue;
                }

                int j = bestXNum - 1;
                double agCpX = ag.Shape.CentrePoint.X;

                if(winners[j].Shape.CentrePoint.X > agCpX)
                {
                    //Not in the best X;
                    continue;
                }

                bool inserted = false;
                for(j--; j > -1; j--)
                {
                    if(winners[j].Shape.CentrePoint.X > agCpX)
                    {
                        winners.Insert(j + 1, ag);
                        winners.RemoveAt(bestXNum);
                        inserted = true;
                        break; //break the for loop
                    }
                }
                if(!inserted)
                {
                    //This means it made it through the forloop and is the best.
                    winners.Insert(j + 1, ag);
                    winners.RemoveAt(bestXNum);
                }
            }
        }
    }
}
