using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace AlifeUniversal.ALife
{
    public class Agent : WorldObject
    {
        protected Brain myBrain;

        public readonly List<SenseInput> Senses;
        public readonly ReadOnlyDictionary<String, Action> Actions;

        public Agent(Point birthPosition)
            : base(birthPosition
                  , 5                                               //current radius
                  , "Agent"                                         //Genus Label
                  , Planet.World.NextUniqueID().ToString()          //Individual Label
                  , "Physical"                                      //Collision Level
                  , Windows.UI.Colors.DarkSalmon)                   //Start Color
        {
            CentrePoint = birthPosition;
            OrientationInRads = 0;
            
            //Senses = GenerateSenses();
            //Properties = GenerateAgentProperties();
            Actions = GenerateActions();

            myBrain = new Brain(this);

            
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

        private List<SenseInput> GenerateSenses()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteDeadTurn()
        {
            //TODO: Abstract this out
        }

        public override void ExecuteAliveTurn()
        {
            myBrain.ExecuteTurn();
        }

        private double radian;
        public double OrientationInRads
        {
            get { return radian; }
            set
            {
                radian = value % (6.28318);//2pi
            }
        }

    }
}
