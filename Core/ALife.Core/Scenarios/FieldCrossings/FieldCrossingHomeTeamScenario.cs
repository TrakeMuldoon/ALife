using ALife.Core.Collision;
using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Prebuilt;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.Scenarios.FieldCrossings
{
    [ScenarioRegistration("Field Crossing (Home Team)",
    description:
        @"
4 way field crossing (Home Team)
This scenario features populations of agents all trying to reach the opposite end, by colour. 
They are not redistributed randomly. Each colour reproduces only to their own side, and cannot reproduce if they are the most populous team.
Failure cases:
If they do not reach the other end within 1900 turns, they die without reproducing.
If they do not leave their starting zone within 200 turns, the die without reproducing.
If they crash into another agent, both will die without reproducing.

Success Cases:
If they reach the target zone, they will restart in their own zones, and two evolved children will be spawned"
     )]
    [SuggestedSeed(1819625171, "Fun scenario!!!")]
    [SuggestedSeed(1592661105, "Fun scenario!!!")]
    [SuggestedSeed(301854669, "Fun scenario!!!")]
    [SuggestedSeed(974430841, "Fun scenario!!!")]
    [SuggestedSeed(2125869630, "Fun scenario!!!")]
    [SuggestedSeed(1989326141, "Fun scenario!!!")]

    public class FieldCrossingHomeTeamScenario : FieldCrossingScenario
    {
        public Dictionary<Zone, int> agentsPerTeam = new Dictionary<Zone, int>();
        public int reproductionThreshold = 0;

        protected override void VictoryBehaviour(Agent me)
        {
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

            //Get a new free Point within the start zone.
            Point myPoint = me.HomeZone.Distributor.NextObjectCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            me.Shape.CentrePoint = myPoint;
            collider.MoveObject(me);

            //You have a new countdown
            me.Statistics["DeathTimer"].Value = 0;
            me.Statistics["ZoneEscapeTimer"].Value = 0;

            //Create two Children
            if(agentsPerTeam[me.HomeZone] <= reproductionThreshold)
            {
                FieldCrossingHelpers.CreateZonedChild(me, collider, AgentZoneSpecs[me.HomeZone]);
                FieldCrossingHelpers.CreateZonedChild(me, collider, AgentZoneSpecs[me.HomeZone]);
            }
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

            foreach(Zone z in AgentZoneSpecs.Keys)
            {
                agentsPerTeam.Add(z, numAgents);
            }
            reproductionThreshold = numAgents;

            List<Wall> walls = new List<Wall>()
            {
                new Wall(new Point(20, 40), 40, new Angle(180), "NW -"),
                new Wall(new Point(40, 20), 40, new Angle(90), "NW |"),
                new Wall(new Point(20, 960), 40, new Angle(180), "SW -"),
                new Wall(new Point(40, 980), 40, new Angle(90), "SW |"),
                new Wall(new Point(980, 40), 40, new Angle(180), "NE -"),
                new Wall(new Point(960, 20), 40, new Angle(90), "NE |"),
                new Wall(new Point(980, 960), 40, new Angle(180), "SE -"),
                new Wall(new Point(960, 980), 40, new Angle(90), "SE |"),
            };

            walls.ForEach(w => Planet.World.AddObjectToWorld(w));
        }

        public override void GlobalEndOfTurnActions()
        {

            //reset reproduction threshold
            IEnumerable<Agent> aliveAgents = Planet.World.AllActiveObjects.OfType<Agent>();

            foreach(Zone z in AgentZoneSpecs.Keys)
            {
                agentsPerTeam[z] = 0;
            }

            foreach(Agent a in aliveAgents)
            {
                agentsPerTeam[a.HomeZone] += 1;
            }

            reproductionThreshold = 0;
            List<int> teamValues = agentsPerTeam.Values.ToList<int>();
            teamValues.Sort();
            reproductionThreshold = teamValues[2];
        }
    }
}
