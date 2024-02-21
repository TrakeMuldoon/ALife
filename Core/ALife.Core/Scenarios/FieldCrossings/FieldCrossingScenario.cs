using ALife.Core.Collision;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.Utility.Collections;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using ALife.Core.WorldObjects.Prebuilt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.Scenarios.FieldCrossings
{
    [ScenarioRegistration("Field Crossing",
    description:
        @"
4 way field crossing
This scenario features populations of agents all trying to reach the opposite end, by colour.
Failure cases:
If they crash into each other, or the spinning rock, they die without reproducing.
If they do not reach the other end within 1900 turns, they die without reprodcing.
If they do not leave their starting zone within 200 turns, the die without reproducing.

Success Cases:
If they reach the target zone, they will restart in their own zones, and an evolved child will be spawned in each of the four zones."
     )]
    public class FieldCrossingScenario : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgentOne(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation)
        {
            throw new NotImplementedException();
        }

        public void AgentEndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["DeathTimer"].Value > 1899)
            {
                me.Die();
                return;
            }
            //TODO: Refactor this into a world function called "WhatZonesAmIIn" or something like that. Get the scenario a little further from the nuts/bolts.
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.HomeZone.Name)
                {
                    if(me.Statistics["ZoneEscapeTimer"].Value > 200)
                    {
                        me.Die();
                        return;
                    }
                }
                else if(z.Name == me.TargetZone.Name)
                {
                    VictoryBehaviour(me);
                }
            }
        }

        protected virtual void VictoryBehaviour(Agent me)
        {
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

            //Get a new free Point within the start zone.
            Point myPoint = me.HomeZone.Distributor.NextObjectCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            me.Shape.CentrePoint = myPoint;
            collider.MoveObject(me);

            //Reproduce one child for each direction
            foreach(AgentZoneSpec spec in AgentZoneSpecs.Values)
            {
                FieldCrossingHelpers.CreateZonedChild(me, collider, spec);
            }

            //You have a new countdown
            me.Statistics["DeathTimer"].Value = 0;
            me.Statistics["ZoneEscapeTimer"].Value = 0;
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //Collision means death right now
            foreach(WorldObject wo in collisions)
            {
                if(wo is Agent ag)
                {
                    ag.Die();
                }
            }
            me.Die();
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth { get { return 1000; } }

        public virtual int WorldHeight { get { return 1000; } }

        public virtual bool FixedWidthHeight { get { return false; } }

        protected Dictionary<Zone, AgentZoneSpec> AgentZoneSpecs = new Dictionary<Zone, AgentZoneSpec>();

        public virtual void PlanetSetup()
        {

            int width = Planet.World.WorldWidth;
            int height = Planet.World.WorldHeight;

            AgentZoneSpecs = FieldCrossingHelpers.InsertOpposedZonesAndReturnZoneSpec();

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                foreach(Zone z in AgentZoneSpecs.Keys)
                {
                    CreateZonedAgent(AgentZoneSpecs[z]);
                }
            }

            Point rockCP = new Point((width / 2) + (width / 3), height / 2);
            Rectangle rec = new Rectangle(rockCP, 40, 20, Colour.Black);
            FallingRock fr = new FallingRock(rockCP, rec, Colour.Black);
            Planet.World.AddObjectToWorld(fr);
        }

        public void GlobalEndOfTurnActions()
        {
            //Do Nothing
        }

        protected Agent CreateZonedAgent(AgentZoneSpec spec)
        {
            Agent agent = AgentFactory.ConstructCircularAgent("Agent"
                                                        , spec.StartZone
                                                        , spec.TargetZone
                                                        , spec.AgentColor
                                                        , null
                                                        , spec.StartOrientation);

            List<SenseCluster> agentSenses = ListHelpers.CompileList(
                new IEnumerable<SenseCluster>[]
                {
                    CommonSenses.PairOfEyes(agent)
                },
                new GoalSenseCluster(agent, "GoalSense", agent.TargetZone)
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent, CollisionBehaviour),
                new RotateCluster(agent, CollisionBehaviour)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 15, 12 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

    }
}
