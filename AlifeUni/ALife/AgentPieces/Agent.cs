using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    public class Agent : WorldObject
    {
        protected Brain myBrain;

        List<SenseInput> Senses;
        List<Action> Actions;

        public Agent()
        {
            Senses = GenerateSenses();
            Properties = GenerateAgentProperties();
            Actions = GenerateActions();

            myBrain = new Brain();
        }

        private List<Action> GenerateActions()
        {
            //TODO: Link this somehow to world-settings
            List<Action> myActions = new List<Action>();
            myActions.Add(new ColorAction(this));
            myActions.Add(new MoveAction(this));
            myActions.Add(new TurnAction(this));
            return myActions;
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
