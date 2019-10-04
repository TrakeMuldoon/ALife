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

            if (randNum < 0.20)
            {
                parent.Actions["Rotate"].AttemptEnact(randNum * 3);
            }
            else if (randNum < 0.90)
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
