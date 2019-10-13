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
            double randNum = Planet.World.NumberGen.NextDouble();

            foreach(SenseCluster sc in parent.Senses)
            {
                sc.Detect();
            }

            //parent.Actions["Rotate"].AttemptEnact(0.05f);

            if (randNum < 0.20)
            {
                parent.Actions["Rotate"].AttemptEnact(randNum * 5);
            }
            else if (randNum < 0.90)
            {
                parent.Actions["Move"].AttemptEnact((randNum - 0.33) * 3);
            }
            //else
            //{
            //    parent.Actions["Color"].AttemptEnact(0.99);
            //}
            
            //Reset means that the bounding box cache is wiped out
            //Until the next time it is reset (during detect) it will be using the cached one
            foreach (SenseCluster sc in parent.Senses)
            {
                sc.GetShape().Reset();
            }
        }
    }
}
