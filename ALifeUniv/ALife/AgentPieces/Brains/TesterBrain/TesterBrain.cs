namespace ALifeUni.ALife.AgentPieces.Brains.TesterBrain
{
    class TesterBrain : IBrain
    {
        private Agent body;

        public TesterBrain(Agent self)
        {
            this.body = self;
        }

        public IBrain Clone(Agent newSelf)
        {
            return new TesterBrain(newSelf);
        }

        public IBrain Reproduce(Agent newSelf)
        {
            return new TesterBrain(newSelf);
        }


        public void ExecuteTurn()
        {


            foreach(SenseCluster sc in body.Senses)
            {
                sc.Detect();
            }

            body.Actions["Rotate"].SubActions["TurnLeft"].Intensity = 0.1;
            body.Actions["Move"].SubActions["GoForward"].Intensity = 0.5;

            foreach(ActionCluster ac in body.Actions.Values)
            {
                ac.ActivateAction();
            }
        }
    }
}
