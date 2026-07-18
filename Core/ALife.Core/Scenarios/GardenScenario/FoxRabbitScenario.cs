using ALife.Core.Geometry.Shapes;
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
using System;
using System.Collections.Generic;

namespace ALife.Core.Scenarios.GardenScenario
{
    [ScenarioRegistration("FoxRabbit",
        description:
        @"
Fox And Rabbit Scenario
In this scenario, There are lettuce. Every Agent is allow to eat either agents or lettuce, but once they eat something, that is the only thing they can eat. 

Failure cases:
Getting Eaten by other agents.

Success cases: 
Eating two of your chosen food.
        "
    )]
    public class FoxRabbitScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

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

            EyeCluster colourView = new EyeCluster(agent, "ColourView", true
                    , new ReadOnlyEvoNumber(startValue: -30, evoDeltaMax: 1, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                    , new ReadOnlyEvoNumber(startValue: 0,   evoDeltaMax: 1, hardMin: -360, hardMax: 360)     //Relative Orientation
                    , new ReadOnlyEvoNumber(startValue: 40,  evoDeltaMax: 1, hardMin: 40,   hardMax: 120)     //Radius
                    , new ReadOnlyEvoNumber(startValue: 60,  evoDeltaMax: 1, hardMin: 15,   hardMax: 40));    //Sweep
            EyeCluster eatDetector = new EyeCluster(agent, "EatView", true
                    , EatCluster.DefaultOrientationAroundParent //Orientation Around Parent
                    , EatCluster.DefaultRelativeOrientation     //Relative Orientation
                    , EatCluster.DefaultRadius                  //Radius
                    , EatCluster.DefaultSweep);                 //Sweep


            List<SenseCluster> agentSenses = ListHelpers.CompileList<SenseCluster>(
                new[] { CommonSenses.PairOfEyes(agent, 0) }
                //, colourView
                , eatDetector
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, int.MaxValue, StatisticInputType.Incrementing)
                , new StatisticInput("Carnivore", 0, int.MaxValue)
                , new StatisticInput("Herbivore", 0, int.MaxValue)
                , new StatisticInput("ReproEnergy", -5, int.MaxValue)
                , new StatisticInput("GoodEats", 0, int.MaxValue)
                , new StatisticInput("BadEats", 0, int.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, ActionCluster.NullInteraction),
                new RotateCluster(agent, ActionCluster.NullInteraction),
                new EatCluster(agent, EatBehaviour)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

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

            if(me.Statistics["ReproEnergy"].Value >= 2)
            {
                me.Reproduce();
                me.Statistics["ReproEnergy"].Value -= 2;
            }
            else if(me.Statistics["ReproEnergy"].Value <= -2)
            {
                me.Die();
                return;
            }
        }

        private void EatBehaviour(Agent me, List<WorldObject> collisions)
        {
            foreach(WorldObject wo in collisions)
            {
                if (me.Statistics["Carnivore"].Value == 0 && me.Statistics["Herbivore"].Value == 0) 
                {
                    if (wo is Agent)
                    {
                        me.Shape.Colour = Colour.Red;
                        me.Statistics["Carnivore"].Value = 1;
                    }
                    else if(wo is Fruit)
                    {
                        me.Shape.Colour = Colour.Blue;
                        me.Statistics["Herbivore"].Value = 1;
                    }
                    else
                    {
                        continue;
                    }
                }

                if(me.Statistics["Herbivore"].Value == 1)
                {
                    if(wo is Fruit f)
                    {
                        f.Die();
                        FruitCount -= 1;
                        me.Statistics["ReproEnergy"].IncreasePropertyBy(1);
                        me.Statistics["GoodEats"].IncreasePropertyBy(1);
                    }
                    continue;

                }
                else if(me.Statistics["Carnivore"].Value == 1)
                {
                    if(wo is Agent a)
                    {
                        a.Die();
                        me.Statistics["ReproEnergy"].IncreasePropertyBy(1);
                        me.Statistics["GoodEats"].IncreasePropertyBy(1);
                    }
                    else if (wo is Fruit f)
                    {
                        f.Die();
                        FruitCount -= 1;
                        me.Statistics["ReproEnergy"].DecreasePropertyBy(1);
                        me.Statistics["BadEats"].IncreasePropertyBy(1);
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth => 735;

        public virtual int WorldHeight => 735;

        public virtual bool FixedWidthHeight => false;

        public const int FruitMax = 100;
        public int FruitCount = 0;
        public Zone WorldZone = null;
        public virtual void PlanetSetup()
        {
            double height = Planet.World.WorldHeight;
            double width = Planet.World.WorldWidth;

            WorldZone = new Zone("WholeWorld", "Random", Colour.Yellow, new Point(0, 0), width, height);
            Planet.World.AddZone(WorldZone);

            int numAgents = 200;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = CreateAgentOne("Agent", WorldZone, null, Colour.Purple, Planet.World.NumberGen.NextDouble());
            }

            while(FruitCount < FruitMax)
            {
                Fruit gf = Fruit.FruitCreator(WorldZone, Colour.Green);
                Planet.World.AddObjectToWorld(gf);
                FruitCount += 1;
            }

        }

        public virtual void GlobalEndOfTurnActions()
        {
            while(FruitCount < FruitMax)
            {
                Fruit gf = Fruit.FruitCreator(WorldZone, Colour.Green);
                Planet.World.AddObjectToWorld(gf);
                FruitCount += 1;
            }
        }
    }
}
