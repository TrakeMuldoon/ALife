using System;

namespace ALife.Core.Utility
{
    public class BoundedNumber
    {
        private double val;
        public virtual double Value
        {
            get { return val; }

            set
            {
                if(ManualClamp)
                {
                    val = value;
                }
                else
                {
                    val = Math.Clamp(value, ValueMin, ValueMax);
                }
            }
        }
        public virtual double ValueMin
        {
            get;
            set;
        }
        public virtual double ValueMax
        {
            get;
            set;
        }
        public double Increment
        {
            get;
        }

        public bool ManualClamp;

        public BoundedNumber(double value, double minValue, double maxValue, bool manualClamp)
        {
            val = value;
            ValueMin = minValue;
            ValueMax = maxValue;
            ManualClamp = manualClamp;
        }

        public double Clamp()
        {
            val = Math.Clamp(val, ValueMin, ValueMax);
            return val;
        }
    }
}
