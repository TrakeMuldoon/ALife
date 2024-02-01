using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses.GoalSense
{
    class RotationToObjectDoubleInput : SenseInput<double>
    {
        public RotationToObjectDoubleInput(string name) : base(name)
        {
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            throw new InvalidOperationException("Do Not Use this SetValue on RotationToObjectDoubleInput");
        }

        public void SetValue(double newValue)
        {
            Value = newValue;
        }
    }
}
