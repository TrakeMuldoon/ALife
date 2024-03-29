﻿using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses
{
    public interface SenseInput
    {
        string Name
        {
            get;
            set;
        }
        void SetValue(List<WorldObject> collisions);
    }

    public abstract class SenseInput<T> : Input<T>, SenseInput
    {
        public SenseInput(string name) : base(name)
        {
        }

        public abstract void SetValue(List<WorldObject> collisions);
    }
}
