﻿/***
 * Scenario: 
 * 4 way field crossing
 * This scenario features populations of agents all trying to reach the opposite end, by colour. 
 * Failure cases:
 * If they crash into each other, or the spinning rock, they die without reproducing.
 * If they do not reach the other end within 1900 turns, they die without reprodcing.
 * If they do not leave their starting zone within 200 turns, the die without reproducing.
 * 
 * Success Cases:
 * If they reach the target zone, they will restart in their own zones, and an evolved child will be spawned in each of the four zones.
 * **/


using ALifeUni.ALife.Brains;
using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class FieldCrossingScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/
        public virtual string Name
        {
            get { return "Field Crossing"; }
        }

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
                },
                new GoalSenseCluster(agent, "GoalSense", targetZone)
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new BehaviourBrain(agent, "IF Age.Value GreaterThan [10] THEN Move.GoForward AT [0.2]", "*", "*", "*", "*");
            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 15, 12 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void EndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["DeathTimer"].Value > 1899)
            {
                me.Die();
                return;
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.Zone.Name)
                {
                    if(me.Statistics["ZoneEscapeTimer"].Value > 200)
                    {
                        me.Die();
                        return;
                    }
                }
                else if(z.Name == me.TargetZone.Name)
                {
                    this.VictoryBehaviour(me);
                }
            }
        }

        protected virtual void VictoryBehaviour(Agent me)
        {
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

            //Get a new free point within the start zone.
            Point myPoint = me.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            me.Shape.CentrePoint = myPoint;
            collider.MoveObject(me);

            //Reproduce one child for each direction
            foreach(AgentZoneSpec spec in AgentZoneSpecs.Values)
            {
                CreateZonedChild(me, collider, spec);
            }

            //You have a new countdown
            me.Statistics["DeathTimer"].Value = 0;
            me.Statistics["ZoneEscapeTimer"].Value = 0;
        }

        protected static void CreateZonedChild(Agent me, ICollisionMap<WorldObject> collider, AgentZoneSpec specification)
        {
            //TODO: Refactor this into the Agent
            Agent child = (Agent)me.Reproduce();
            child.Zone = specification.StartZone;
            child.TargetZone = specification.TargetZone;
            Point reverseChildPoint = child.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            child.Shape.CentrePoint = reverseChildPoint;
            child.Shape.Orientation.Degrees = specification.StartOrientation;
            child.Shape.Color = specification.AgentColor;
            (child.Senses[0] as GoalSenseCluster).ChangeTarget(specification.TargetZone);

            collider.MoveObject(child);
        }

        public virtual void AgentUpkeep(Agent me)
        {
            //Increment or Decrement end of turn values
            me.Statistics["Age"].IncreasePropertyBy(1);
            me.Statistics["DeathTimer"].IncreasePropertyBy(1);
            me.Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);
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
            Rectangle rec = new Rectangle(rockCP, 40, 20, Colors.Black);
            FallingRock fr = new FallingRock(rockCP, rec, Colors.Black);
            Planet.World.AddObjectToWorld(fr);
        }

        public void GlobalEndOfTurnActions()
        {
            //Do Nothing
        }

        protected Agent CreateZonedAgent(AgentZoneSpec spec)
        {
            return AgentFactory.CreateAgent("Agent"
                                            , spec.StartZone
                                            , spec.TargetZone
                                            , spec.AgentColor
                                            , spec.StartOrientation);
        }
    }
}
