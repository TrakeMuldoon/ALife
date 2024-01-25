using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.Utility.Collections;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.Scenarios.Mazes
{
    [ScenarioRegistration("Maze",
        description:
        @"
A maze where the agents must attempt to reach the end zone from a starting zone.
Failure cases:
If they crash into each other, or a wall, they die without reproducing.
If they go 600 turns without increasing their X value, they die without reproducing.

Success Cases:
If there are less than 50 agents remaining, 6 more will be added, taken from the best agents to ever live.
If an agent reaches the goal line, the simuluation stops."
    )]
    public class MazeScenario : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical
                                    , parentZone
                                    , targetZone);

            int agentRadius = 5;
            agent.ApplyCircleShapeToAgent(parentZone.Distributor, colour, agentRadius, startOrientation);

            List<SenseCluster> agentSenses = ListHelpers.CompileList(
                new IEnumerable<SenseCluster>[]
                {
                    CommonSenses.PairOfEyes(agent)
                },
                new ProximityCluster(agent, "Proximity1"
                                    , new ReadOnlyEvoNumber(startValue: 20, evoDeltaMax: 4, hardMin: 10, hardMax: 40)), //Radius
                new GoalSenseCluster(agent, "GoalSense", targetZone)
            );


            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("MaximumX", 0, Int32.MaxValue),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, ActionCluster.NullInteraction),
                new RotateCluster(agent, ActionCluster.NullInteraction)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["MaxXTimer"].Value > 600)
            {
                me.Die();
                return;
            }
            int roundedX = (int)(me.Shape.CentrePoint.X / 100) * 100;
            if(roundedX > me.Statistics["MaximumX"].Value)
            {
                me.Statistics["MaximumX"].Value = roundedX;
                me.Statistics["MaxXTimer"].Value = 0;
                if(roundedX % 300 == 0)
                {
                    me.Reproduce();
                }
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.TargetZone.Name)
                {
                    int successfulSeed = Planet.World.Seed;
                    int turns = Planet.World.Turns;

                    throw new Exception("SUCCESS!!!!!!!!? at " + turns);
                }
            }
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth { get { return 6000; } }

        public virtual int WorldHeight { get { return 2000; } }

        public virtual bool FixedWidthHeight { get { return true; } }

        public virtual void PlanetSetup()
        {
            Planet instance = Planet.World;
            double height = instance.WorldHeight;
            double width = instance.WorldWidth;

            Zone red = new Zone("Red(Blue)", "Random", Colour.Red, new Geometry.Shapes.Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(Red)", "Random", Colour.Blue, new Geometry.Shapes.Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;

            instance.AddZone(red);
            instance.AddZone(blue);

            int numAgents = 2;

            for(int i = 0; i < numAgents; i++)
            {
                Colour randomColour = Colour.GetRandomColour(Planet.World.NumberGen);
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, randomColour, 0);
            }
            //MazeRunner mr = new MazeRunner(red, blue);

            MazeSetups.SetUpMaze();
        }

        int bestXNum = 5;
        public virtual void GlobalEndOfTurnActions()
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

            Zone red = Planet.World.Zones["Red(Blue)"];
            Zone blue = Planet.World.Zones["Blue(Red)"];
            if(Planet.World.AllActiveObjects.OfType<Agent>().Count() < 50)
            {
                Planet.World.ReproduceBest();
                Planet.World.ReproduceBest();
                Planet.World.ReproduceBest();
                Colour randomColourA = Colour.GetRandomColour(Planet.World.NumberGen);
                Colour randomColourB = Colour.GetRandomColour(Planet.World.NumberGen);
                Colour randomColourC = Colour.GetRandomColour(Planet.World.NumberGen);
                Agent ag1 = AgentFactory.CreateAgent("Agent", red, blue, randomColourA, 0);
                Agent ag2 = AgentFactory.CreateAgent("Agent", red, blue, randomColourB, 0);
                Agent ag3 = AgentFactory.CreateAgent("Agent", red, blue, randomColourC, 0);

                var weaklings = Planet.World.InactiveObjects.Where((wo) => wo.Shape.CentrePoint.X < 50).ToList();
                foreach(WorldObject wo in weaklings)
                {
                    Planet.World.InactiveObjects.Remove(wo);
                }
            }
        }
    }
}
