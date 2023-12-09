using System;

namespace ALifeUni.ALife.Agents.Brains.BehaviourBrains
{
    public class BehaviourCondition<T> : BehaviourCondition
    {
        public String Text;
        public bool LastState;
        private BehaviourInput origin;
        private Func<T, T, bool> comparator;
        private BehaviourInput compareTo;

        public BehaviourCondition(BehaviourInput leftSide, BehaviourInput rightSide, Func<T, T, bool> operation, string opString)
        {
            origin = leftSide;
            comparator = operation;
            compareTo = rightSide;

            Text = origin.FullName + " " + opString + " " + rightSide.FullName;
        }
        protected BehaviourCondition()
        {

        }

        public override bool EvaluateSuccess()
        {
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
