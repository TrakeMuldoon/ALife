using ALifeUni.ALife.Agents;
using ALifeUni.ALife.Agents.AgentActions;
using ALifeUni.ALife.Agents.Brains;
using ALifeUni.ALife.Agents.CustomAgents;
using ALifeUni.ALife.Agents.Properties;
using ALifeUni.ALife.Agents.Senses;
using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.Utility.WorldObjects;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class RabbitScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public virtual string Name => throw new NotImplementedException();

        private Rabbit TargetRabbit;

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
                new[] { CommonSenses.QuadrantEyes(agent, 0) },
                new GoalSenseCluster(agent, "RabbitSense", TargetRabbit.Shape)
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("RabbitKills", 0, Int32.MaxValue),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");
            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 18, 15, 12 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }
    
        public virtual void AgentUpkeep(Agent me)
        {
            me.Statistics["Age"].IncreasePropertyBy(1);
        }

        public virtual void EndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["Age"].Value != 0
                && me.Statistics["Age"].Value % 300 == 0)
            {
                me.Reproduce();
                me.Die();
            }
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            foreach(WorldObject wo in collisions)
            {
                if(wo is Agent ag)
                {
                    ag.Die();
                    me.Die();
                    return;
                }
                if(wo is Rabbit rab)
                {
                    rab.Caught(me);
                    me.Reproduce();
                    me.Reproduce();
                    me.Reproduce();
                    me.Reproduce();
                    me.Reproduce();
                }
            }
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        //TODO: Fully Comment This
        public virtual int WorldWidth => 735;

        //TODO: Fully Comment This
        public virtual int WorldHeight => 735;

        //TODO: Fully Comment This
        public virtual bool FixedWidthHeight
        {
            get { return false; }
        }


        public virtual void PlanetSetup()
        {
            double height = Planet.World.WorldHeight;
            double width = Planet.World.WorldWidth;

            Zone worldZone = new Zone("WholeWorld", "Random", Colors.Yellow, new Point(0, 0), width, height);
            Planet.World.AddZone(worldZone);

            TargetRabbit = new Rabbit(worldZone);

            int numAgents = 200;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", worldZone, null, Colors.Thistle, Planet.World.NumberGen.NextDouble());
            }

        }

        //TODO: Fully Comment This
        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
