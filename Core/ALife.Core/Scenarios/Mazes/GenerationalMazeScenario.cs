﻿using ALife.Core.Geometry.Shapes;
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
    [ScenarioRegistration("Maze (Generational)",
        description:
        @"
A maze where the agents must attempt to reach the end zone from a starting zone.
In this Generational maze, when all the agents die, a new set of agents is created from the best of the previous generation

Failure cases:
If they crash into each other, or a wall, they die without reproducing.
If they go 600 turns without increasing their X value, they die without reproducing.

Success Cases:
Whichever agents reached the furthest during the timelimit will be reproduced.
If an agent reaches the goal line, the simuluation stops."
    )]
    public class GenerationalMazeScenario : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgentOne(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation)
        {
            Agent agent = AgentFactory.ConstructCircularAgent(genusName
                                                             , parentZone
                                                             , targetZone
                                                             , colour
                                                             , null
                                                             , startOrientation);

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
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("MaximumX", 0, Int32.MaxValue),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("Iteration", 0, Int32.MaxValue, Iteration)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, CollisionBehaviour),
                new RotateCluster(agent, CollisionBehaviour)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 9, 9 });
            //            IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            int roundedX = (int)(me.Shape.CentrePoint.X / 100) * 100;
            if(roundedX > me.Statistics["MaximumX"].Value)
            {
                me.Statistics["MaximumX"].Value = roundedX;
                me.Statistics["MaxXTimer"].Value = 0;
            }

            if(me.Statistics["MaxXTimer"].Value > 600)
            {
                me.Die();
                return;
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.HomeZone.Name
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

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            me.Die();
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth
        {
            get { return 6000; }
        }
        public virtual int WorldHeight
        {
            get { return 2000; }
        }
        public virtual bool FixedWidthHeight
        {
            get { return true; }
        }

        public virtual void PlanetSetup()
        {
            Planet instance = Planet.World;
            double height = instance.WorldHeight;
            double width = instance.WorldWidth;

            Zone red = new Zone("Red(Blue)", "Random", Colour.Red, new Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(Red)", "Random", Colour.Blue, new Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;

            instance.AddZone(red);
            instance.AddZone(blue);

            int numAgents = 30;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = CreateAgentOne("Agent", red, blue, Colour.Blue, 0);
            }

            MazeSetups.SetUpMaze();
        }

        readonly int bestXNum = 4;
        int Iteration = 1;
        Agent bestEver;
        public virtual void GlobalEndOfTurnActions()
        {
            IEnumerable<Agent> someAgents = Planet.World.AllActiveObjects.OfType<Agent>();

            IEnumerable<Agent> LivingAgents =
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

                string generationString = string.Format("Gen {0}: Stragglers: {1} Avg: {2:0.000}, MaxX: {3:0}", Iteration, living, averageX, maxX);
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
    }
}
