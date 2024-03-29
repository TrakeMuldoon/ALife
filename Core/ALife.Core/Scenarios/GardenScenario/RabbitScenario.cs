﻿using ALife.Core.Geometry.Shapes;
using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.Utility.Collections;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using ALife.Core.WorldObjects.Prebuilt;
using System.Collections.Generic;

namespace ALife.Core.Scenarios.GardenScenario
{
    [ScenarioRegistration("Rabbits",
        description:
        @"
Rabbit Chaser (smart, slow rabbit)
In this scenario, the agents are trying to chase down a rabbit using the 'GoalSense' cluster. 
Each Agent has a sense for where the rabbit is. 

Failure cases:
If the agents bump into anything except the rabbit, they die.
After 1000 turns, they die.

Success cases: 
If the agents come withing 64,32,16,8,4,2 units of distance from the Rabbit, they reproduce.
If the agents bump into the rabbit, they reproduce 5 times, and the rabbit respawns somewhere else.
        "
    )]
    [SuggestedSeed(1434557654, "Merry Chase!")]
    [SuggestedSeed(1763573612, "Agents almost die out (start coming back around 10-20k ticks)")]
    [SuggestedSeed(1735952524, "Barely successful")]
    [SuggestedSeed(334962087, "Successful from start")]
    public class RabbitScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        private Rabbit TargetRabbit;

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

            List<SenseCluster> agentSenses = ListHelpers.CompileList<SenseCluster>(
                new[] { CommonSenses.QuadrantEyes(agent, 0) },
                new EyeCluster(agent, "ColourForward", true
                    , new ReadOnlyEvoNumber(startValue: 0,  evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                    , new ReadOnlyEvoNumber(startValue: -8, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Relative Orientation
                    , new ReadOnlyEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40,   hardMax: 120)     //Radius
                    , new ReadOnlyEvoNumber(startValue: 16, evoDeltaMax: 1, hardMin: 15,   hardMax: 40)),    //Sweep
                new GoalSenseCluster(agent, "RabbitSense", TargetRabbit.Shape)
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, int.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("RabbitKills", 0, int.MaxValue),
                new StatisticInput("ReproDistance", 0, int.MaxValue, 64),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, CollisionBehaviour),
                new RotateCluster(agent, CollisionBehaviour)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new BehaviourBrain(agent, "*", "*", "*", "*", "*");
            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 18, 15, 12 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["Age"].Value > 1000)
            {
                me.Die();
                return;
            }

            double distanceFromRabbit = GeometryMath.DistanceBetweenTwoPoints(me.Shape.CentrePoint, TargetRabbit.Shape.CentrePoint);
            if(distanceFromRabbit < me.Statistics["ReproDistance"].Value)
            {
                int newValue = me.Statistics["ReproDistance"].Value / 2;
                me.Statistics["ReproDistance"].ChangePropertyTo(newValue);
                _ = me.Reproduce();
            }
        }

        private void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            if(me is Rabbit)
            {
                return;
            }
            foreach(WorldObject wo in collisions)
            {
                if(wo is Rabbit rab)
                {
                    rab.Caught(me);
                    me.Statistics["RabbitKills"].IncreasePropertyBy(1);
                    _ = me.Reproduce();
                    _ = me.Reproduce();
                    _ = me.Reproduce();
                    _ = me.Reproduce();
                    _ = me.Reproduce();
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

        public virtual int WorldWidth => 735;

        public virtual int WorldHeight => 735;

        public virtual bool FixedWidthHeight => false;


        public virtual void PlanetSetup()
        {
            double height = Planet.World.WorldHeight;
            double width = Planet.World.WorldWidth;

            Zone worldZone = new Zone("WholeWorld", "Random", Colour.Yellow, new Point(0, 0), width, height);
            Planet.World.AddZone(worldZone);

            TargetRabbit = new Rabbit(worldZone);

            int numAgents = 200;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = CreateAgentOne("Agent", worldZone, null, Colour.LawnGreen, Planet.World.NumberGen.NextDouble());
            }

        }

        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
