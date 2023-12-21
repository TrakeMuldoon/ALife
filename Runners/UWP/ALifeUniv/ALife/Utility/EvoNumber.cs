using System;

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
                    //TODO: Remove this if statement, it's only for debugging.
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

        private static double EvolveANumber(double current, double deltaMax, double hardMin, double hardMax)
        {
            if(deltaMax == 0)
            {
                return current;
            }
            double mean = 0;
            double stdDev = 0.2; //TODO: This is a magic number to approximate the distribution I like.

            double u1 = 1.0 - Planet.World.NumberGen.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - Planet.World.NumberGen.NextDouble(); //uniform(0,1] random doubles
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1))
                                   * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal;     //random normal(mean,stdDev^2)

            double delta = randNormal * deltaMax;
            //double delta = (Planet.World.NumberGen.NextDouble() * deltaMax)
            //               + (Planet.World.NumberGen.NextDouble() * deltaMax)
            //               - deltaMax;

            double moddedValue = current + delta;
            double clampedValue = Math.Clamp(moddedValue, hardMin, hardMax);
            return clampedValue;
        }
    }
}
