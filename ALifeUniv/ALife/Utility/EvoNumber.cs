using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Utility
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
                        , bool manualClamp)
            : base(startValue, valueMin, valueMax, manualClamp)
        {
            DeltaHardMax = deltaHardMax;
            DeltaEvoMax = deltaEvoMax;
            this.deltaMax = deltaMax;

            ValueMinMaxEvoMax = valueMinMaxEvoMax;
            ValueHardMin = valueHardMin;
            ValueHardMax = valueHardMax;
            ValueMin = valueMin;
            ValueMax = valueMax;
            val = startValue;

            StartValue = startValue;
            StartValueEvoDeltaMax = startValueEvoDeltaMax;
        }

        public EvoNumber Clone()
        {
            return new EvoNumber(StartValue, StartValueEvoDeltaMax
                                , ValueMin, ValueMax, ValueHardMin, ValueHardMax, ValueMinMaxEvoMax
                                , DeltaMax, DeltaEvoMax, DeltaHardMax
                                , ManualClamp);
        }

        public EvoNumber Evolve()
        {
            double newStartValue = EvolveANumber(StartValue, StartValueEvoDeltaMax, ValueMin, ValueMax);
            double newValueMin = EvolveANumber(ValueMin, ValueMinMaxEvoMax, ValueHardMin, ValueHardMax);
            double newValueMax = EvolveANumber(ValueMax, ValueMinMaxEvoMax, newValueMin, ValueHardMax);
            double newDeltaMax = EvolveANumber(DeltaMax, DeltaEvoMax, 0, DeltaHardMax);

            return new EvoNumber(newStartValue, StartValueEvoDeltaMax
                                , newValueMin, newValueMax, ValueHardMin, ValueHardMax, ValueMinMaxEvoMax
                                , newDeltaMax, DeltaEvoMax, DeltaHardMax
                                , ManualClamp);
        }

        private double EvolveANumber(double current, double deltaMax, double hardMin, double hardMax)
        {
            double moddedValue = current + (Planet.World.NumberGen.NextDouble() * deltaMax * 2) - deltaMax;
            double clampedValue = Math.Clamp(moddedValue, hardMin, hardMax);
            return clampedValue;
        }
    }
}
