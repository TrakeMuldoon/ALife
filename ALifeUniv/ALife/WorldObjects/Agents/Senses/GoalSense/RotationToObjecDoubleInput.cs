using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Agents.Senses.GoalSense
{
    class RotationToObjectDoubleInput : SenseInput<double>
    {
        public RotationToObjectDoubleInput(string name) : base(name)
        {
        }

        //TODO: Add Deprecated tag to this so it's a compiler error, instead.
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
