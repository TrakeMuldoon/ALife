using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces;
using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Brains.BehaviourBrains
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

        private List<Behaviour> behaviours = new List<Behaviour>();
        private Agent self;
        private BehaviourCabinet behaviorCabinet;

        public BehaviourBrain(Agent self, params string[] behaviourStrings)
        {
            this.behaviorCabinet = new BehaviourCabinet(self);
            this.self = self;
            foreach(string behaviourString in behaviourStrings)
            {
                behaviours.Add(new Behaviour(behaviourString, behaviorCabinet));
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
                for(int i = rules.Count; i > 0; i--)
                {
                    //TODO: Modification percentage hardcoded
                    double below10 = Planet.World.NumberGen.Next();
                    if(below10 < 0.1)
                    {
                        rules.RemoveAt(i - 1);
                        break;
                    }
                }
                rules.Add("*");
            }
            //else we dont need to modify the list

            foreach(string rule in rules)
            {
                behaviours.Add(new Behaviour(rule, this.behaviorCabinet));
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


        BehaviourWaitQueue bwq = new BehaviourWaitQueue();

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