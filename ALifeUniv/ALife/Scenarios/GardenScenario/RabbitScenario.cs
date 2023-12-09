using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.CustomAgents;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    [ScenarioRegistration("Rabbits", description: "Lorum Ipsum")]
    public class RabbitScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

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
                new StatisticInput("ReproDistance", 0, Int32.MaxValue, 256),
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
            //if(me.Statistics["Age"].Value != 0
            //    && me.Statistics["Age"].Value % 300 == 0)
            //{
            //    me.Reproduce();
            //    me.Die();
            //}

            double distanceFromRabbit = ExtraMath.DistanceBetweenTwoPoints(me.Shape.CentrePoint, TargetRabbit.Shape.CentrePoint);
            if(distanceFromRabbit < me.Statistics["ReproDistance"].Value)
            {
                int newValue = me.Statistics["ReproDistance"].Value / 2;
                me.Statistics["ReproDistance"].ChangePropertyTo(newValue);
                me.Reproduce();
            }
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            foreach(WorldObject wo in collisions)
            {
                if(wo is Rabbit rab)
                {
                    rab.Caught(me);
                    me.Statistics["RabbitKills"].IncreasePropertyBy(1);
                    me.Reproduce();
                    me.Reproduce();
                    me.Reproduce();
                    me.Reproduce();
                    me.Reproduce();
                }
                else if(wo is Agent ag)
                {
                    ag.Die();
                    me.Die();
                    return;
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
