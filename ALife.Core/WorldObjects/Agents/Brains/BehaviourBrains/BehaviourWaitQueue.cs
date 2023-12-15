using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Brains.BehaviourBrains
{
    public class BehaviourWaitQueue
    {
        List<Action>[] waitQueue;
        int currPosition = 0;
        int maxWaitTurns;

        public BehaviourWaitQueue(int waitMax)
        {
            maxWaitTurns = waitMax;
            waitQueue = new List<Action>[waitMax];
            for(int i = 0; i < waitMax; i++)
            {
                waitQueue[i] = new List<Action>();
            }
        }

        public void AddAction(Action activity, int waitTurns)
        {
            waitQueue[(currPosition + waitTurns) % maxWaitTurns].Add(activity);
        }

        public IEnumerable<Action> PopThisTurnsActions()
        {
            IEnumerable<Action> toReturn = waitQueue[currPosition];
            waitQueue[currPosition] = new List<Action>();
            currPosition = (currPosition + 1) % maxWaitTurns;
            return toReturn;
        }
    }
}
