using System.Collections.Generic;

namespace ALifeUni.ALife.Agents.Senses.Generic
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
