using ALife.Core.WorldObjects.Agents;
using System;
using System.Diagnostics;

namespace ALife.Core.WorldObjects.Agents
{
    public abstract class Input
    {
        public String Name
        {
            get;
            set;
        }

        public Input(String name)
        {
            Name = name;
        }

        public abstract Type GetContainedType();

        public abstract String GetValueAsString();
    }

    [DebuggerDisplay("{Name}:{Value}")]
    public abstract class Input<T> : Input
    {
        private T myValue;
        public virtual T Value
        {
            get
            {
                return myValue;
            }
            set
            {
                mostRecentValue = myValue;
                myValue = value;
                modified = true;
            }
        }

        private T mostRecentValue;
        public T MostRecentValue
        {
            get
            {
                return mostRecentValue;
            }
        }

        protected bool modified;
        public bool Modified
        {
            get
            {
                return modified;
            }
        }

        public Input(string name) : base(name)
        {
        }

        public void Reset()
        {
            modified = false;
        }

        public override Type GetContainedType()
        {
            return typeof(T);
        }

        public override string GetValueAsString()
        {
            return Value.ToString();
        }
    }
}
