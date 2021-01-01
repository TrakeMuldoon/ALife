﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{ 
    class DistanceToObjectInput : SenseInput<int>
    {
        public DistanceToObjectInput(string name) : base(name)
        {
        }

        //TODO: Add Deprecated tag to this so it's a compiler error, instead.
        public override void SetValue(List<WorldObject> collisions)
        {
            throw new InvalidOperationException("Do not use this SetValue on RotationToObjectInput");
        }

        public void SetValue(int newValue)
        {
            Value = newValue;
        }
    }
}