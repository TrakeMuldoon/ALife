using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Brains
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

        private const int BehaviourWaitMax = 5; //TODO: Should this be an abstracted setting? It doesn't seem very impactful.
        private List<Behaviour> behaviours = new List<Behaviour>();
        private Agent self;
        private BehaviourCabinet behaviorCabinet;
        private BehaviourWaitQueue bwq = new BehaviourWaitQueue(BehaviourWaitMax);


        public BehaviourBrain(Agent self, params string[] behaviourStrings)
        {
            this.behaviorCabinet = new BehaviourCabinet(self);
            this.self = self;
            foreach(string behaviourString in behaviourStrings)
            {
                behaviours.Add(new Behaviour(behaviourString, behaviorCabinet, BehaviourWaitMax));
            }
        }

        private BehaviourBrain(Agent self, BehaviourBrain oldBrain, bool exact)
        {
            this.self = self;
            this.behaviorCabinet = new BehaviourCabinet(self);

            List<string> rules = new List<string>();
            oldBrain.behaviours.ForEach((beh) => rules.Add(beh.ToString()));

            if(!exact)
            {
                rules.Add("*");
                //TODO: Modification percentage hardcoded
                int baseDeletePercent = 5;
                int deletePercentPerRule = 2;
                for(int i = rules.Count; i > 0; i--)
                {
                    double percent = Planet.World.NumberGen.NextDouble() * 100;
                    int currThreshold = baseDeletePercent + (deletePercentPerRule * i);
                    if(percent < currThreshold)
                    {
                        rules.RemoveAt(i - 1);
                    }
                }
                rules.Add("*");
            }
            //else we dont need to modify the list

            foreach(string rule in rules)
            {
                behaviours.Add(new Behaviour(rule, this.behaviorCabinet, BehaviourWaitMax));
            }
        }

        public IBrain Clone(Agent newSelf)
        {
            return new BehaviourBrain(newSelf, this, true);
        }

        public IBrain Reproduce(Agent newSelf)
        {
            return new BehaviourBrain(newSelf, this, false);
        }

        public void ExecuteTurn()
        {
            foreach(SenseCluster sc in self.Senses)
            {
                sc.Detect();
            }
            foreach(Behaviour beh in behaviours)
            {
                beh.EvaluateAndEnqueue(bwq);
            }

            IEnumerable<Action> actions = bwq.PopThisTurnsActions();

            //This adds the intensity to the agents actions
            foreach(Action addIntensityToAgentAction in actions)
            {
                addIntensityToAgentAction();
            }

            //This makes the agent enact those items.
            foreach(ActionCluster act in self.Actions.Values)
            {
                act.ActivateAction();
            }
        }
    }
}