using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{

    public class EvoNumber : BoundedNumber
    {
        double startValue;
        public double StartValue
        {
            get { return startValue; }
            set
            {
                double delta = value - val;
                double realDelta = Math.Clamp(delta, -StartValueEvoDeltaMax, StartValueEvoDeltaMax);
                double newVal = val + realDelta;
                startValue = Math.Clamp(newVal, ValueMin, ValueMax);
            }

        }
        public double StartValueEvoDeltaMax;

        double val;
        public override double Value
        {
            get { return val; }
            set 
            {
                //Cannot change more than the deltaMax
                double delta = value - val;
                double realDelta = Math.Clamp(delta, -DeltaMax, DeltaMax);
                if(realDelta != delta)
                {
                    //TODO: REmove this if statement, it's only for debugging.
                    throw new Exception("BLAHBLAHBLAH");
                }
                double newVal = val + realDelta;
                if(ManualClamp)
                {
                    val = newVal;
                }
                else
                {
                    val = Math.Clamp(newVal, ValueMin, ValueMax);
                }
            }
        }

        double valueMin;
        public override double ValueMin
        {
            get { return valueMin; }
            set { valueMin = value >= ValueHardMin ? value : ValueHardMin; }
        }
        double valueMax;
        public override double ValueMax
        {
            get { return valueMax; }
            set { valueMax = value <= ValueHardMax ? value : ValueHardMax; }
        }
        public double ValueHardMin;
        public double ValueHardMax;
        public double ValueMinMaxEvoMax;

        double deltaMax;
        public double DeltaMax
        {
            get { return deltaMax; }
            set 
            { 
                double delta = value - val;
                double realDelta = Math.Clamp(delta, -DeltaEvoMax, DeltaEvoMax);
                double newVal = val + realDelta;
                deltaMax = Math.Clamp(newVal, 0, DeltaHardMax);
            }
        }
        public double DeltaEvoMax;
        public double DeltaHardMax;

        public EvoNumber(double startValue, double startValueEvoDeltaMax
                        , double valueMin, double valueMax, double valueHardMin, double valueHardMax, double valueMinMaxEvoMax
                        , double deltaMax, double deltaEvoMax, double deltaHardMax
                        , double increment, bool manualClamp)
            : base(startValue, valueMin, valueMax, increment, manualClamp)
        {
            DeltaHardMax = deltaHardMax;
            DeltaEvoMax = deltaEvoMax;
            this.deltaMax = deltaMax;

            ValueMinMaxEvoMax = valueMinMaxEvoMax;
            ValueHardMin = valueHardMin;
            ValueHardMax = valueHardMax;
            ValueMin = valueMin;
            ValueMax = valueMax;
            Value = startValue;

            StartValue = startValue;
            StartValueEvoDeltaMax = startValueEvoDeltaMax;
        }
    }
}
