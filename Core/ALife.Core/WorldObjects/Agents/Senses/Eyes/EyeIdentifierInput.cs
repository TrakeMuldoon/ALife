﻿using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses.Eyes
{
    public class EyeIdentifierInput : SenseInput<string>
    {
        public EyeIdentifierInput(string name) : base(name)
        {
            Value = string.Empty;
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            if(collisions.Count > 0)
            {
                Value = collisions[0].IndividualLabel;
            }
            else
            {
                Value = string.Empty;
            }
        }
    }
}
