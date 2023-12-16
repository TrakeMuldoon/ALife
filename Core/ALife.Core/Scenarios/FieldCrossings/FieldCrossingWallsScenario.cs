using ALife.Core.Geometry;
using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Prebuilt;

namespace ALife.Core.Scenarios.FieldCrossings
{
    [ScenarioRegistration("Field Crossing (Walls)",
        description:
        @"
4 way field crossing: (Walls)
This scenario features populations of agents all trying to reach the opposite end, by colour. 
In order to reduce the number of agents who just blindly run, there are walls criss-crossing, so there is no cardinal straight line available.
Failure cases:
If they do not reach the other end within 1900 turns, they die without reprodcing.
If they do not leave their starting zone within 200 turns, the die without reproducing.
If they bump into anything, they die.

Success Cases:
If they reach the target zone, they will restart in their own zones, and an evolved children will be spawned in each of the four zones"
     )]
    public class FieldCrossingWallsScenario : FieldCrossingScenario
    {
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
            AgentZoneSpecs = FieldCrossingHelpers.InsertOpposedZonesAndReturnZoneSpec();

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                foreach(Zone z in AgentZoneSpecs.Keys)
                {
                    CreateZonedAgent(AgentZoneSpecs[z]);
                }
            }

            List<Wall> walls = new List<Wall>();
            for(int i = 0; i < 14; i++)
            {
                int angleDelta = i % 2 == 0 ? 10 : -10;

                walls.Add(new Wall(new Geometry.Shapes.Point(50 + (i * 65), 100 + (i * 65)), 50, new Angle(angleDelta), $"x-1.{i}"));
                walls.Add(new Wall(new Geometry.Shapes.Point(950 - (i * 65), 100 + (i * 65)), 50, new Angle(90 + angleDelta), $"x-2.{i}"));
            }

            walls.ForEach(w => Planet.World.AddObjectToWorld(w));
        }
    }
}
