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
                        Planet.World.ReproduceBest();
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

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                //Agent rag = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
            }

            List<Wall> walls = new List<Wall>();
            walls.Add(new Wall(new Point(260, 450), 1000, new Angle(75), "w1"));
            walls.Add(new Wall(new Point(260, 1550), 1000, new Angle(105), "w2"));
            walls.Add(new Wall(new Point(520, 450), 1000, new Angle(105), "w3"));
            walls.Add(new Wall(new Point(520, 1550), 1000, new Angle(75), "w4"));

            foreach(Wall w in walls)
            {
                Planet.World.AddObjectToWorld(w);
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
