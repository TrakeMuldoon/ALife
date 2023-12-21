using ALife.Core.Collision;
using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.Scenarios.FieldCrossings
{
    [ScenarioRegistration("Field Crossing (Low Repro)",
    description:
        @"
4 way field crossing (Low Reproduction)
This scenario features populations of agents all trying to reach the opposite end, by colour.
Failure cases:
If they do not reach the other end within 1900 turns, they die without reprodcing.
If they do not leave their starting zone within 200 turns, the die without reproducing.

Success Cases:
If they reach the target zone, they will restart in their own zones, and two evolved children will be spawned"
     )]
    public class FieldCrossingLowReproScenario : FieldCrossingScenario
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

            //Get a new free Geometry.Shapes.Point within the start zone.
            Geometry.Shapes.Point myPoint = me.HomeZone.Distributor.NextObjectCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            me.Shape.CentrePoint = myPoint;
            collider.MoveObject(me);

            //Create two Children
            FieldCrossingScenario.CreateZonedChild(me, collider, RotatedZoneSpecs[me.TargetZone]);
            FieldCrossingScenario.CreateZonedChild(me, collider, RotatedZoneSpecs[me.HomeZone]);

            //You have a new countdown
            me.Statistics["DeathTimer"].Value = 0;
            me.Statistics["ZoneEscapeTimer"].Value = 0;
        }

        public override void PlanetSetup()
        {
            AgentZoneSpecs = FieldCrossingHelpers.InsertOpposedZonesAndReturnZoneSpec();

            int numAgents = 80;
            for(int i = 0; i < numAgents; i++)
            {
                foreach(Zone z in AgentZoneSpecs.Keys)
                {
                    CreateZonedAgent(AgentZoneSpecs[z]);
                }
            }
        }
    }
}
