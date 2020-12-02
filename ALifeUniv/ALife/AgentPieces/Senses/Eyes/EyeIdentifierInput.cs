﻿using System;
using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public class EyeIdentifierInput : SenseInput<string>
    {
        public EyeIdentifierInput(string name) : base(name)
        {
            Value = String.Empty;
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            if(collisions.Count > 0)
            {
                Value = collisions[0].IndividualLabel;
            }
            else
            {
                Value = String.Empty;
            }
        }
    }
}