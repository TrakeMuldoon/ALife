using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
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

            //Get a new free point within the start zone.
            Point myPoint = me.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            me.Shape.CentrePoint = myPoint;
            collider.MoveObject(me);

            //Create two Children
            FieldCrossingScenario.CreateZonedChild(me, collider, RotatedZoneSpecs[me.TargetZone]);
            FieldCrossingScenario.CreateZonedChild(me, collider, RotatedZoneSpecs[me.Zone]);

            //You have a new countdown
            me.Statistics["DeathTimer"].Value = 0;
            me.Statistics["ZoneEscapeTimer"].Value = 0;
        }

        public override void PlanetSetup()
        {
            AgentZoneSpecs  = FieldCrossingHelpers.InsertOpposedZonesAndReturnZoneSpec();

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