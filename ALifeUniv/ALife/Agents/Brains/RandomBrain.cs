using ALifeUni.ALife.Agents.AgentActions;
using ALifeUni.ALife.Agents.Senses;

namespace ALifeUni.ALife.Agents.Brains
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
        }
    }
}
