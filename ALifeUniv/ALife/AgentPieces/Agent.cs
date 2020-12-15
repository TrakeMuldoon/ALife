using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.TesterBrain;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class Agent : WorldObject
    {
        public IBrain myBrain
        {
            get;
            private set;
        }

        public List<SenseCluster> Senses
        {
            get;
            private set;
        }
        public ReadOnlyDictionary<String, ActionCluster> Actions
        {
            get;
            private set;
        }

        public AgentShadow Shadow
        {
            get;
            private set;
        }


        public Agent Parent
        {
            get;
            set;
        }
        public Agent LivingAncestor
        {
            get;
            set;
        }

        public readonly int Generation;
        public Zone Zone;
        public Zone TargetZone;
        public double StartOrientation
        {
            get;
            private set;
        }

        public Agent(Point birthPosition, Zone zone, Zone targetZone, Color color, double startOrientation)
            : base(birthPosition
                  , new Circle(birthPosition, 5)  //current radius    //TODO: Hardcoded Agent Radius
                  , "Agent"                                         //Genus Label
                  , AgentIDGenerator.GetNextAgentId()               //Individual Label
                  , ReferenceValues.CollisionLevelPhysical          //Collision Level
                  , color)                                          //Start Color
        {
            Zone = zone;
            TargetZone = targetZone;
            StartOrientation = startOrientation;
            Shape.DebugColor = Colors.PaleVioletRed;
            Shape.Orientation = new Angle(startOrientation);

            InitializeAgentProperties(); //Adds any agent properties custom to Agents
            Senses = GenerateSenses(); //TODO: Import senses from a config
            Actions = GenerateDefaultActions(); //TODO: Import senses into a uiSetting thing.

            //myBrain = new RandomBrain(this);
            //myBrain = new TesterBrain(this);
            //TODO: Brain Behaviour is hardcoded. It should be in the config.
            myBrain = new BehaviourBrain(this, "IF Age.Value GreaterThan [10] THEN Move.GoForward AT [0.2]", "*", "*", "*", "*");

            Shadow = new AgentShadow(this);
            Generation = 1;
            Parent = null;
            LivingAncestor = null;
        }

        //FOR REPRODUCTION/Cloning
        private Agent(Point birthPosition, Agent parent)
             : base(birthPosition
                  , parent.Shape.CloneShape()                                                    //shape of child is same as parent
                  , parent.GenusLabel                                                            //Genus Label
                  , AgentIDGenerator.GetNextChildId(parent.IndividualLabel, parent.NumChildren)  //Individual Label
                  , parent.CollisionLevel                                                        //Collision Level
                  , Color.FromArgb(parent.Shape.Color.A, parent.Shape.Color.R, parent.Shape.Color.G, parent.Shape.Color.B))  //Start Color
        {
            Shape.CentrePoint = birthPosition;
            StartOrientation = parent.StartOrientation;
            Shape.Orientation = new Angle(StartOrientation);
            Zone = parent.Zone;
            TargetZone = parent.TargetZone;

            Generation = parent.Generation + 1;
            Parent = parent;
            LivingAncestor = parent;
        }

        private ReadOnlyDictionary<string, ActionCluster> GenerateDefaultActions()
        {
            //TODO: Link this somehow to world-settings
            List<ActionCluster> actionList = new List<ActionCluster>()
            {
                //new ColorCluster(this),
                new MoveCluster(this),
                new RotateCluster(this)
            };

            return CreateRODForActions(actionList);
        }

        private ReadOnlyDictionary<string, ActionCluster> CreateRODForActions(List<ActionCluster> actionList)
        {
            Dictionary<string, ActionCluster> myActions = new Dictionary<string, ActionCluster>();
            actionList.ForEach((ac) => myActions.Add(ac.Name, ac));

            return new ReadOnlyDictionary<string, ActionCluster>(myActions);
        }


        private void InitializeAgentProperties()
        {
            //TODO: Link this to the config generation
            StatisticInput Age = new StatisticInput("Age", 0, Int32.MaxValue);
            Statistics.Add(Age.Name, Age);
            StatisticInput DeathTimer = new StatisticInput("DeathTimer", 0, Int32.MaxValue);
            Statistics.Add(DeathTimer.Name, DeathTimer);
            StatisticInput ZoneEscapeTimer = new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue);
        }

        private List<SenseCluster> GenerateSenses()
        {
            List<SenseCluster> mySenses = new List<SenseCluster>();
            //TODO: Hardcoded Sense creation. This should be in config;
            mySenses.Add(new EyeCluster(this, "Eye1"));
            mySenses.Add(new ProximityCluster(this, "Proximity1"));
            return mySenses;
        }

        public override void Die()
        {
            Alive = false;
            Shape.DebugColor = Colors.Maroon;
            Planet.World.ChangeCollisionLayerForObject(this, ReferenceValues.CollisionLevelDead);
            CollisionLevel = ReferenceValues.CollisionLevelDead;
        }

        public override void ExecuteDeadTurn()
        {
            //TODO: Abstract this out
            Planet.World.RemoveWorldObject(this);
        }

        public override void ExecuteAliveTurn()
        {
            Shadow = new AgentShadow(this);
            myBrain.ExecuteTurn();
            if(!Alive)
            {
                return;
            }
            //TODO: Abstract this out.
            //Increment or Decrement end of turn values
            Statistics["Age"].IncreasePropertyBy(1);
            Statistics["DeathTimer"].IncreasePropertyBy(1);
            Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);

            //Reset all the senses. 
            Senses.ForEach((se) => se.Shape.Reset());
            foreach(StatisticInput stat in Statistics.Values)
            {
                stat.Reset();
            }
            foreach(PropertyInput prop in Properties.Values)
            {
                prop.Reset();
            }
            EndOfTurnTriggers();
        }

        public void EndOfTurnTriggers()
        {
            if(Statistics["DeathTimer"].Value > 999)
            {
                Die();
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == Zone.Name)
                {
                    if(Statistics["ZoneEscapeTimer"].Value > 200)
                    {
                        Die();
                    }
                }
                else if(z.Name == TargetZone.Name)
                {
                    ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[CollisionLevel];

                    Point myPoint = Zone.Distributor.NextAgentCentre(Shape.BoundingBox.XLength, Shape.BoundingBox.YHeight);
                    Shape.CentrePoint = myPoint;
                    collider.MoveObject(this);

                    foreach(Zone zon in Planet.World.Zones.Values)
                    {
                        if(zon.Name != Zone.Name)
                        {
                            Reproduce(zon, zon.OppositeZone, new Angle(zon.OrientationDegrees), zon.OppositeZone.Color);
                        }
                    }

                    //You have a new countdown
                    Statistics["DeathTimer"].Value = 0;
                    Statistics["ZoneEscapeTimer"].Value = 0;
                }
            }
        }

        public void ProduceOffspring()
        {
            //Reproduce();
        }

        public override WorldObject Reproduce()
        {
            double bbLength = Shape.BoundingBox.XLength;
            double bbHeight = Shape.BoundingBox.YHeight;
            //Determine child position
            Point birthSpot = Zone.Distributor.NextAgentCentre(bbLength, bbHeight);
            //Point childCenter = FindAdjacentFreeSpace();
            
            ++NumChildren;
            //Create Child
            Agent child = new Agent(birthSpot, this);
            return Reproduce(child);
        }

        protected WorldObject Reproduce(Zone startZone, Zone targetZone, Angle orientation, Color color)
        {
            double bbLength = Shape.BoundingBox.XLength;
            double bbHeight = Shape.BoundingBox.YHeight;
            //Determine child position
            Point birthSpot = startZone.Distributor.NextAgentCentre(bbLength, bbHeight);

            ++NumChildren;
            //Create Child
            Agent child = new Agent(birthSpot, this);
            child.StartOrientation = orientation.Degrees;
            child.Shape.Orientation = orientation;
            child.Zone = startZone;
            child.TargetZone = targetZone;
            child.Shape.Color = Color.FromArgb(255, color.R, color.G, color.B);
            return Reproduce(child);
        }

        private WorldObject Reproduce(Agent child)
        {
            //Clone Properties
            //TODO: Should there be deviations on reproduction of properties? maybe.
            child.InitializeAgentProperties(); //This initializes all the properties to their default state. Shold be clone or config

            //Reproduce Senses
            child.Senses = new List<SenseCluster>();
            foreach(SenseCluster sc in Senses)
            {
                //TODO: Modify so these aren't "cloned"
                child.Senses.Add(sc.CloneSense(child));
            }

            //Reproduce Actions
            List<ActionCluster> acl = new List<ActionCluster>();
            foreach(ActionCluster oldAC in Actions.Values)
            {
                //Note: Actions will probably never be modified. They are supposed to be the same for everyone I think.
                acl.Add(oldAC.CloneAction(child));
            }
            child.Actions = CreateRODForActions(acl);

            //Reproduce Brain
            child.myBrain = myBrain.Reproduce(child);

            //Create shadow
            child.Shadow = new AgentShadow(child);

            //Release them out into the world
            Planet.World.AddObjectToWorld(child);

            //Return them to the caller, in case the caller wants to do stuff.
            return child;
        }

        public override WorldObject Clone()
        {
            ++NumChildren;

            double bbLength = Shape.BoundingBox.XLength;
            double bbHeight = Shape.BoundingBox.YHeight;
            //Determine child position
            Point birthSpot = Zone.Distributor.NextAgentCentre(bbLength, bbHeight);
            //Point childCenter = FindAdjacentFreeSpace();

            //Create Child
            Agent child = new Agent(birthSpot, this);

            //Clone Properties
            child.InitializeAgentProperties(); //This initializes all the properties to their default state. Shold be clone or config

            //Clone Senses
            child.Senses = new List<SenseCluster>();
            foreach(SenseCluster sc in Senses)
            {
                child.Senses.Add(sc.CloneSense(child));
            }

            //Clone Actions
            List<ActionCluster> acl = new List<ActionCluster>();
            foreach(ActionCluster oldAC in Actions.Values)
            {
                acl.Add(oldAC.CloneAction(child));
            }
            child.Actions = CreateRODForActions(acl);

            //Clone Brain
            child.myBrain = myBrain.Clone(child);

            //Create shadow
            child.Shadow = new AgentShadow(child);

            //Release them out into the world
            Planet.World.AddObjectToWorld(child);

            //Return them to the caller, in case the caller wants to do stuff.
            return child;
        }

        //THIS IS CURRENTLY BROKEN, but also not needed. If it eventually becomes needed, it will need to be fixed.
        //private Point FindAdjacentFreeSpace()
        //{
        //    //BoundingBox pbb = this.BoundingBox;
        //    EmptyObject wo = new EmptyObject(this.CentrePoint, this.Radius, this.CollisionLevel);
        //    Point movingCentrePoint = new Point(wo.CentrePoint.X, wo.CentrePoint.Y);

        //    //BoundingBox childBB = new BoundingBox(pbb.MinX, pbb.MinY, pbb.MaxX, pbb.MaxY);
        //    double diameter = this.Radius * 2;

        //    ICollisionMap collider = Planet.World.CollisionLevels[this.CollisionLevel];
        //    List<WorldObject> collisions = new List<WorldObject>();
        //    for(int distance = 1; distance < 5; distance++)
        //    {
        //        //Start checking the NE
        //        movingCentrePoint.X += diameter;
        //        movingCentrePoint.Y += diameter;
        //        for(int direction = 0; direction < 4; direction++)
        //        {
        //            for(int numSteps = 0; numSteps < distance * 2; numSteps++)
        //            {
        //                switch(direction)
        //                {
        //                    case 0: movingCentrePoint.Y -= diameter; break; //south
        //                    case 1: movingCentrePoint.X -= diameter; break; //west
        //                    case 2: movingCentrePoint.Y += diameter; break; //north
        //                    case 3: movingCentrePoint.X += diameter; break; //east
        //                    default: throw new Exception("invalid direction");
        //                }
        //                wo.CentrePoint = new Point(movingCentrePoint.X, movingCentrePoint.Y);
        //                collisions = collider.QueryForBoundingBoxCollisions(wo.BoundingBox);
        //                if(collisions.Count < 1)
        //                {
        //                    Point childCenter = wo.CentrePoint;
        //                    return childCenter;
        //                }
        //            }
        //        }
        //    }

        //    //The only way to get here is if the search algorithm above, evaluates all 24 positions and can't find anything.
        //    throw new Exception("too crowded");
        //}
    }
}
