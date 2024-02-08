using ALife.Core.Collision;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.Utility.Collections;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.Scenarios.FieldCrossings
{
    [ScenarioRegistration("Field Crossing (Champions)",
    description:
        @"
4 way field crossing (Champions)
This scenario features populations of agents all trying to reach the opposite end, by colour.
It also features a few 'best of all time' agents as well.
Failure cases:
If they do not reach the other end within 1900 turns, they die without reprodcing.
If they do not leave their starting zone within 200 turns, the die without reproducing.

Success Cases:
If they reach the target zone, they will restart in their own zones, and two evolved children will be spawned"
     )]
    public class FieldCrossingChampions : FieldCrossingScenario
    {
        private Dictionary<Zone, AgentZoneSpec> RotatedZoneSpecs = new Dictionary<Zone, AgentZoneSpec>();
        private bool _init;
        private void Initialize()
        {
            List<Zone> keys = AgentZoneSpecs.Keys.ToList<Zone>();
            List<AgentZoneSpec> specs = AgentZoneSpecs.Values.ToList<AgentZoneSpec>();
            specs.Add(specs[0]);
            specs.RemoveAt(0);

            for(int i = 0; i < keys.Count; i++)
            {
                RotatedZoneSpecs.Add(keys[i], specs[i]);
            }
            _init = true;
        }

        protected override void VictoryBehaviour(Agent me)
        {
            if(!_init)
            {
                Initialize();
            }
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

            //Get a new free Point within the start zone.
            Point myPoint = me.HomeZone.Distributor.NextObjectCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            me.Shape.CentrePoint = myPoint;
            collider.MoveObject(me);

            //Create one child
            FieldCrossingScenario.CreateZonedChild(me, collider, RotatedZoneSpecs[me.TargetZone]);

            //You have a new countdown
            me.Statistics["DeathTimer"].Value = 0;
            me.Statistics["ZoneEscapeTimer"].Value = 0;
        }

        public override void PlanetSetup()
        {
            AgentZoneSpecs = FieldCrossingHelpers.InsertOpposedZonesAndReturnZoneSpec();

            int numAgents = 40;
            for(int i = 0; i < numAgents; i++)
            {
                foreach(Zone z in AgentZoneSpecs.Keys)
                {
                    CreateZonedAgent(AgentZoneSpecs[z]);
                }
            }

            inputString = null;
            debugColour = Colour.Red;

            for(int j = 0; j < 40; ++j)
            {
                foreach(Zone z in AgentZoneSpecs.Keys)
                {
                    CreateZonedAgent(AgentZoneSpecs[z]);
                }
            }
        }


        //GROSS GROSS GROSS HACK HACK
        private string inputString = null;
        private Colour debugColour = Colour.Blue;

        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical
                                    , parentZone
                                    , targetZone);

            int agentRadius = 5;
            agent.ApplyCircleShapeToAgent(parentZone.Distributor, colour, agentRadius, startOrientation);

            List<SenseCluster> agentSenses = ListHelpers.CompileList(
                new IEnumerable<SenseCluster>[]
                {
                    CommonSenses.PairOfEyes(agent)
                },
                new EyeCluster(agent, "ShortEyeLeft"
                    , new ReadOnlyEvoNumber(startValue: 0, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ReadOnlyEvoNumber(startValue: 295, evoDeltaMax: 5, hardMin: -360, hardMax: 360)   //Relative Orientation
                    , new ReadOnlyEvoNumber(startValue: 45, evoDeltaMax: 3, hardMin: 40, hardMax: 120)     //Radius
                    , new ReadOnlyEvoNumber(startValue: 30, evoDeltaMax: 1, hardMin: 15, hardMax: 45)),    //Sweep
               new EyeCluster(agent, "ShortEyeRight"
                    , new ReadOnlyEvoNumber(startValue: 0, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ReadOnlyEvoNumber(startValue: 65, evoDeltaMax: 5, hardMin: -360, hardMax: 360)   //Relative Orientation
                    , new ReadOnlyEvoNumber(startValue: 45, evoDeltaMax: 3, hardMin: 40, hardMax: 120)     //Radius
                    , new ReadOnlyEvoNumber(startValue: 30, evoDeltaMax: 1, hardMin: 15, hardMax: 45)),    //Sweep
                new GoalSenseCluster(agent, "GoalSense", targetZone)
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

            IBrain newBrain = null;
            if(inputString != null)
            {
                newBrain = new NeuralNetworkBrain(agent, inputString);
                agent.Shape.DebugColour = debugColour;
            }
            else
            {
                newBrain = new NeuralNetworkBrain(agent, new List<int> { 15, 12 });
            }

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }
    }
}