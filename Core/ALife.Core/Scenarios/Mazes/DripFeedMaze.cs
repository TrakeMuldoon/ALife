using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.Utility.Collections;
using ALife.Core.Utility.Colours;
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
    [ScenarioRegistration("Maze (Drip Feed)",
        description:
        @"
A maze where the agents must attempt to reach the end zone from a starting zone.
In this drip feed maze, new agents are created every few turns, to keep the population infused with new agents.

Failure cases:
If they crash into each other, or a wall, they die without reproducing.
If they go 600 turns without increasing their X value, they die without reproducing.

Success Cases:
Whichever agents reached the furthest during the timelimit will be reproduced.
If an agent reaches the goal line, the simuluation stops."
    )]
    public class DripFeedMaze : IScenario
    {
        public Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation)
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
                new GoalSenseCluster(agent, "GoalSense", targetZone)
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),

                new StatisticInput("MaximumX", 0, Int32.MaxValue),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, CollisionBehaviour),
                new RotateCluster(agent, CollisionBehaviour)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 10, 12, 10 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        private void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            foreach(WorldObject wo in collisions)
            {
                if(wo is Agent)
                {
                    me.Die();
                }
            }
        }

        public void AgentEndOfTurnTriggers(Agent me)
        {
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

            if(me.Statistics["MaxXTimer"].Value > 400)
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

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public int WorldWidth { get { return 6000; } }

        public int WorldHeight { get { return 2000; } }

        public bool FixedWidthHeight { get { return true; } }


        Zone startZone = null;
        Zone endZone = null;
        public void PlanetSetup()
        {
            Planet instance = Planet.World;
            double height = instance.WorldHeight;
            double width = instance.WorldWidth;

            startZone = new Zone("Red(Blue)", "Random", Colour.Red, new ALife.Core.GeometryOld.Shapes.Point(0, 0), 50, height);
            endZone = new Zone("Blue(Red)", "Random", Colour.Blue, new ALife.Core.GeometryOld.Shapes.Point(width - 50, 0), 50, height);
            startZone.OppositeZone = endZone;
            startZone.OrientationDegrees = 0;

            instance.AddZone(startZone);
            instance.AddZone(endZone);

            int numAgents = 50;

            for(int i = 0; i < numAgents; i++)
            {
                Colour randomColour = Colour.GetRandomColour(Planet.World.NumberGen);
                Agent rag = AgentFactory.CreateAgent("Agent", startZone, endZone, randomColour, 0);
            }

            MazeSetups.SetUpMaze();
        }

        public void GlobalEndOfTurnActions()
        {
            if(Planet.World.Turns % 10 == 0
                && Planet.World.AllActiveObjects.OfType<Agent>().Count() < 50)
            {
                Colour randomColourA = Colour.GetRandomColour(Planet.World.NumberGen);
                Colour randomColourB = Colour.GetRandomColour(Planet.World.NumberGen);
                AgentFactory.CreateAgent("Agent", startZone, endZone, randomColourA, 0);
                AgentFactory.CreateAgent("Agent", startZone, endZone, randomColourB, 0);
            }
        }
    }
}
