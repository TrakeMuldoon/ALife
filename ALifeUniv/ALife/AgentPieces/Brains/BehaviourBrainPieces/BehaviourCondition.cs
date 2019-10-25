using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public class BehaviourCondition<T> : BehaviourCondition
    {
        public Func<T> inputTarget;
        public Func<T, T, bool> comparator;
        public Func<T> compareTo;

        public override bool EvaluateSuccess()
        {
            return comparator(inputTarget(), compareTo());
        }
    }

    public abstract class BehaviourCondition
    {
        public abstract bool EvaluateSuccess(); 
    }
}
