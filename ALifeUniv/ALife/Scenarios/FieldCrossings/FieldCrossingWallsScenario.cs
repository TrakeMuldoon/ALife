using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Utility;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios.FieldCrossings
{
    public class FieldCrossingWallsScenario : FieldCrossingScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/
        public override string Name
        {
            get { return "Field Crossing With Walls"; }
        }

        /******************/
        /*   AGENT STUFF  */
        /******************/

        public override void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            me.Die();
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public override int WorldWidth { get { return 1000; } }

        public override int WorldHeight { get { return 1000; } }

        public override bool FixedWidthHeight { get { return true; } }

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

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = CreateZonedAgent(AgentZoneSpecs[red]);
                Agent bag = CreateZonedAgent(AgentZoneSpecs[blue]);
                Agent gag = CreateZonedAgent(AgentZoneSpecs[green]);
                Agent oag = CreateZonedAgent(AgentZoneSpecs[orange]);
            }

            List<Wall> walls = new List<Wall>();
            for(int i = 0; i < 14; i++)
            {
                int angleDelta = i % 2 == 0 ? 10 : -10;

                walls.Add(new Wall(new Point(50 + (i * 65), 100 + (i * 65)), 50, new Angle(angleDelta), $"x-1.{i}"));
                walls.Add(new Wall(new Point(950 - (i * 65), 100 + (i * 65)), 50, new Angle(90 + angleDelta), $"x-2.{i}"));
            }

            walls.ForEach(w => Planet.World.AddObjectToWorld(w));
        }
    }
}
