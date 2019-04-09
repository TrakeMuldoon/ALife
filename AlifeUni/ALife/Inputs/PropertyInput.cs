using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    public class PropertyInput : Input
    {
        public double PropertyMaximum;
        public double PropertyMinimum;

        public void IncreasePropertyBy(double value)
        {
            if(value < 0)
            {
                throw new Exception("Negative Value for 'increase property'");
            }
            double temp = Value + value;
            if(temp > PropertyMaximum)
            {
                temp = PropertyMaximum;
            }

            Value = temp;
        }

        public void DecreasePropertyBy(double value)
        {
            if (value < 0)
            {
                throw new Exception("Negative Value for 'decrease property'");
            }
            double temp = Value - value;
            if (temp < PropertyMinimum)
            {
                temp = PropertyMinimum;
            }

            Value = temp;
        }

        public void ChangePropertyTo(double value)
        {
            double temp = value;
            if (temp < PropertyMinimum)
            {
                temp = PropertyMinimum;
            }
            else if(temp > PropertyMaximum)
            {
                temp = PropertyMaximum;
            }

            Value = temp;
        }
    }
}
