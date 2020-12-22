using System;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public class BehaviourCondition<T> : BehaviourCondition
    {
        public String Text;
        public bool LastState;
        private BehaviourInput origin;
        private Func<T, T, bool> comparator;
        private string comparatorString;
        private BehaviourInput compareTo;

        public BehaviourCondition(BehaviourInput leftSide, BehaviourInput rightSide, Func<T, T, bool> operation, string opString)
        {
            origin = leftSide;
            comparator = operation;
            compareTo = rightSide;
            comparatorString = opString;

            Text = origin.FullName + " " + opString + " " + rightSide.FullName;
        }
        protected BehaviourCondition()
        {

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

        public override string ToString()
        {
            return Text;
        }
    }

    public abstract class BehaviourCondition
    {
        public abstract override string ToString();
        public abstract bool EvaluateSuccess();
    }
}
