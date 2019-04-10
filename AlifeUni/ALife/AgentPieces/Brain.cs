using System;
using System.Collections.Generic;

namespace AlifeUniversal.ALife
{
    public class Brain
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


        public Brain(Agent parent)
        {
            this.parent = parent;
            behaviours = new List<Behaviour>();
            //TODO: Config this, for now, it'll be 10

            for(int i = 0; i < 10; i ++)
            {

            }
        }

        internal void ExecuteTurn()
        {
            throw new NotImplementedException();
        }
    }
}