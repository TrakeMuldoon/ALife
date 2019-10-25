using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.RandomBrains;
using ALifeUni.ALife.Brains.BehaviourBrainPieces;
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
                  , Planet.World.NextUniqueID().ToString()          //Individual Label
                  , ReferenceValues.CollisionLevelPhysical          //Collision Level
                  , Windows.UI.Colors.PeachPuff)                    //Start Color
        {
            CentrePoint = birthPosition;
            Orientation = new Angle(0);
            
            Senses = GenerateSenses();
            //Properties = GenerateAgentProperties();
            Actions = GenerateActions();

            //myBrain = new RandomBrain(this);
            myBrain = new BehaviourBrain(this);
        }

        private ReadOnlyDictionary<string, Action> GenerateActions()
        {
            //TODO: Link this somehow to world-settings
            //TODO: This probably doesn't need to be a dictionary
            Dictionary<string, Action> myActions = new Dictionary<string, Action>();
            myActions.Add("Color", new ColorAction(this));
            myActions.Add("Move", new MoveAction(this));
            myActions.Add("Rotate", new RotateAction(this));

            return new ReadOnlyDictionary<string, Action>(myActions);
        }

        private Dictionary<String, PropertyInput> GenerateAgentProperties()
        {
            //TODO: Link this to settings
            return new Dictionary<string, PropertyInput>();
        }

        private List<SenseCluster> GenerateSenses()
        {
            List<SenseCluster> mySenses = new List<SenseCluster>();
            mySenses.Add(new EyeCluster(this, "EyeCluster1"));
            return mySenses;
        }

        public override void ExecuteDeadTurn()
        {
            //TODO: Abstract this out
        }

        public override void ExecuteAliveTurn()
        {
            this.Color = Colors.Firebrick;
            myBrain.ExecuteTurn();
        }
    }
}
