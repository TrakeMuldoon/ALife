using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public class BehaviourCondition<T> : BehaviourCondition
    {
        public String text;
        private BehaviourInput origin;
        private Func<T, T, bool> comparator;
        private BehaviourInput compareTo;

        public BehaviourCondition(BehaviourInput leftSide, Func<T, T, bool> operation, BehaviourInput rightSide)
        {
            origin = leftSide;
            comparator = operation;
            compareTo = rightSide;
        }

        public override bool EvaluateSuccess()
        {
            Func<T> ogFunc = ((BehaviourInput<T>)origin).MyFunc;
            Func<T> ctFunc = ((BehaviourInput<T>)compareTo).MyFunc;
            return comparator(ogFunc(), ctFunc());
        }
    }

    public abstract class BehaviourCondition
    {
        public abstract bool EvaluateSuccess(); 
    }
}
