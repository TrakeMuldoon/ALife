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
            //TODO: Holy Crap this is bad
            throw new NotImplementedException();
        }
    }
}