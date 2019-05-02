using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.BehaviourBrainPieces
{
    public class BehaviourBrain
    {
        public IEnumerable<Behaviour> Behaviours
        {
            get
            {
                return behaviours;
            }
        }

        private List<Behaviour> behaviours;
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

        internal void ExecuteTurn()
        {
            //TODO: Holy Crap this is bad
            double randNum = Planet.World.NumberGen.NextDouble();
            
            if (randNum < 0.20)
            {
                parent.Actions["Rotate"].AttemptEnact(randNum * 3);
            }
            else if(randNum < 0.90)
            {
                parent.Actions["Move"].AttemptEnact((randNum - 0.33) * 3);
            }
            //else
            //{
            //    parent.Actions["Color"].AttemptEnact(0.99);
            //}
        }
    }
}