using ALifeUni.ALife.AgentPieces.Brains;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
{
    public class BehaviourBrain : IBrain
    {
        public IEnumerable<Behaviour> Behaviours
        {
            get
            {
                return behaviours;
            }
        }

        private List<Behaviour> behaviours = new List<Behaviour>();
        private Agent parent;
        private double prop = 1.0;

        public BehaviourBrain(Agent parent)
        {
            this.parent = parent;
            //Func<double> blah = this.prop;

            Func<double> bleh = () => this.prop;
            Regex englishStringParser =
                new Regex("If (([^\\s]+)( and( [^\\s]+))*) then( Wait\\(\\d+\\) to )?( [^\\s]+) with intensity( [^\\s]+)\\.");


        }

        public void ExecuteTurn()
        {
            foreach(Behaviour beh in behaviours)
            {
                beh.EvaluateBehaviour();
            }
            foreach(Action act in parent.Actions.Values)
            {
                act.AttemptEnact();
            }
        }
    }
}