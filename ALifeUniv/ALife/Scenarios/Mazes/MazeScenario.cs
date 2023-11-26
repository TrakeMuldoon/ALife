﻿/***
 * Scenario: 
 * A maze where the agents must attempt to reach the end zone from a starting zone.
 * Failure cases:
 * If they crash into each other, or a wall, they die without reproducing.
 * If they go 600 turns without increasing their X value, they die without reproducing.
 * 
 * Success Cases:
 * If there are less than 50 agents remaining, 6 more will be added, taken from the best agents to ever live.
 * If an agent reaches the goal line, the simuluation stops.
 * **/

using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class MazeScenario : AbstractScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public override string Name
        {
            get { return "Maze"; }
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
                                , new ROEvoNumber(startValue: -30, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                                , new ROEvoNumber(startValue: 10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Relative Orientation
                                , new ROEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40, hardMax: 120)        //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),       //Sweep
                new EyeCluster(agent, "EyeRight"
                                , new ROEvoNumber(startValue: 30, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                                , new ROEvoNumber(startValue: -10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Relative Orientation
                                , new ROEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40, hardMax: 120)        //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),       //Sweep
                new ProximityCluster(agent, "Proximity1"
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 4, hardMin: 10, hardMax: 40)),        //Radius
                new GoalSenseCluster(agent, "GoalSense", targetZone)
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("MaximumX", 0, Int32.MaxValue),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
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
            if(me.Statistics["MaxXTimer"].Value > 600)
            {
                me.Die();
                return;
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

        public override void AgentUpkeep(Agent me)
        {
            //Increment or Decrement end of turn values
            me.Statistics["Age"].IncreasePropertyBy(1);
            me.Statistics["MaxXTimer"].IncreasePropertyBy(1);
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
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public override int WorldWidth { get { return 6000; } }

        public override int WorldHeight { get { return 2000; } }

        public override bool FixedWidthHeight { get { return true; } }

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

            int numAgents = 2;

            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, ColorExtensions.GetRandomColor(), 0);
            }
            //MazeRunner mr = new MazeRunner(red, blue);

            MazeSetups.SetUpMaze();
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

            Zone red = Planet.World.Zones["Red(Blue)"];
            Zone blue = Planet.World.Zones["Blue(Red)"];
            if(Planet.World.AllActiveObjects.OfType<Agent>().Count() < 50)
            {
                Planet.World.ReproduceBest();
                Planet.World.ReproduceBest();
                Planet.World.ReproduceBest();
                Agent ag1 = AgentFactory.CreateAgent("Agent", red, blue, ColorExtensions.GetRandomColor(), 0);
                Agent ag2 = AgentFactory.CreateAgent("Agent", red, blue, ColorExtensions.GetRandomColor(), 0);
                Agent ag3 = AgentFactory.CreateAgent("Agent", red, blue, ColorExtensions.GetRandomColor(), 0);

                var weaklings = Planet.World.InactiveObjects.Where((wo) => wo.Shape.CentrePoint.X < 50).ToList();
                foreach(WorldObject wo in weaklings)
                {
                    Planet.World.InactiveObjects.Remove(wo);
                }
            }
        }

        public override void Reset()
        {
            bestXNum = 5;
        }
    }
}
