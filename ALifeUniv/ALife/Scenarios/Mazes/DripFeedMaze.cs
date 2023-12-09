using ALifeUni.ALife.Agents;
using ALifeUni.ALife.Agents.AgentActions;
using ALifeUni.ALife.Agents.Brains;
using ALifeUni.ALife.Agents.Properties;
using ALifeUni.ALife.Agents.Senses;
using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    [ScenarioRegistration("Drip Feed Maze", description: "Lorum Ipsum")]
    public class DripFeedMaze : IScenario
    {
        public Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color colour, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical
                                    , parentZone
                                    , targetZone);

            int agentRadius = 5;
            agent.ApplyCircleShapeToAgent(parentZone.Distributor, colour, agentRadius, startOrientation);

            List<SenseCluster> agentSenses = ListExtensions.CompileList<SenseCluster>(
                new IEnumerable<SenseCluster>[]
                {
                    CommonSenses.PairOfEyes(agent)
                },
                new GoalSenseCluster(agent, "GoalSense", targetZone)
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("MaximumX", 0, Int32.MaxValue),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 10, 12, 10 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public void AgentUpkeep(Agent me)
        {
            //Increment or Decrement end of turn values
            me.Statistics["Age"].IncreasePropertyBy(1);
            me.Statistics["MaxXTimer"].IncreasePropertyBy(1);
            me.Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);

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

        public void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            foreach(WorldObject wo in collisions)
            {
                if(wo is Agent)
                {
                    me.Die();
                }
            }
        }

        public void EndOfTurnTriggers(Agent me)
        {
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

            startZone = new Zone("Red(Blue)", "Random", Colors.Red, new Point(0, 0), 50, height);
            endZone = new Zone("Blue(Red)", "Random", Colors.Blue, new Point(width - 50, 0), 50, height);
            startZone.OppositeZone = endZone;
            startZone.OrientationDegrees = 0;

            instance.AddZone(startZone);
            instance.AddZone(endZone);

            int numAgents = 50;

            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", startZone, endZone, ColorExtensions.GetRandomColor(), 0);
            }

            MazeSetups.SetUpMaze();
        }

        public void GlobalEndOfTurnActions()
        {
            if(Planet.World.Turns % 10 == 0
                && Planet.World.AllActiveObjects.OfType<Agent>().Count() < 50)
            {
                AgentFactory.CreateAgent("Agent", startZone, endZone, ColorExtensions.GetRandomColor(), 0);
                AgentFactory.CreateAgent("Agent", startZone, endZone, ColorExtensions.GetRandomColor(), 0);
            }
        }
    }
}
