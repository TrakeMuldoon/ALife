using ALife.Core.Utility;
using ALife.Core.Utility.Collections;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using ALife.Core.WorldObjects.Prebuilt;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ALife.Core.Scenarios.GardenScenario
{
    [ScenarioRegistration("Mushroom Garden",
    description:
        @"
Mushroom Garden
This scenario takes place in a mushroom garden. Mushrooms spawn with a ratio of 50% good/bad.
Failure cases:
Eat a bad mushroom, they die. 
Get eaten (collided into) by another agent, they die.

Success Cases:
If they eat three other agents, they reproduce. 
If they eat two green mushrooms, they reproduce."
     )]
    [SuggestedSeed(1559842024, "Loops and Swirls!")]
    public class MushroomScenario : IScenario
    {
        public const double GOOD_MUSH_PERCENT = 0.50;

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
                new EyeCluster(agent, "EyeLeft", true
                    , new ReadOnlyEvoNumber(startValue: -20, evoDeltaMax: 1, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ReadOnlyEvoNumber(startValue: 5,  evoDeltaMax: 1, hardMin: -360,  hardMax: 360)     //Relative Orientation
                    , new ReadOnlyEvoNumber(startValue: 80, evoDeltaMax: 1, hardMin: 40,    hardMax: 120)       //Radius
                    , new ReadOnlyEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15,    hardMax: 40)),      //Sweep
                new EyeCluster(agent, "EyeRight", true
                    , new ReadOnlyEvoNumber(startValue: 20, evoDeltaMax: 1, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                    , new ReadOnlyEvoNumber(startValue: -5, evoDeltaMax: 1, hardMin: -360, hardMax: 360)    //Relative Orientation
                    , new ReadOnlyEvoNumber(startValue: 80, evoDeltaMax: 1, hardMin: 40,    hardMax: 120)       //Radius
                    , new ReadOnlyEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15,    hardMax: 40))       //Sweep
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
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
                    if(f.Shape.Colour == PURE_GREEN)
                    {
                        me.Statistics["HowFullAmI"].IncreasePropertyBy(3);
                        me.Statistics["DeathTimer"].ChangePropertyTo(0);
                        f.Die();
                    }
                    else if(f.Shape.Colour == PURE_RED)
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

        Colour PURE_RED = Colour.Red;
        Colour PURE_BLUE = Colour.Blue;
        Colour PURE_GREEN = Colour.Green;

        List<Fruit> AllFruits = new List<Fruit>();
        Zone WorldZone = null;

        public virtual void PlanetSetup()
        {
            double height = Planet.World.WorldHeight;
            double width = Planet.World.WorldWidth;

            WorldZone = new Zone("WholeWorld", "Random", Colour.Yellow, new Geometry.Shapes.Point(0, 0), width, height);
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
