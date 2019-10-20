using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public class BehaviourCondition<T> : BehaviourCondition
    {
        public Input<T> inputTarget;
        public Func<T, T, bool> comparator;
        public T compareTo;

        public override bool EvaluateCondition()
        {
            return comparator(inputTarget.Value, compareTo);
        }
    }

    public abstract class BehaviourCondition
    {
        public abstract bool EvaluateCondition(); 
    }
}
