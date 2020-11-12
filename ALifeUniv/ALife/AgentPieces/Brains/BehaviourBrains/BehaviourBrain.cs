﻿using ALifeUni.ALife.AgentPieces.Brains;
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
        private Agent parent;
        private BehaviourCabinet behaviorCabinet;

        public BehaviourBrain(Agent parent, params string [] behaviourStrings)
        {
            this.behaviorCabinet = new BehaviourCabinet(parent);
            this.parent = parent;
            foreach(string behaviourString in behaviourStrings)
            {
                behaviours.Add(new Behaviour(behaviourString, behaviorCabinet));
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
                beh.EvaluateAndEnqueue(bwq);
            }

            IEnumerable<System.Action> actions = bwq.PopThisTurnsActions();  

            //This adds the intensity to the agents actions
            foreach(System.Action addIntensityToAction in actions)
            {
                addIntensityToAction();
            }

            //This makes the agent enact those items.
            foreach(Action act in parent.Actions.Values)
            {
                act.AttemptEnact();
            }
        }
    }
}