using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public BehaviourBrain(Agent self, params string [] behaviourStrings)
        {
            this.behaviorCabinet = new BehaviourCabinet(self);
            this.self = self;
            foreach(string behaviourString in behaviourStrings)
            {
                behaviours.Add(new Behaviour(behaviourString, behaviorCabinet));
            }
        }

        public BehaviourBrain(Agent self, BehaviourBrain oldBrain)
        {
            this.behaviorCabinet = new BehaviourCabinet(self);
            this.self = self;
            //TODO pull from config somehow
            foreach(Behaviour beh in oldBrain.Behaviours)
            {
                double belowNinety = Planet.World.NumberGen.NextDouble();
                if(belowNinety < 0.9)
                {
                    behaviours.Add(new Behaviour(beh.AsEnglish, behaviorCabinet));
                }
                //else this behaviour is dropped
            }
            double belowEightyFive = Planet.World.NumberGen.NextDouble();
            if(belowEightyFive < 0.85)
            {
                behaviours.Add(new Behaviour("*", behaviorCabinet));
            }
        }

        BehaviourWaitQueue bwq = new BehaviourWaitQueue();

        public void ExecuteTurn()
        {
            foreach (SenseCluster sc in self.Senses)
            {
                sc.Detect();
            }
            foreach (Behaviour beh in behaviours)
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