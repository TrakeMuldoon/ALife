using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class GenerationalMazeScenario : AbstractScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public override string Name
        {
            get { return "Generational Maze"; }
        }

        /******************/
        /*   AGENT STUFF  */
        /******************/

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
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 9, 9 });
            //            IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public override void EndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["MaxXTimer"].Value > 600)
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

            me.Die();
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

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

            MazeScenarioHelpers.SetUpMaze();
        }

        int bestXNum = 4;
        int Iteration = 1;
        Agent bestEver;
        public override void GlobalEndOfTurnActions()
        {
            IEnumerable<Agent> someAgents = Planet.World.AllActiveObjects.OfType<Agent>();

            var LivingAgents =
                from ag in someAgents
                where ag.Alive == true
                select ag;
            int living = LivingAgents.Count();

            if(Planet.World.Turns % 10000 == 0
                || living == 0)
            {
                foreach(Agent aa in someAgents)
                {
                    aa.Die();
                }

                List<Agent> allAgents = new List<Agent>(someAgents);
                IEnumerable<Agent> otherAgents = Planet.World.InactiveObjects.OfType<Agent>();
                allAgents.AddRange(otherAgents);

                double averageX = allAgents.Average((ag) => ag.Shape.CentrePoint.X);
                double maxX = allAgents.Max((ag) => ag.Shape.CentrePoint.X);

                String generationString = String.Format("Gen {0}: Stragglers: {1} Avg: {2:0.000}, MaxX: {3:0}", Iteration, living, averageX, maxX);
                Planet.World.MessagePump.Add(generationString);

                List<Agent> bestX = FindTopX<Agent>(bestXNum, allAgents, (ag) => (double)(ag.Shape.CentrePoint.X));

                Zone red = Planet.World.Zones["Red(Blue)"];
                Zone blue = Planet.World.Zones["Blue(Red)"];

                if(bestEver == null
                    || bestEver.Shape.CentrePoint.X < bestX[0].Shape.CentrePoint.X)
                {
                    bestEver = bestX[0];
                }
                else
                {
                    bestX.Insert(0, bestEver);
                }

                for(int i = 0; i < (60 / bestXNum) + 1; i++)
                {
                    for(int j = 0; j < bestXNum; j++)
                    {
                        Agent ag = (Agent)bestX[j].Reproduce();
                        ag.Statistics["Iteration"].Value = Iteration;
                    }
                }

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
            bestXNum = 4;
            Iteration = 1;
            bestEver = null;
        }
    }
}
