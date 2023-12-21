using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios.ScenarioHelpers
{
    public struct AgentZoneSpec
    {
        public Zone StartZone;
        public Zone TargetZone;
        public Color AgentColor;
        public int StartOrientation;
        public AgentZoneSpec(Zone start, Zone target, Color color, int ori)
        {
            StartZone = start;
            TargetZone = target;
            AgentColor = color;
            StartOrientation = ori;
        }
    }

    public static class FieldCrossingHelpers
    {
        public static Dictionary<Zone, AgentZoneSpec> InsertOpposedZonesAndReturnZoneSpec()
        {
            double height = Planet.World.WorldHeight;
            double width = Planet.World.WorldWidth;

            Dictionary<Zone, AgentZoneSpec> zoneSpecs = new Dictionary<Zone, AgentZoneSpec>();
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

            Planet.World.AddZone(red);
            Planet.World.AddZone(blue);
            Planet.World.AddZone(green);
            Planet.World.AddZone(orange);

            zoneSpecs.Add(red, new AgentZoneSpec(red, blue, Colors.Blue, 0));
            zoneSpecs.Add(green, new AgentZoneSpec(green, orange, Colors.Orange, 90));
            zoneSpecs.Add(blue, new AgentZoneSpec(blue, red, Colors.Red, 180));
            zoneSpecs.Add(orange, new AgentZoneSpec(orange, green, Colors.Green, 270));

            return zoneSpecs;
        }
    }
}
