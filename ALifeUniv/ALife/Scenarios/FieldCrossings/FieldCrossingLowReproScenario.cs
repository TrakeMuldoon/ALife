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
            Planet instance = Planet.World;
            double height = instance.WorldHeight;
            double width = instance.WorldWidth;

            Zone red = new Zone("Red(->Blue)", "Random", Colors.Red, new Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(->Red)", "Random", Colors.Blue, new Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;
            blue.OppositeZone = red;
            blue.OrientationDegrees = 180;

            Zone green = new Zone("Green(->Orange)", "Random", Colors.Green, new Point(0, 0), width, 40);
            Zone orange = new Zone("Orange(->Green)", "Random", Colors.Orange, new Point(0, height - 40), width, 40);
            green.OppositeZone = orange;
            green.OrientationDegrees = 90;
            orange.OppositeZone = green;
            orange.OrientationDegrees = 270;

            instance.AddZone(red);
            instance.AddZone(blue);
            instance.AddZone(green);
            instance.AddZone(orange);

            AgentZoneSpecs.Add(red, new AgentZoneSpec(red, blue, Colors.Blue, 0));
            AgentZoneSpecs.Add(green, new AgentZoneSpec(green, orange, Colors.Orange, 90));
            AgentZoneSpecs.Add(blue, new AgentZoneSpec(blue, red, Colors.Red, 180));
            AgentZoneSpecs.Add(orange, new AgentZoneSpec(orange, green, Colors.Green, 270));

            int numAgents = 80;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = CreateZonedAgent(AgentZoneSpecs[red]);
                Agent bag = CreateZonedAgent(AgentZoneSpecs[blue]);
                Agent gag = CreateZonedAgent(AgentZoneSpecs[green]);
                Agent oag = CreateZonedAgent(AgentZoneSpecs[orange]);
            }
        }
    }
}