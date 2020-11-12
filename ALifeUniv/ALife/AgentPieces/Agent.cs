using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.RandomBrains;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.Inputs;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class Agent : WorldObject
    {
        protected IBrain myBrain;

        public readonly List<SenseCluster> Senses;
        public readonly ReadOnlyDictionary<String, Action> Actions;

        public override Angle Orientation
        {
            get;
            set;
        }

        public Agent(Point birthPosition)
            : base(birthPosition
                  , 5                                               //current radius
                  , "Agent"                                         //Genus Label
                  , AgentIDGenerator.GetNextAgentId()               //Individual Label
                  , ReferenceValues.CollisionLevelPhysical          //Collision Level
                  , Colors.Green)                                   //Start Color
        {
            CentrePoint = birthPosition;
            Orientation = new Angle(0);

            InitializeAgentProperties(); //Adds any agent properties custom to Agents
            Senses = GenerateSenses();
            Actions = GenerateActions();

            //myBrain = new RandomBrain(this);
            //myBrain = new TesterBrain(this);
            myBrain = new BehaviourBrain(this,
                "IF Eye1.SeeSomething.Value Equals Eye1.IsRed.Value AND Eye1.HowRed.Value GreaterThan [0.1] THEN WAIT [3] TO Move AT [0.8]",
                //"IF Eye1.HowGreen.Value LessThan [0.8] THEN Color AT Eye1.HowGreen.Value",
                "IF Eye1.SeeSomething.Value Equals [False] THEN Move AT [1.0]",
                "IF Eye1.SeeSomething.Value Equals [False] THEN Rotate AT [0.3]",
                "*");

            this.DebugColor = Colors.PaleVioletRed;
        }

        private Agent(Point birthPosition, Agent parent)
             : base(birthPosition
                  , parent.Radius                                                                //current radius
                  , parent.GenusLabel                                                            //Genus Label
                  , AgentIDGenerator.GetNextChildId(parent.IndividualLabel, parent.numChildren)  //Individual Label
                  , parent.CollisionLevel                                                        //Collision Level
                  , parent.Color)                                                                //Start Color
        {
            CentrePoint = birthPosition;
            Orientation = new Angle(0);

            DebugColor = Colors.Blue;

            Senses = new List<SenseCluster>();
            Actions = GenerateActions();
            InitializeAgentProperties();
            myBrain = new BehaviourBrain(this);

            //Reproduce Actions

            //Reproduce Properties
            
            //Reproduce Inputs

            //Reproduce Brain

            //Reproduce Reproduction Rules
        }

        private ReadOnlyDictionary<string, Action> GenerateActions()
        {
            //TODO: Link this somehow to world-settings
            Dictionary<string, Action> myActions = new Dictionary<string, Action>();
            List<Action> actionList = new List<Action>()
            {
                new ColorAction(this),
                new MoveAction(this),
                new RotateAction(this)
            };

            actionList.ForEach((ac) => myActions.Add(ac.Name, ac));

            return new ReadOnlyDictionary<string, Action>(myActions);
        }

        private void InitializeAgentProperties()
        {
            //TODO: Link this to the config generation
            PropertyInput Age = new PropertyInput("Age", 0, Double.MaxValue);
            PropertyInput ReproAge = new PropertyInput("ReproAge", 0, Double.MaxValue);
            Random r = new Random();
            ReproAge.Value = Math.Floor(900 * r.NextDouble()) + 100;

            

            Properties.Add(ReproAge.Name, ReproAge);
            Properties.Add(Age.Name, Age);
        }

        private List<SenseCluster> GenerateSenses()
        {
            List<SenseCluster> mySenses = new List<SenseCluster>();
            mySenses.Add(new EyeCluster(this, "Eye1"));
            return mySenses;
        }

        public override void Die()
        {
            this.Alive = false;
            this.DebugColor = Colors.Maroon;
            Planet.World.ChangeCollisionLayerForObject(this, ReferenceValues.CollisionLevelDead);
        }

        public override void ExecuteDeadTurn()
        {
            //TODO: Abstract this out
        }

        public override void ExecuteAliveTurn()
        {
            myBrain.ExecuteTurn();
            Properties["Age"].IncreasePropertyBy(1.0);
            //Reset all the senses. 
            Senses.ForEach((se) => se.GetShape().Reset());
            CheckAndReproduce();
        }

        public void CheckAndReproduce()
        {
            if((Properties["Age"].Value % Properties["ReproAge"].Value) == 0)
            {
                WorldObject child = Reproduce(false);
                Planet.World.AddObjectToWorld(child);
            }
        }


        public override WorldObject Reproduce(bool exactCopy)
        {
            numChildren += 1;

            //Determine child position
            Point childCenter = FindAdjacentFreeSpace();

            //Create Child
            Agent child = new Agent(childCenter, this);
            return child;
        }

        private Point FindAdjacentFreeSpace()
        {
            BoundingBox pbb = this.BoundingBox;
            
            BoundingBox childBB = new BoundingBox(pbb.MinX, pbb.MinY, pbb.MaxX, pbb.MaxY);
            double diameter = this.Radius * 2;
            
            ICollisionMap collider = Planet.World.CollisionLevels[this.CollisionLevel];
            List<WorldObject> collisions = new List<WorldObject>();
            bool found = false;
            for(int distance = 1; distance < 5;  distance++)
            {
                childBB.MinX += diameter;
                childBB.MaxX += diameter;
                childBB.MinY += diameter;
                childBB.MaxY += diameter;
                for(int direction = 0; direction < 4; direction++)
                {
                    for(int numSteps = 0; numSteps < distance * 2; numSteps++)
                    {
                        switch(direction)
                        {
                            case 0://south
                                childBB.MinY -= diameter;
                                childBB.MaxY -= diameter;
                                break;
                            case 1://west
                                childBB.MinX -= diameter;
                                childBB.MaxX -= diameter;
                                break;
                            case 2://north
                                childBB.MinY += diameter;
                                childBB.MaxY += diameter;
                                break;
                            case 3://east
                                childBB.MinX += diameter;
                                childBB.MaxX += diameter;
                                break;
                            default: throw new Exception("invalid direction");
                        }
                        collisions = collider.QueryForBoundingBoxCollisions(childBB);
                        if(collisions.Count < 1)
                        {
                            found = true;
                            goto Found;
                        }
                    }
                }
            }
            Found: 
            if(!found)
            {
                throw new Exception("too crowded");
            }

            Point childCenter = new Point((childBB.MinX + childBB.MaxX) / 2, (childBB.MinY + childBB.MaxY) / 2);
            return childCenter;
        }
    }
}
