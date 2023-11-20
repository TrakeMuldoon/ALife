/***
 * Scenario: 
 * 4 way field crossing
 * This scenario features populations of agents all trying to reach the opposite end, by colour. 
 * Failure cases:
 * If they crash into each other, or the spinning rock, they die without reproducing.
 * If they do not reach the other end within 1900 turns, they die without reprodcing.
 * If they do not leave their starting zone within 200 turns, the die without reproducing.
 * 
 * Success Cases:
 * If they reach the target zone, they will restart in their own zones, and an evolved child will be spawned in each of the four zones.
 * **/


using ALifeUni.ALife.Brains;
using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class FieldCrossingScenario : AbstractScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/
        public override string Name
        {
            get { return "Field Crossing"; }
        }

        /******************/
        /*   AGENT STUFF  */
        /******************/

        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.Zone = parentZone;
            agent.TargetZone = targetZone;

            //TODO: FIX SO THAT THE SHAPE IS A PARAMETER
            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10); //TODO: HARDCODED AGENT RADIUS

            IShape myShape = new Circle(centrePoint, 5);                        //TODO: HARDCODED AGENT RADIUS
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "EyeLeft"
                                , new ROEvoNumber(startValue: -20, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                                , new ROEvoNumber(startValue: 10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax: 5, hardMin: 30, hardMax: 100)       //Radius
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),      //Sweep
                new EyeCluster(agent, "EyeRight"
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                                , new ROEvoNumber(startValue: -10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax: 5, hardMin: 30, hardMax: 100)       //Radius
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),      //Sweep
                new GoalSenseCluster(agent, "GoalSense", targetZone)
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new BehaviourBrain(agent, "IF Age.Value GreaterThan [10] THEN Move.GoForward AT [0.2]", "*", "*", "*", "*");
            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 15, 12, 10 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public override void EndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["DeathTimer"].Value > 1899)
            {
                me.Die();
                return;
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.Zone.Name)
                {
                    if(me.Statistics["ZoneEscapeTimer"].Value > 200)
                    {
                        me.Die();
                        return;
                    }
                }
                else if(z.Name == me.TargetZone.Name)
                {
                    this.VictoryBehaviour(me);
                }
            }
        }

        protected virtual void VictoryBehaviour(Agent me)
        {
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

            //Get a new free point within the start zone.
            Point myPoint = me.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            me.Shape.CentrePoint = myPoint;
            collider.MoveObject(me);

            //Reproduce one child for each direction
            foreach(AgentZoneSpec spec in AgentZoneSpecs.Values)
            {
                CreateZonedChild(me, collider, spec);
            }

            //You have a new countdown
            me.Statistics["DeathTimer"].Value = 0;
            me.Statistics["ZoneEscapeTimer"].Value = 0;
        }

        protected static void CreateZonedChild(Agent me, ICollisionMap<WorldObject> collider, AgentZoneSpec specification)
        {
            Agent child = (Agent)me.Reproduce();
            child.Zone = specification.StartZone;
            child.TargetZone = specification.TargetZone;
            Point reverseChildPoint = child.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
            child.Shape.CentrePoint = reverseChildPoint;
            child.Shape.Orientation.Degrees = specification.StartOrientation;
            child.Shape.Color = specification.AgentColor;
            (child.Senses[2] as GoalSenseCluster).ChangeTarget(specification.TargetZone);

            collider.MoveObject(child);
        }

        public override void AgentUpkeep(Agent me)
        {
            //Increment or Decrement end of turn values
            me.Statistics["Age"].IncreasePropertyBy(1);
            me.Statistics["DeathTimer"].IncreasePropertyBy(1);
            me.Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);
        }

        public override void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //Collision means death right now
            foreach(WorldObject wo in collisions)
            {
                if(wo is Agent ag)
                {
                    ag.Die();
                }
            }
            me.Die();
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public override int WorldWidth { get { return 1000; } }

        public override int WorldHeight { get { return 1000; } }

        public override bool FixedWidthHeight { get { return false; } }

        protected struct AgentZoneSpec
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

        protected static Dictionary<Zone, AgentZoneSpec> AgentZoneSpecs = new Dictionary<Zone, AgentZoneSpec>();

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

            Point rockCP = new Point((width / 2) + (width / 3), height / 2);
            Rectangle rec = new Rectangle(rockCP, 40, 20, Colors.Black);
            FallingRock fr = new FallingRock(rockCP, rec, Colors.Black);
            instance.AddObjectToWorld(fr);
        }

        protected Agent CreateZonedAgent(AgentZoneSpec spec)
        {
            return AgentFactory.CreateAgent("Agent"
                                            , spec.StartZone
                                            , spec.TargetZone
                                            , spec.AgentColor
                                            , spec.StartOrientation);
        }

        public override void Reset()
        {
            AgentZoneSpecs.Clear();
        }
    }
}
