using ALifeUni.ALife.Utility;
using ALifeUni.ALife.Utility.WorldObjects;
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
    [ScenarioRegistration("Mushroom Garden",
    description:
        @"Mushroom Garden
            This scenario takes place in a mushroom garden. Mushrooms spawn with a ratio of 50% good/bad.
            Failure cases:
            Eat a bad mushroom, they die. 
            Get eaten (collided into) by another agent, they die.

            Success Cases:
            If they eat three other agents, they reproduce. 
            If they eat two green mushrooms, they reproduce."
     )]
    public class MushroomScenario : IScenario
    {
        public const double GOOD_MUSH_PERCENT = 0.50;

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

            List<SenseCluster> agentSenses = ListExtensions.CompileList<SenseCluster>(null,
                new EyeCluster(agent, "EyeLeft", true
                    , new ROEvoNumber(startValue: -20, evoDeltaMax: 1, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: 5,  evoDeltaMax: 1, hardMin: -360,  hardMax: 360)     //Relative Orientation
                    , new ROEvoNumber(startValue: 80, evoDeltaMax: 1, hardMin: 40,    hardMax: 120)       //Radius
                    , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15,    hardMax: 40)),      //Sweep
                new EyeCluster(agent, "EyeRight", true
                    , new ROEvoNumber(startValue: 20, evoDeltaMax: 1, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                    , new ROEvoNumber(startValue: -5, evoDeltaMax: 1, hardMin: -360, hardMax: 360)    //Relative Orientation
                    , new ROEvoNumber(startValue: 80, evoDeltaMax: 1, hardMin: 40,    hardMax: 120)       //Radius
                    , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15,    hardMax: 40))       //Sweep
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue),
                new StatisticInput("HowFullAmI", 0, Int32.MaxValue),
                new StatisticInput("Kills", 0, Int32.MaxValue),
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
            me.Statistics["DeathTimer"].IncreasePropertyBy(1);
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["DeathTimer"].Value > 500)
            {
                me.Die();
                return;
            }
            if(me.Statistics["HowFullAmI"].Value >= 6)
            {
                me.Reproduce();
                me.Statistics["HowFullAmI"].DecreasePropertyBy(6);
            }
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            foreach(WorldObject wo in collisions)
            {
                if(wo is Agent ag)
                {
                    ag.Die();
                    me.Statistics["HowFullAmI"].IncreasePropertyBy(2);
                    me.Statistics["Kills"].IncreasePropertyBy(1);
                }
                else if(wo is Fruit f)
                {
                    if(f.Shape.Color == PURE_GREEN)
                    {
                        me.Statistics["HowFullAmI"].IncreasePropertyBy(3);
                        me.Statistics["DeathTimer"].ChangePropertyTo(0);
                        f.Die();
                    }
                    else if(f.Shape.Color == PURE_RED)
                    {
                        me.Die();
                        f.Die();
                    }
                    AllFruits.Remove(f);
                }
            }
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        //TODO: Fully Comment This
        public virtual int WorldWidth => 850;

        //TODO: Fully Comment This
        public virtual int WorldHeight => 850;

        //TODO: Fully Comment This
        public virtual bool FixedWidthHeight
        {
            get { return false; }
        }


        const int FruitMax = 100;
        Color PURE_RED = new Color() { A = 255, R = 255, G = 0, B = 0 };
        Color PURE_BLUE = new Color() { A = 255, R = 0, G = 0, B = 255 };
        Color PURE_GREEN = new Color() { A = 255, R = 0, G = 255, B = 0 };

        List<Fruit> AllFruits = new List<Fruit>();
        Zone WorldZone = null;

        public virtual void PlanetSetup()
        {
            double height = Planet.World.WorldHeight;
            double width = Planet.World.WorldWidth;

            WorldZone = new Zone("WholeWorld", "Random", Colors.Yellow, new Point(0, 0), width, height);
            Planet.World.AddZone(WorldZone);

            int numAgents = 200;

            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", WorldZone, null, PURE_BLUE, 0);
            }

            for(int k = 0; k < 5; k++)
            {
                Agent mg = new MushroomGatherer(WorldZone);
            }

            while(AllFruits.Count < FruitMax)
            {
                Fruit gf = Fruit.FruitCreator(WorldZone, PURE_GREEN);
                Planet.World.AddObjectToWorld(gf);
                AllFruits.Add(gf);
            }
            for(int j = 0; j < 5; j++)
            {
                Fruit rf = Fruit.FruitCreator(WorldZone, PURE_RED);
                Planet.World.AddObjectToWorld(rf);
                AllFruits.Add(rf);
            }
        }

        public virtual void GlobalEndOfTurnActions()
        {
            while(AllFruits.Count < FruitMax)
            {
                double d = Planet.World.NumberGen.NextDouble();
                Fruit newFruit;
                if(d > 0.80)
                {
                    newFruit = Fruit.FruitCreator(WorldZone, PURE_RED);
                }
                else
                {
                    newFruit = Fruit.FruitCreator(WorldZone, PURE_GREEN);
                }
                Planet.World.AddObjectToWorld(newFruit);
                AllFruits.Add(newFruit);
            }
        }
    }
}
