using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses.GenericInputs
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
