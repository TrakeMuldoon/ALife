using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
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

    public abstract class Input<T> : Input
    { 
        private T myValue;
        public T Value
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
