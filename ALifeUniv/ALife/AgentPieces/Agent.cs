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
        public readonly IBrain myBrain;

        public readonly List<SenseCluster> Senses;
        public readonly ReadOnlyDictionary<String, ActionCluster> Actions;

        public override Angle Orientation
        {
            get;
            set;
        }

        public Agent(Point birthPosition)
            : base(birthPosition
                  , 5                                               //current radius    //TODO: Hardcoded Agent Radius
                  , "Agent"                                         //Genus Label
                  , AgentIDGenerator.GetNextAgentId()               //Individual Label
                  , ReferenceValues.CollisionLevelPhysical          //Collision Level
                  , Colors.Green)                                   //Start Color       //TODO: Hardcoded start colour
        {
            CentrePoint = birthPosition;
            Orientation = new Angle(0);//TODO Agent Orientation starts at zero. Is this okay?

            InitializeAgentProperties(); //Adds any agent properties custom to Agents
            Senses = GenerateSenses();
            Actions = GenerateActions();

            //myBrain = new RandomBrain(this);
            //myBrain = new TesterBrain(this);
            //TODO: Brain Behaviour is hardcoded. IT shoudl be in the config.
            myBrain = new BehaviourBrain(this,"*", "*", "*", "*", "*");

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

            InitializeAgentProperties();
            Senses = GenerateSenses();
            Actions = GenerateActions();

            myBrain = parent.myBrain.Reproduce(this);

            //Reproduce Actions

            //Reproduce Properties
            
            //Reproduce Inputs

            //Reproduce Brain

            //Reproduce Reproduction Rules
        }

        private ReadOnlyDictionary<string, ActionCluster> GenerateActions()
        {
            //TODO: Link this somehow to world-settings
            Dictionary<string, ActionCluster> myActions = new Dictionary<string, ActionCluster>();
            List<ActionCluster> actionList = new List<ActionCluster>()
            {
                new ColorCluster(this),
                new MoveCluster(this),
                new RotateCluster(this)
            };

            actionList.ForEach((ac) => myActions.Add(ac.Name, ac));

            return new ReadOnlyDictionary<string, ActionCluster>(myActions);
        }

        private void InitializeAgentProperties()
        {
            //TODO: Link this to the config generation
            //TODO: Implement Property<int> instead of making all properties doubles.
            StatisticInput Age = new StatisticInput("Age", 0, Int32.MaxValue);
            Statistics.Add(Age.Name, Age);
            StatisticInput NumChild = new StatisticInput("NumChildrenWaiting", 0, Int32.MaxValue);
            Statistics.Add(NumChild.Name, NumChild);
            StatisticInput TimeSinceRepro = new StatisticInput("TimeSinceRepro", 0, Int32.MaxValue);
            Statistics.Add(TimeSinceRepro.Name, TimeSinceRepro);
            StatisticInput MinimumReproWait = new StatisticInput("MinimumReproWait", 0, Int32.MaxValue);
            MinimumReproWait.Value = 5;
            Statistics.Add(MinimumReproWait.Name, MinimumReproWait);
        }

        private List<SenseCluster> GenerateSenses()
        {
            List<SenseCluster> mySenses = new List<SenseCluster>();
            //TODO: Hardcoded Sense creation. This should be in config;
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
            Statistics["Age"].IncreasePropertyBy(1);
            //Reset all the senses. 
            Senses.ForEach((se) => se.GetShape().Reset());
            EndOfTurnTriggers();
        }

        public void EndOfTurnTriggers()
        {
            //TODO: Pull End of Turn triggers from Config
            if(Statistics["NumChildrenWaiting"].Value > 0)
            {
                if(Statistics["TimeSinceRepro"].Value > Statistics["MinimumReproWait"].Value)
                {
                    Reproduce(false);
                    Statistics["NumChildrenWaiting"].DecreasePropertyBy(1);
                    Statistics["TimeSinceRepro"].Value = 0;
                }
            }
            Statistics["TimeSinceRepro"].IncreasePropertyBy(1);
        }


        public override WorldObject Reproduce(bool exactCopy)
        {
            numChildren += 1;

            //Determine child position
            Point childCenter = FindAdjacentFreeSpace();

            //Create Child
            Agent child = new Agent(childCenter, this);
            Planet.World.AddObjectToWorld(child);

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
