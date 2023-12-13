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
                new EyeCluster(agent, "ColourForward", true
                    , new ROEvoNumber(startValue: 0,  evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: -8, evoDeltaMax: 5, hardMin: -360, hardMax: 360)   //Relative Orientation
                    , new ROEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40,   hardMax: 120)     //Radius
                    , new ROEvoNumber(startValue: 16, evoDeltaMax: 1, hardMin: 15,   hardMax: 40)),    //Sweep
                    
                new GoalSenseCluster(agent, "RabbitSense", TargetRabbit.Shape)
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("RabbitKills", 0, Int32.MaxValue),
                new StatisticInput("ReproDistance", 0, Int32.MaxValue, 64),
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

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["Age"].Value > 1000)
            {
                me.Die();
                return;
            }

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
                Agent rag = AgentFactory.CreateAgent("Agent", worldZone, null, Colors.LawnGreen, Planet.World.NumberGen.NextDouble());
            }

        }

        //TODO: Fully Comment This
        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
