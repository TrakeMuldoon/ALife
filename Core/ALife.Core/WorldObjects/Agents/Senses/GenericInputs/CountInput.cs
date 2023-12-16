using System.Collections.Generic;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents.Senses;

namespace ALife.Core.WorldObjects.Agents.Senses.GenericInputs
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
