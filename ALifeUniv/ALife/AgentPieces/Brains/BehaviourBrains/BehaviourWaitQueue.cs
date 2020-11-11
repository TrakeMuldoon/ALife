using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Brains.BehaviourBrains
{
    public class BehaviourWaitQueue
    {
        List<System.Action>[] waitQueue = new List<System.Action>[Settings.BehaviourWaitMax];
        int currPosition = 0;

        public BehaviourWaitQueue()
        {
            for(int i = 0; i < Settings.BehaviourWaitMax; i++)
            {
                waitQueue[i] = new List<System.Action>();
            }
        }

        public void AddAction(System.Action activity, int waitTurns)
        {
            waitQueue[(currPosition + waitTurns) % Settings.BehaviourWaitMax].Add(activity);
        }

        public IEnumerable<System.Action> PopCurrentActions()
        {
            IEnumerable<System.Action> toReturn = waitQueue[currPosition];
            waitQueue[currPosition] = new List<System.Action>();
            currPosition = (currPosition + 1) % Settings.BehaviourWaitMax;
            return toReturn;
        }
    }
}