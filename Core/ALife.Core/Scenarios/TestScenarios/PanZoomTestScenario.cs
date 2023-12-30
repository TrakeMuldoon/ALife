using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Prebuilt;
using System.Collections.Generic;

namespace ALife.Core.Scenarios.TestScenarios
{
    [ScenarioRegistration("Test - PanZoom", description: "Lorum Ipsum", debugModeOnly: true)]
    public class PanZoomTestScenario : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, System.Drawing.Color colour, double startOrientation)
        {
            throw new System.NotImplementedException();
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            //Do Nothing
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //Do Nothing
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth { get { return 800; } }

        public virtual int WorldHeight { get { return 800; } }

        public bool FixedWidthHeight => true;

        public virtual void PlanetSetup()
        {
            Zone blueZone = new Zone("Blue", "random", System.Drawing.Color.Blue, new Point(0, 0), 50, WorldWidth);
            Planet.World.AddZone(blueZone);

            List<Wall> walls = new List<Wall>();
            for(int i = 0; i < 13; ++i)
            {
                for(int j = 0; j < 15; ++j)
                {
                    walls.Add(new Wall(new Point(50 + (50 * j), 30 + ( i * 60)), 50, new Angle(85 - (j * 6) - (i * 2)), $"{j}.{i}"));
                }
            }

            foreach(Wall w in walls)
            {
                Planet.World.AddObjectToWorld(w);
            }
        }

        public void GlobalEndOfTurnActions()
        {
            //Do Nothing
        }

    }
}
