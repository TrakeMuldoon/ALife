using ALifeUni.ALife.AgentPieces.Brains;
using System;
using System.Collections.Generic;

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


        public BehaviourBrain(Agent parent)
        {
            this.parent = parent;
            //behaviours = new List<Behaviour>();
            ////TODO: Config this, for now, it'll be 10

            //for(int i = 0; i < 10; i ++)
            //{

            //}
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