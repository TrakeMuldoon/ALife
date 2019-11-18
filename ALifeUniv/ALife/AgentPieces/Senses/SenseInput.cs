using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public interface SenseInput
    {
        string Name
        {
            get;
            set;
        }
        void SetValue(List<WorldObject> collisions);
    }

    public abstract class SenseInput<T> : Input<T>, SenseInput
    {
        public SenseInput(string name) : base(name)
        {
        }
        
        public abstract void SetValue(List<WorldObject> collisions);
    }
}
