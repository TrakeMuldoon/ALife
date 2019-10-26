using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Inputs.SenseClusters
{
    public class RedInput : SenseInput<double>
    {
        public RedInput(string name) : base(name)
        {
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            int count = 0;
            double redness = 0;
            foreach(WorldObject wo in collisions)
            {
                redness += wo.Color.R;
                count++;
            }
            if(count == 0)
            {
                throw new Exception("Should not have no collisions");
                //Value = 0;
                //return;
            }
            double average = redness / count;
            double betweenOneAndZero = average / byte.MaxValue;
            Value = betweenOneAndZero;
        }
    }
}
