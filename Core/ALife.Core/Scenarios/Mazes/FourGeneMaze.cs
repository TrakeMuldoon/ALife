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

namespace ALife.Core.Scenarios.Mazes
{
    [ScenarioRegistration("Maze (Four Gene)",
        description:
        @"
A maze scenario where four gene families are created and evolved continuously.
Each gene keeps track of its own best agents.
Whenever an agent in a gene dies past the starting zone, it reproduces. 

Failure cases:
If they crash into each other, or a wall, they die without reproducing.
If they go 400 turns without increasing their X value, they die without reproducing.

Success Cases:
Whichever agents reached the furthest during the timelimit will be reproduced.
If an agent reaches the goal line, the simuluation stops."
    )]
    public class FourGeneMaze : IScenario
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
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("MaximumX", 0, Int32.MaxValue),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, CollisionBehaviour),
                new RotateCluster(agent, CollisionBehaviour)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 10,10,10,10 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        int xGoal = 50;
        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            //Checking if the agent moved far enough before the timeout

            //Rounding to the xGoal
            int roundedX = (int)(me.Shape.CentrePoint.X / xGoal) * xGoal;
            if(roundedX > me.Statistics["MaximumX"].Value)
            {
                me.Statistics["MaximumX"].Value = roundedX;
                me.Statistics["MaxXTimer"].Value = 0;
            }

            if(me.Statistics["MaxXTimer"].Value > 400)
            {
                AgentDeathBehaviour(me);
                return;
            }


            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.HomeZone.Name
                    && me.Statistics["ZoneEscapeTimer"].Value > 200)
                {
                    AgentDeathBehaviour(me);
                    return;
                }

                if(z.Name == me.TargetZone.Name)
                {
                    int successfulSeed = Planet.World.Seed;
                    int turns = Planet.World.Turns;

                    throw new Exception($"An Agent Successfully completed the maze for seed: [{successfulSeed}] in {turns} turns.");
                }
            }
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            AgentDeathBehaviour(me);
        }


        Dictionary<string, List<Agent>> Top4ByGene = new Dictionary<string, List<Agent>>();
        public void AgentDeathBehaviour(Agent toDie)
        {
            //Find the gene the agent belongs to 

            /* Unmerged change from project 'ALife.Core (netstandard2.0)'
            Before:
                        String gene = toDie.IndividualLabel.Substring(0,3);

                        //Find where this agent sits in the gene ranking
            After:
                        String gene = toDie.IndividualLabel.Substring(0,3);

                        //Find where this agent sits in the gene ranking
            */
            String gene = toDie.IndividualLabel.Substring(0,3);

            //Find where this agent sits in the gene ranking
            List<Agent> bestGeneAgents = Top4ByGene[gene];
            double toDieScore = calculateAgentScore(toDie);
            for(int i = 0; i < bestGeneAgents.Count; ++i)
            {
                Agent agent = bestGeneAgents[i];
                double agentScore = calculateAgentScore(agent);
                if(toDieScore > agentScore)
                {
                    bestGeneAgents.Insert(i, toDie);
                    bestGeneAgents.RemoveAt(bestGeneAgents.Count - 1);
                    break;
                }
            }

            //Reproduce from the top 4 based on a weighted distribution.
            // 50% #1, 25 #2, 15% #3, 10% #4
            double selection = Planet.World.NumberGen.NextDouble();
            if(selection > 0.5)
            {
                bestGeneAgents[0].Reproduce();
            }
            else if(selection > 0.75)
            {
                bestGeneAgents[1].Reproduce();
            }
            else if(selection > 0.9)
            {
                bestGeneAgents[2].Reproduce();
            }
            else
            {
                bestGeneAgents[3].Reproduce();
            }

            toDie.Die();
        }

        private double calculateAgentScore(Agent me)
        {
            double maxXAchieved = (double) me.Statistics["MaximumX"].Value;
            if(maxXAchieved < 50)
            {
                return 0;
            }
            int turnsLived = me.Statistics["Age"].Value;
            return (maxXAchieved * 100) / turnsLived;
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

            Zone red = new Zone("Red(Blue)", "Random", Colour.Red, new Geometry.Shapes.Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(Red)", "Random", Colour.Blue, new Geometry.Shapes.Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;

            instance.AddZone(red);
            instance.AddZone(blue);

            int numGenes = 4;
            int numAgentsPerGene = 10;
            Colour[] colors = new Colour[] { Colour.Orange, Colour.Red, Colour.Green, Colour.Blue };
            for(int i = 0; i < numGenes; i++)
            {
                Agent geneParent = AgentFactory.CreateAgent("Agent", red, blue, colors[i], 0);
                Top4ByGene.Add(geneParent.IndividualLabel, new List<Agent>() { geneParent, geneParent, geneParent, geneParent });
                for(int j = 0; j < numAgentsPerGene; ++j)
                {
                    geneParent.Reproduce();
                }
            }

            MazeSetups.SetUpMaze();
        }

        public virtual void GlobalEndOfTurnActions()
        {

        }
    }
}
