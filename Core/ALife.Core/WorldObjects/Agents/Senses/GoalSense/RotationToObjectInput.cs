using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses.GoalSense
{
    class RotationToObjectInput : SenseInput<int>
    {
        public RotationToObjectInput(string name) : base(name)
        {
        }

        //TODO: Add Deprecated tag to this so it's a compiler error, instead.
        public override void SetValue(List<WorldObject> collisions)
        {
            throw new InvalidOperationException("Do Not Used this SetValue on RotationToObjectInput");
        }

        public void SetValue(int newValue)
        {
            Value = newValue;
        }
    }
}
