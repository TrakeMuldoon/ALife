
using System;
using System.Diagnostics;

namespace ALifeUni.ALife.UtilityClasses
{
    [DebuggerDisplay("Deg:{Degrees}, Rads:{Radians}")]
    public class Angle
    {
        private double rads;
        public double Radians

        {
            get { return rads; }
            set
            {
                if(value < 0)
                {
                    value += (2 * Math.PI);
                }
                rads = value % (2 * Math.PI);
                degrees = rads * 180 / Math.PI;
            }
        }

        private double degrees;
        public double Degrees
        {
            get { return degrees; }
            set
            {
                if(value < 0)
                {
                    value += 360;
                }
                degrees = value % 360;
                rads = degrees * Math.PI / 180.00;
            }
        }

        public Angle(double degrees) : this(degrees, false) { }

        public Angle(double value, bool isRads)
        {
            degrees = 0;
            rads = 0;

            if(isRads == true)
            {
                Radians = value;
            }
            else
            {
                Degrees = value;
            }
        }

        public static Angle operator +(Angle a, Angle b)
            => new Angle(a.degrees + b.degrees);

        public static Angle operator -(Angle a, Angle b)
            => new Angle(a.degrees - b.degrees);
    }
}
