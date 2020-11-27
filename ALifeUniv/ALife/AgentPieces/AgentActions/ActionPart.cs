using System;

namespace ALifeUni.ALife
{
    public class ActionPart
    {

        public ActionPart(String name, string ParentName)
        {
            Name = name;
            FullName = ParentName + "." + name;
        }

        public readonly string FullName;
        public readonly string Name;

        private double intensity;
        public double Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = value;
            }
        }
        public double IntensityLastTurn
        {
            get;
            private set;
        }

        const double IntensityMax = 1.0;
        const double IntensityMin = 0.0;

        public virtual void Reset()
        {
            IntensityLastTurn = intensity;
            intensity = 0;
        }
    }
}
