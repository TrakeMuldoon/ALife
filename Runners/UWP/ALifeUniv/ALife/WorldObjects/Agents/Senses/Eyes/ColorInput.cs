using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.WorldObjects.Agents.Senses.Eyes
{
    public class ColorInput : SenseInput<double>
    {
        Func<WorldObject, double> getColor;

        public ColorInput(string name, Func<WorldObject, double> getColor) : base(name)
        {
            this.getColor = getColor;
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            if(collisions.Count == 0)
            {
                Value = 0;
                return;
            }

            int count = 0;
            double colourness = 0;
            foreach(WorldObject wo in collisions)
            {
                colourness += getColor(wo);
                count++;
            }
            double average = colourness / count;
            double betweenOneAndZero = average / byte.MaxValue;
            Value = betweenOneAndZero;
        }
    }
}

