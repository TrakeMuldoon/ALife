using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.RandomBrains
{
    class RandomBrain : IBrain
    {
        private Agent parent;

        public RandomBrain(Agent parent)
        {
            this.parent = parent;
        }

        public void ExecuteTurn()
        {
            
            foreach(SenseCluster sc in parent.Senses)
            {
                sc.Detect();
            }

            for(int i = 0; i < parent.Actions.Count; i++)
            {
                AgentAction act = parent.Actions.Values.ElementAt(i);
                double ifValue = Planet.World.NumberGen.NextDouble();
                double intensityValue = Planet.World.NumberGen.NextDouble();

                if(ifValue > 0.5)
                {
                    act.Intensity = intensityValue;
                    act.AttemptEnact();
                }
            }
            
            //Reset means that the bounding box cache is wiped out
            //Until the next time it is reset (during detect) it will be using the cached one
            foreach (SenseCluster sc in parent.Senses)
            {
                sc.GetShape().Reset();
            }
        }
    }
}
