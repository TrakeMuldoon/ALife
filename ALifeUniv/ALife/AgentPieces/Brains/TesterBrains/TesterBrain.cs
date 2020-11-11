using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.RandomBrains
{
    class TesterBrain : IBrain
    {
        private Agent parent;

        public TesterBrain(Agent parent)
        {
            this.parent = parent;
        }

        public void ExecuteTurn()
        {
            

            foreach(SenseCluster sc in parent.Senses)
            {
                sc.Detect();
            }

            parent.Actions["Rotate"].Intensity = 0.1;
            parent.Actions["Rotate"].AttemptEnact();
            
            //Reset means that the bounding box cache is wiped out
            //Until the next time it is reset (during detect) it will be using the cached one
            foreach (SenseCluster sc in parent.Senses)
            {
                sc.GetShape().Reset();
            }
        }
    }
}
