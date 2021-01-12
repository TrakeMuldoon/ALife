using ALifeUni.ALife.Brains;
using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class GenerationalMazeScenario : BaseScenario
    {
        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical)
            {
                Zone = parentZone,
                TargetZone = targetZone
            };

            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10);

            IShape myShape = new Circle(centrePoint, 5);                       
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "EyeLeft"
                                , new ROEvoNumber(startValue: -20, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                                , new ROEvoNumber(startValue: 10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax: 3, hardMin: 40, hardMax: 120)       //Radius
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),      //Sweep
                new EyeCluster(agent, "EyeRight"
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                                , new ROEvoNumber(startValue: -10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax: 3, hardMin: 40, hardMax: 120)       //Radius
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),      //Sweep
                new ProximityCluster(agent, "Proximity1"
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 4, hardMin: 10, hardMax: 40)),      //Radius
                new GoalSenseCluster(agent, "GoalSense", targetZone)
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue),
                new StatisticInput("MaximumX", 0, Int32.MaxValue),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue),
                new StatisticInput("Iteration", 0, Int32.MaxValue, Iteration)
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
            if(me.Statistics["MaxXTimer"].Value > 1000)
            {
                me.Die();
                return;
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.Zone.Name
                    && me.Statistics["ZoneEscapeTimer"].Value > 200)
                {
                    me.Die();
                    return;
                }
                if(z.Name == me.TargetZone.Name)
                {
                    int successfulSeed = Planet.World.Seed;
                    int turns = Planet.World.Turns;

                    throw new Exception("SUCCESS!!!!!!!!? at " + turns);
                }
            }
        }

        public override void AgentUpkeep(Agent me)
        {
            me.Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);
            me.Statistics["MaxXTimer"].IncreasePropertyBy(1);
            int roundedX = (int)(me.Shape.CentrePoint.X / 100) * 100;
            if(roundedX > me.Statistics["MaximumX"].Value)
            {
                me.Statistics["MaximumX"].Value = roundedX;
                me.Statistics["MaxXTimer"].Value = 0;
            }
        }

        public override void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //foreach(WorldObject wo in collisions)
            //{
            //    if(!(wo is Wall))
            //    {
            //        wo.Die();
            //    }
            //}
            //me.Die();
            //foreach(WorldObject wo in collisions)
            //{
            //    if(wo is Wall)
            //    {
            //        me.Die();
            //        return;
            //    }
            //}
            me.Die();
        }


        public override string Name
        {
            get { return "Maze"; }
        }

        public override int WorldWidth
        {
            get { return 6000; }
        }
        public override int WorldHeight
        {
            get { return 2000; }
        }
        public override bool FixedWidthHeight
        {
            get { return true; }
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

            int numAgents = 30;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
            }

            List<Wall> walls = new List<Wall>();
            walls.Add(new Wall(new Point(260, 410), 850, new Angle(75), "w1-1"));
            walls.Add(new Wall(new Point(260, 1590), 850, new Angle(105), "w1-2"));
            walls.Add(new Wall(new Point(480, 410), 850, new Angle(105), "w1-3"));
            walls.Add(new Wall(new Point(480, 1590), 850, new Angle(75), "w1-4"));

            //i 2
            walls.Add(new Wall(new Point(735, 210), 200, new Angle(80), "w5"));
            walls.Add(new Wall(new Point(735, 510), 200, new Angle(100), "w6"));
            walls.Add(new Wall(new Point(735, 810), 200, new Angle(80), "w7"));
            walls.Add(new Wall(new Point(735, 1110), 200, new Angle(100), "w8"));
            walls.Add(new Wall(new Point(735, 1410), 200, new Angle(80), "w9"));
            walls.Add(new Wall(new Point(735, 1710), 200, new Angle(100), "w10"));

            for(int j = 1; j < 10; j++)
            {
                int yVal = (j * 200) - 30;
                int angleVal = ((j * 30) % 90) - 10;
                walls.Add(new Wall(new Point(900, yVal), 175, new Angle(angleVal), "w3-" + j));
            }

            walls.Add(new Wall(new Point(1100, 390), 800, new Angle(80), "w4-1"));
            walls.Add(new Wall(new Point(1120, 1450), 1100, new Angle(95), "w4-2"));
            walls.Add(new Wall(new Point(1240, 390), 800, new Angle(100), "w4-3"));
            walls.Add(new Wall(new Point(1220, 1450), 1100, new Angle(85), "w4-4"));

            for(int k = 1; k < 20; k++)
            {
                int val = k * 100;
                walls.Add(new Wall(new Point(1400, val), 100, new Angle(350), "w5-" + k));
            }

            walls.Add(new Wall(new Point(1550, 740), 1500, new Angle(85), "w6-1"));
            walls.Add(new Wall(new Point(1560, 1800), 450, new Angle(105), "w6-2"));
            walls.Add(new Wall(new Point(1680, 1800), 450, new Angle(75), "w6-3"));
            walls.Add(new Wall(new Point(1680, 740), 1500, new Angle(95), "w6-4"));


            for(int m = 1; m < 11; m++)
            {
                int val = (m * 200) - 150;
                walls.Add(new Wall(new Point(1810, val), 100, new Angle(340), "w7-" + m));
                walls.Add(new Wall(new Point(1810, val + 80), 100, new Angle(20), "w7-" + m + "_1"));
            }

            for(int n = 1; n < 20; n++)
            {
                int val = (n * 100) - 50;
                walls.Add(new Wall(new Point(1930, val + 25), 100, new Angle(342), "w8-" + n));
                walls.Add(new Wall(new Point(1930, val + 55), 100, new Angle(18), "w8-" + n + "_1"));
            }

            walls.Add(new Wall(new Point(2150, 230), 450, new Angle(85), "w9-1"));
            walls.Add(new Wall(new Point(2143, 1260), 1500, new Angle(92), "w9-2"));
            walls.Add(new Wall(new Point(2198, 1260), 1500, new Angle(88), "w9-3"));
            walls.Add(new Wall(new Point(2190, 230), 450, new Angle(95), "w9-4"));

            foreach(Wall w in walls)
            {
                List<Wall> splitsies = Wall.WallSplitter(w);
                foreach(Wall smallWall in splitsies)
                {
                    Planet.World.AddObjectToWorld(smallWall);
                }
                //Planet.World.AddObjectToWorld(w);
            }

            List<Wall> borderWalls = new List<Wall>()
            {
                new Wall(new Point(3000, 3), 6000, new Angle(0), "bNorth"),
                new Wall(new Point(3000, 1997), 6000, new Angle(0), "bSouth"),
                new Wall(new Point(1, 1000), 2000, new Angle(90), "bSouth"),
            };

            foreach(Wall w in borderWalls)
            {
                Planet.World.AddObjectToWorld(w);
            }
        }

        int bestXNum = 6;
        int Iteration = 1;
        public override void GlobalEndOfTurnActions()
        {
            IEnumerable<Agent> someAgents = Planet.World.AllActiveObjects.OfType<Agent>();

            var LivingAgents =
                from ag in someAgents
                where ag.Alive == true
                select ag;

            if(Planet.World.Turns % 10000 == 0
                || LivingAgents.Count() == 0)
            {
                foreach(Agent aa in someAgents)
                {
                    aa.Die();
                }

                List<Agent> allAgents = new List<Agent>(someAgents);
                IEnumerable<Agent> otherAgents = Planet.World.InactiveObjects.OfType<Agent>();
                allAgents.AddRange(otherAgents);

                List<Agent> bestX = FindTopX<Agent>(bestXNum, allAgents, (ag) => (double)(ag.Shape.CentrePoint.X));

                Zone red = Planet.World.Zones["Red(Blue)"];
                Zone blue = Planet.World.Zones["Blue(Red)"];

                for(int i = 0; i < (80/bestXNum)+1; i++)
                {
                    for(int j = 0; j< bestXNum; j++)
                    {
                        Agent ag = (Agent)bestX[j].Reproduce();
                        ag.Statistics["Iteration"].Value = Iteration;
                    }
                }

                ////This will create a gradually decreasing number of agents. weighted towards whichever agents did best in the last round
                //for(int i = bestXNum; i > 0; i--)
                //{
                //    for(int j = 0; j < i; j++)
                //    {
                //        bestX[bestXNum - i].Reproduce();
                //    }
                //}
                ////for(int k = 0; k < 30; k++)
                ////{
                ////    Agent ag1 = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
                ////}

                //Clear the Refuse
                Planet.World.InactiveObjects.Clear();
                Iteration += 1;
            }
        }

        private static List<T> FindTopX<T>(int topX, List<T> listOfThings, Func<T, double> getValue)
        {
            List<T> winners = new List<T>();
            foreach(T eval in listOfThings)
            {
                if(winners.Count < topX)
                {
                    winners.Add(eval);
                    winners.Sort((a, b) => getValue(b).CompareTo(getValue(a)));
                    continue;
                }

                int j = topX - 1;
                double agCpX = getValue(eval);

                if(getValue(winners[j]) > agCpX)
                {
                    //Not in the best X;
                    continue;
                }

                bool inserted = false;
                for(j--; j > -1; j--)
                {
                    if(getValue(winners[j]) > agCpX)
                    {
                        winners.Insert(j + 1, eval);
                        winners.RemoveAt(topX);
                        inserted = true;
                        break; //break the for loop
                    }
                }
                if(!inserted)
                {
                    //This means it made it through the forloop and is the best.
                    winners.Insert(j + 1, eval);
                    winners.RemoveAt(topX);
                }
            }
            return winners;
        }

        public override void Reset()
        {
            bestXNum = 5;
        }
    }
}
