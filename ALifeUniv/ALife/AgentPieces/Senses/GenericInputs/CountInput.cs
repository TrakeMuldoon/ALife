using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public class CountInput : SenseInput<int>
    {
        public CountInput(string name) : base(name)
        {
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            Value = collisions.Count;
        }
    }
}
