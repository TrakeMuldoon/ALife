using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public class EyeCounterInput : SenseInput<int>
    {
        public EyeCounterInput(string name) : base(name)
        {
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            Value = collisions.Count;
        }
    }
}
