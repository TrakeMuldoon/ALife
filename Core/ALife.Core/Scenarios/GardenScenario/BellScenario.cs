using ALife.Core.Collision;
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

namespace ALife.Core.Scenarios.GardenScenario
{
    [ScenarioRegistration("Bell Scenario",
description:
    @"
Bell Scenario
This  scenario takes place in a field with a bell ringing. This is a basic sound scenario. 
We need to determine if the agents can learn to orient on a sound base only on its volume and the differential thereof between two ears.
Failure cases:
Crash into each other, die.
Don't follow the sound within 800 units. Die.

Success Cases:
Arrive at the source of the sound. Reproduce twice, and get moved to a random location."
 )]
    [SuggestedSeed(1566464761, "Straight to the emitters")]
    public class BellScenario : IScenario
    {

        private const int DEATH_TIMER = 800;
        private const string TARGET_ZONENAME_PREFIX = "SoundSource";
        private const int NUM_AGENTS = 120;

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

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new ProximityCluster(agent, "Proximity"
                                    , new ReadOnlyEvoNumber(20, 4, 10, 40)) //Radius
                , new EarCluster(agent, "LeftEar"
                                 , new ReadOnlyEvoNumber(270, 3, 190, 350) //Orientation around parent
                                 , new ReadOnlyEvoNumber(10, 1, 7, 13)) // Radius
                , new EarCluster(agent, "RightEar"
                                 , new ReadOnlyEvoNumber(90, 3, 10, 170) //Orientation around parent
                                 , new ReadOnlyEvoNumber(10, 1, 7, 13)) // Radius
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue, StatisticInputType.Decrementing, DEATH_TIMER),
                new StatisticInput("BellDings", 0, Int32.MaxValue),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, CollisionBehaviour),
                new RotateCluster(agent, CollisionBehaviour)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 18, 15, 12 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["DeathTimer"].Value < 1)
            {
                me.Die();
                return;
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name.StartsWith(TARGET_ZONENAME_PREFIX))
                {
                    VictoryBehaviour(me);
                }
            }
        }

        private void VictoryBehaviour(Agent winner)
        {
            winner.Reproduce();
            winner.Reproduce();
            winner.Statistics["BellDings"].IncreasePropertyBy(1);
            winner.Statistics["DeathTimer"].ChangePropertyTo(DEATH_TIMER);

            //Move to a random position so they can attempt to find the zone again. 
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[winner.CollisionLevel];
            ALife.Core.GeometryOld.Shapes.Point myPoint = winner.HomeZone.Distributor.NextObjectCentre(winner.Shape.BoundingBox.XLength, winner.Shape.BoundingBox.YHeight);
            winner.Shape.CentrePoint = myPoint;
            collider.MoveObject(winner);
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            me.Die();
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth => 1200;

        public virtual int WorldHeight => 1200;

        public virtual bool FixedWidthHeight
        {
            get { return false; }
        }


        public virtual void PlanetSetup()
        {
            double height = Planet.World.WorldHeight;
            double width = Planet.World.WorldWidth;

            Zone WorldZone = new Zone("WholeWorld", "Random", Colour.Yellow, new ALife.Core.GeometryOld.Shapes.Point(0, 0), width, height);
            Planet.World.AddZone(WorldZone);


            AddEmitterPair(height / 3, width / 3);
            AddEmitterPair(height * 2 / 3, width / 3);
            AddEmitterPair(height / 3, width * 2 / 3);
            AddEmitterPair(height * 2 / 3, width * 2 / 3);

            for(int i = 0; i < NUM_AGENTS; ++i)
            {
                AgentFactory.CreateAgent("Agent", WorldZone, WorldZone, Colour.Blue, 0);
            }
        }

        private int EmitterPairs = 0;
        private void AddEmitterPair(double x, double y)
        {
            ALife.Core.GeometryOld.Shapes.Point targetPoint = new ALife.Core.GeometryOld.Shapes.Point(x, y);
            Zone targetZone = new Zone($"{TARGET_ZONENAME_PREFIX}{++EmitterPairs}", "Random", Colour.Red, targetPoint, 12,12);
            Planet.World.AddZone(targetZone);
            SoundEmitter emitter = new SoundEmitter(new ALife.Core.GeometryOld.Shapes.Point(targetPoint.X + 6, targetPoint.Y + 6));
            Planet.World.AddObjectToWorld(emitter);
        }

        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }

    }
}
