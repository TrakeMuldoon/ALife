using System;

namespace ALifeUni.ALife.WorldObjects.Agents.Brains.BehaviourBrains
{
    public abstract class BehaviourInput
    {
        public String FullName;
        public abstract Type GetContainedType();
    }

    public class BehaviourInput<T> : BehaviourInput
    {
        public readonly Func<T> MyFunc;

        public BehaviourInput(string name, Func<T> myFuncValue)
        {
            FullName = name;
            MyFunc = myFuncValue;
        }

        public override Type GetContainedType()
        {
            return typeof(T);
        }

    }
}
