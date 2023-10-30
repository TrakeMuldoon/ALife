using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class ZoneRunnerScenario : BaseScenario
    {
        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            return base.CreateAgent(genusName, parentZone, targetZone, color, startOrientation);
        }

        public override void EndOfTurnTriggers(Agent me)
        {
            base.EndOfTurnTriggers(me);
        }

        public override void AgentUpkeep(Agent me)
        {
            base.AgentUpkeep(me);
        }

        public override string Name
        {
            get { return "Zone Runner"; }
        }

        public override int WorldWidth
        {
            get { return 1800; }
        }
        public override int WorldHeight
        {
            get { return 1800; }
        }
        public override bool FixedWidthHeight
        {
            get { return false; }
        }

        public override void PlanetSetup()
        {
            Planet instance = Planet.World;
            double height = instance.WorldHeight;
            double width = instance.WorldWidth;

            Zone red = new Zone("Red(Blue)", "Random", Colors.Red, new Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(Red)", "Random", Colors.Blue, new Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;
            blue.OppositeZone = red;
            blue.OrientationDegrees = 180;

            Zone green = new Zone("Green(Orange)", "Random", Colors.Green, new Point(0, 0), width, 100);
            Zone orange = new Zone("Orange(Green)", "Random", Colors.Orange, new Point(0, height - 40), width, 40);
            green.OppositeZone = orange;
            green.OrientationDegrees = 90;
            orange.OppositeZone = green;
            orange.OrientationDegrees = 270;

            instance.AddZone(red);
            instance.AddZone(blue);
            instance.AddZone(green);
            instance.AddZone(orange);

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
                Agent bag = AgentFactory.CreateAgent("Agent", blue, red, Colors.Red, 180);
                Agent gag = AgentFactory.CreateAgent("Agent", green, orange, Colors.Orange, 90);
                Agent oag = AgentFactory.CreateAgent("Agent", orange, green, Colors.Green, 270);
            }

            //Point rockCP = new Point((width / 2) + (width / 15), height / 2);
            //Circle cir = new Circle(rockCP, 30);
            //FallingRock fr = new FallingRock(rockCP, cir, Colors.Black);
            //instance.AddObjectToWorld(fr);

            Point rockRCP = new Point((width / 2), (height / 2) - (height / 10));
            Rectangle rec = new Rectangle(rockRCP, 100, 40, Colors.Black);
            FallingRock frR = new FallingRock(rockRCP, rec, Colors.Black);
            instance.AddObjectToWorld(frR);
        }
    }
}
