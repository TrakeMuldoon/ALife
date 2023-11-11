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
                                , new ROEvoNumber(startValue: 60, evoDeltaMax: 3, hardMin: 40, hardMax: 120)       //Radius
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),      //Sweep
                new EyeCluster(agent, "EyeRight"
                                , new ROEvoNumber(startValue: 20, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                                , new ROEvoNumber(startValue: -10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax: 3, hardMin: 40, hardMax: 120)       //Radius
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

            IBrain newBrain = new BehaviourBrain(agent, "IF Age.Value GreaterThan [10] THEN Move.GoForward AT [0.2]", "*", "*", "*", "*");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public override void EndOfTurnTriggers(Agent me)
        {
            if(me.Statistics["DeathTimer"].Value > 1899)
            {
                me.Die();
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(me.Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == me.Zone.Name)
                {
                    if(me.Statistics["ZoneEscapeTimer"].Value > 200)
                    {
                        me.Die();
                    }
                }
                else if(z.Name == me.TargetZone.Name)
                {
                    ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[me.CollisionLevel];

                    //Get a new free point within the start zone.
                    Point myPoint = me.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
                    me.Shape.CentrePoint = myPoint;
                    collider.MoveObject(me);

                    //Reproduce one child going the same direction
                    me.Reproduce();

                    //Reproduce one child going the other way
                    Agent reverseChild = (Agent)me.Reproduce();
                    reverseChild.Zone = me.TargetZone;
                    reverseChild.TargetZone = me.Zone;
                    Point reverseChildPoint = reverseChild.Zone.Distributor.NextAgentCentre(me.Shape.BoundingBox.XLength, me.Shape.BoundingBox.YHeight);
                    reverseChild.Shape.CentrePoint = reverseChildPoint;
                    reverseChild.Shape.Orientation.Degrees += 180;
                    reverseChild.Shape.Color = reverseChild.Zone.Color;
                    
                    collider.MoveObject(reverseChild);


                    //You have a new countdown
                    me.Statistics["DeathTimer"].Value = 0;
                    me.Statistics["ZoneEscapeTimer"].Value = 0;
                }
            }
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

            int numAgents = 80;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
                Agent bag = AgentFactory.CreateAgent("Agent", blue, red, Colors.Red, 180);
                Agent gag = AgentFactory.CreateAgent("Agent", green, orange, Colors.Orange, 90);
                Agent oag = AgentFactory.CreateAgent("Agent", orange, green, Colors.Green, 270);
            }

            Point rockCP = new Point((width / 2) + (width / 15), height / 2);
            Circle cir = new Circle(rockCP, 30);
            FallingRock fr = new FallingRock(rockCP, cir, Colors.Black);
            instance.AddObjectToWorld(fr);
        }
    }
}
