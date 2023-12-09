using ALifeUni.ALife.Agents;
using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Utility.WorldObjects;
using System.Collections.Generic;
using Windows.Foundation;

namespace ALifeUni.ALife.Scenarios.FieldCrossings
{
    [ScenarioRegistration("Field Crossing With Walls", description: "Lorum Ipsum")]
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

                walls.Add(new Wall(new Point(50 + (i * 65), 100 + (i * 65)), 50, new Angle(angleDelta), $"x-1.{i}"));
                walls.Add(new Wall(new Point(950 - (i * 65), 100 + (i * 65)), 50, new Angle(90 + angleDelta), $"x-2.{i}"));
            }

            walls.ForEach(w => Planet.World.AddObjectToWorld(w));
        }
    }
}
