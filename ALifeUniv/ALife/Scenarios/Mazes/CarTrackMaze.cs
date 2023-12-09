using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    [ScenarioRegistration("Around The Track", description: "Lorum Ipsum")]
    [SuggestedSeed(1832460063, "Fun scenario!!!")]
    public class CarTrackMaze : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color colour, double startOrientation)
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
                }
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("ProgressTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");
            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 15, 12, 10 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentUpkeep(Agent me)
        {
            me.Statistics["Age"].IncreasePropertyBy(1);
            me.Statistics["ProgressTimer"].IncreasePropertyBy(1);
        }

        private Dictionary<string, HashSet<Agent>> zonesHit = new Dictionary<string, HashSet<Agent>>
        {
            { "Start", new HashSet<Agent>() },
            { "Mid1", new HashSet<Agent>() },
            { "Half", new HashSet<Agent>() },
            { "Mid3", new HashSet<Agent>() },
            { "End", new HashSet<Agent>() }
        };

        public virtual void EndOfTurnTriggers(Agent me)
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

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //me.Die();
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

            Zone startZone = new Zone("Start", "Random", Colors.Green, new Point(30, 331), 190, 80);
            Zone midOne = new Zone("Mid1", "Random", Colors.Orange, new Point(800, 585), 20, 200);
            Zone halfWay = new Zone("Half", "Random", Colors.Orange, new Point(1320, 320), 200, 20);
            Zone midThree = new Zone("Mid3", "Random", Colors.Orange, new Point(800, 18), 20, 115);
            Zone endZone = new Zone("End", "Random", Colors.Red, new Point(30, 285), 190, 40);

            Planet.World.AddZone(startZone);
            Planet.World.AddZone(midOne);
            Planet.World.AddZone(halfWay);
            Planet.World.AddZone(midThree);
            Planet.World.AddZone(endZone);

            int numAgents = 20;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", startZone, endZone, ColorExtensions.GetRandomColor(), 90);
            }
        }


        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
