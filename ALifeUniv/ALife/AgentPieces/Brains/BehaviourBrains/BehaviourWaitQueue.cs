using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Brains
{
    public class BehaviourWaitQueue
    {
        List<Action>[] waitQueue = new List<Action>[Settings.BehaviourWaitMax];
        int currPosition = 0;

        public BehaviourWaitQueue()
        {
            for(int i = 0; i < Settings.BehaviourWaitMax; i++)
            {
                waitQueue[i] = new List<Action>();
            }
        }

        public void AddAction(Action activity, int waitTurns)
        {
            waitQueue[(currPosition + waitTurns) % Settings.BehaviourWaitMax].Add(activity);
        }

        public IEnumerable<Action> PopThisTurnsActions()
        {
            IEnumerable<Action> toReturn = waitQueue[currPosition];
            waitQueue[currPosition] = new List<Action>();
            currPosition = (currPosition + 1) % Settings.BehaviourWaitMax;
            return toReturn;
        }
    }
}