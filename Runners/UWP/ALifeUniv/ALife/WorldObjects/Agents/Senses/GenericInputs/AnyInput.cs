using System.Collections.Generic;

namespace ALifeUni.ALife.WorldObjects.Agents.Senses.Generic
{
    public class AnyInput : SenseInput<bool>
    {
        public AnyInput(string name) : base(name)
        {
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            Value = collisions.Count > 0;
        }
    }
}
