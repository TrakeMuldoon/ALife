using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    public abstract class Input
    {
        public abstract Type GetContainedType();
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

        public abstract T Delta();

        public override Type GetContainedType()
        {
            return typeof(T);
        }

        public string Name;
    }
}
