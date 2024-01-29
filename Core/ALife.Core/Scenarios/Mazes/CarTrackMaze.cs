using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.Utility.Collections;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;

namespace ALife.Core.Scenarios.Mazes
{
    [ScenarioRegistration("Around The Track"
        , description: @"
A racetrack where the agents need to go around the track to an endzone which is nearly where they started.
There are 4 checkpoints (1/4, 1/2, 3/4 and Finish Line)

Failure cases:
If they do not leave the start zone within 200 turns, they die.
If they do not reach the next checkpoint within 1000 turns, they die.

Success Cases:
Reaching 1/4 resets death timer.
Reaching 1/2 resets death timer, and reproduces at the start line.
Reaching 3/4 resets death timer, and reproduces twice at the start line.
Reaching the finish line, reproduces 3 times and then dies... Victorious!"
    )]
    [SuggestedSeed(1832460063, "Fun scenario!!!")]
    public class CarTrackMaze : IScenario
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

            List<SenseCluster> agentSenses = ListHelpers.CompileList<SenseCluster>(
                new IEnumerable<SenseCluster>[]
                {
                    CommonSenses.PairOfEyes(agent)
                },
                new EyeCluster(agent, "ShortEye"
                    , new ReadOnlyEvoNumber(startValue: 0, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ReadOnlyEvoNumber(startValue: 0, evoDeltaMax: 5, hardMin: -360, hardMax: 360)   //Relative Orientation
                    , new ReadOnlyEvoNumber(startValue: 40, evoDeltaMax: 3, hardMin: 40, hardMax: 120)     //Radius
                    , new ReadOnlyEvoNumber(startValue: 30, evoDeltaMax: 1, hardMin: 15, hardMax: 45))    //Sweep
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("ProgressTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, ActionCluster.NullInteraction),
                new RotateCluster(agent, ActionCluster.NullInteraction)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");
            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 15, 12, 10 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        private Dictionary<string, HashSet<Agent>> zonesHit = new Dictionary<string, HashSet<Agent>>
        {
            { "Start", new HashSet<Agent>() },
            { "Mid1", new HashSet<Agent>() },
            { "Half", new HashSet<Agent>() },
            { "Mid3", new HashSet<Agent>() },
            { "End", new HashSet<Agent>() }
        };

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["ProgressTimer"].Value > 1000)
            {
                me.Die();
                return;
            }

            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == "Start")
                {
                    CarTrackMaze.StartZoneBehaviour(me);
                    return;
                }

                if(zonesHit[z.Name].Contains(me))
                {
                    return;
                }

                zonesHit[z.Name].Add(me);
                me.Statistics["ProgressTimer"].Value = 0;

                switch(z.Name)
                {
                    case "Mid1": break;
                    case "Half": me.Reproduce(); break;
                    case "Mid3": me.Reproduce(); me.Reproduce(); break;
                    case "End": CarTrackMaze.VictoryBehaviour(me); break;
                }
            }
        }

        private static void StartZoneBehaviour(Agent me)
        {
            if(me.Statistics["Age"].Value > 200)
            {
                me.Die();
            }
        }

        private static void VictoryBehaviour(Agent me)
        {
            me.Reproduce();
            me.Reproduce();
            me.Reproduce();
            me.Die();
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth => 1500;

        public virtual int WorldHeight => 800;

        public virtual bool FixedWidthHeight
        {
            get { return true; }
        }

        public virtual void PlanetSetup()
        {
            MazeSetups.BuildThinningCarTrack();

            Zone startZone = new Zone("Start", "Random", Colour.Green, new ALife.Core.GeometryOld.Shapes.Point(30, 331), 190, 80);
            Zone midOne = new Zone("Mid1", "Random", Colour.Orange, new ALife.Core.GeometryOld.Shapes.Point(800, 585), 20, 200);
            Zone halfWay = new Zone("Half", "Random", Colour.Orange, new ALife.Core.GeometryOld.Shapes.Point(1320, 320), 200, 20);
            Zone midThree = new Zone("Mid3", "Random", Colour.Orange, new ALife.Core.GeometryOld.Shapes.Point(800, 18), 20, 115);
            Zone endZone = new Zone("End", "Random", Colour.Red, new ALife.Core.GeometryOld.Shapes.Point(30, 285), 190, 40);

            Planet.World.AddZone(startZone);
            Planet.World.AddZone(midOne);
            Planet.World.AddZone(halfWay);
            Planet.World.AddZone(midThree);
            Planet.World.AddZone(endZone);

            int numAgents = 20;
            for(int i = 0; i < numAgents; i++)
            {
                Colour randomColour = Colour.GetRandomColour(Planet.World.NumberGen);
                Agent rag = AgentFactory.CreateAgent("Agent", startZone, endZone, randomColour, 90);
            }
        }


        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
