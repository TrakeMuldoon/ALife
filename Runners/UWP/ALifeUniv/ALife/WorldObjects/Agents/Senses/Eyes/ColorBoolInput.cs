using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.WorldObjects.Agents.Senses.Eyes
{
    class ColorBoolInput : SenseInput<bool>
    {
        Func<WorldObject, double> getColor;

        public ColorBoolInput(string name, Func<WorldObject, double> getColor) : base(name)
        {
            this.getColor = getColor;
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            if(collisions.Count == 0)
            {
                Value = false;
                return;
            }

            foreach(WorldObject wo in collisions)
            {
                if(getColor(wo) > 0)
                {
                    Value = true;
                    return;
                }
            }

            Value = false;
            return;
        }
    }
}
