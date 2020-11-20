using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.RandomBrains
{
    class RandomBrain : IBrain
    {
        private Agent body;

        public RandomBrain(Agent body)
        {
            this.body = body;
        }

        public IBrain Clone(Agent newSelf)
        {
            return new RandomBrain(newSelf);
        }

        public IBrain Reproduce(Agent newSelf)
        {
            return new RandomBrain(newSelf);
        }

        public void ExecuteTurn()
        {
            
            foreach(SenseCluster sc in body.Senses)
            {
                sc.Detect();
            }

            foreach(ActionCluster ac in body.Actions.Values)
            {
                foreach(ActionPart ap in ac.SubActions.Values)
                {
                    double ifValue = Planet.World.NumberGen.NextDouble();
                    double intensityValue = Planet.World.NumberGen.NextDouble();

                    if(ifValue > 0.5)
                    {
                        ap.Intensity = intensityValue;

                    }
                }
                ac.ActivateAction();
            }
            
            //Reset means that the bounding box cache is wiped out
            //Until the next time it is reset (during detect) it will be using the cached one
            foreach (SenseCluster sc in body.Senses)
            {
                sc.GetShape().Reset();
            }
        }
    }
}
