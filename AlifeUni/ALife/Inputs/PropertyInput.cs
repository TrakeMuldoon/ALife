using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public class PropertyInput : Input<Double>
    {
        public double PropertyMaximum;
        public double PropertyMinimum;

        public void IncreasePropertyBy(double value)
        {
            if(value < 0)
            {
                throw new Exception("Negative Value for 'increase property'");
            }
            if (value == 0)
            {
                return;
            }
            double temp = Value + value;
            if(temp > PropertyMaximum)
            {
                temp = PropertyMaximum;
            }

            Value = temp;
            modified = true;
        }

        public void DecreasePropertyBy(double value)
        {
            if (value < 0)
            {
                throw new Exception("Negative Value for 'decrease property'");
            }
            if(value == 0)
            {
                return;
            }
            double temp = Value - value;
            if (temp < PropertyMinimum)
            {
                temp = PropertyMinimum;
            }

            Value = temp;
            modified = true;
        }

        public void ChangePropertyTo(double value)
        {
            double temp = value;
            if(temp == Value)
            {
                return;
            }
            if (temp < PropertyMinimum)
            {
                temp = PropertyMinimum;
            }
            else if(temp > PropertyMaximum)
            {
                temp = PropertyMaximum;
            }

            Value = temp;
            modified = true;
        }
    }
}
