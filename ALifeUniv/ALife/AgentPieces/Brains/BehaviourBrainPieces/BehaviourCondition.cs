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
        public bool LastState;
        private BehaviourInput origin;
        private Func<T, T, bool> comparator;
        private BehaviourInput compareTo;

        public BehaviourCondition(BehaviourInput leftSide, BehaviourInput rightSide, Func<T, T, bool> operation)
        {
            origin = leftSide;
            comparator = operation;
            compareTo = rightSide;
        }

        public override bool EvaluateSuccess()
        {
            //TODO: potentially create an "initialized" variable. 
            //Not worth it right now, because it will cost CPU cycles, and would still throw an error.
            Func<T> ogFunc = ((BehaviourInput<T>)origin).MyFunc;
            Func<T> ctFunc = ((BehaviourInput<T>)compareTo).MyFunc;
            LastState = comparator(ogFunc(), ctFunc());
            return LastState;
        }
    }

    public abstract class BehaviourCondition
    {
        public abstract bool EvaluateSuccess(); 
    }
}
