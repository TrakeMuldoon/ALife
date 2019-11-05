using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
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
        private Agent parent;
        private double prop = 1.0;
        private BehaviourCabinet behaviorCabinet;

        public BehaviourBrain(Agent parent, params string [] behaviourStrings)
        {
            this.behaviorCabinet = new BehaviourCabinet(parent);
            this.parent = parent;
            foreach(string b in behaviourStrings)
            {
                behaviours.Add(new Behaviour(b, behaviorCabinet));
            }
        }

        BehaviourWaitQueue bwq = new BehaviourWaitQueue();

        public void ExecuteTurn()
        {
            foreach (SenseCluster sc in parent.Senses)
            {
                sc.Detect();
            }
            foreach (Behaviour beh in behaviours)
            {
                beh.EvaluateBehaviour(bwq);
            }
            foreach(System.Action actionWithIntensity in bwq.ExecuteTurn())
            {
                actionWithIntensity();
            }
            foreach(Action act in parent.Actions.Values)
            {
                act.AttemptEnact();
            }
        }
    }
}