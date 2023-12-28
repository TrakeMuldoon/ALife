using ALife.Core.Utility.Maths;
using System;
using System.Diagnostics;

namespace ALife.Core.Utility.Numerics
{
    [DebuggerDisplay("{_value}")]
    public class EvoNumber : BaseObject
    {
        /// <summary>
        /// The mean for the evolution of a value.
        /// </summary>
        public static double EVOLUTION_MEAN = 0;

        /// <summary>
        /// The standard deviation for the evolution of a value.
        /// Devin: This is a magic number to approximate the distribution I like.
        /// </summary>
        public static double EVOLUTION_STANDARD_DEVIATION = 0.2;

        /// <summary>
        /// The delta allowed for the value and start value to change.
        /// </summary>
        public DeltaBoundedNumber Delta;

        /// <summary>
        /// The maximum value that the value can be.
        /// </summary>
        public BoundedNumber Maximum;

        /// <summary>
        /// The minimum value that the value can be.
        /// </summary>
        public BoundedNumber Minimum;

        /// <summary>
        /// The start value. This generally reflects the parents value.
        /// </summary>
        private double _startValue;

        /// <summary>
        /// The maximum delta that the start value can change by.
        /// </summary>
        private double _startValueEvoDeltaMax;

        /// <summary>
        /// The value.
        /// </summary>
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="startValueEvoDeltaMax"></param>
        /// <param name="valueMin"></param>
        /// <param name="valueMax"></param>
        /// <param name="valueHardMin"></param>
        /// <param name="valueHardMax"></param>
        /// <param name="valueMinMaxEvoMax"></param>
        /// <param name="deltaMax"></param>
        /// <param name="deltaEvoMax"></param>
        /// <param name="deltaHardMax"></param>
        public EvoNumber(Simulation sim, double startValue, double startValueEvoDeltaMax
                        , double valueMin, double valueMax, double valueHardMin, double valueHardMax, double valueMinMaxEvoMax
                        , double deltaMax, double deltaEvoMax, double deltaHardMax) : base(sim)
        {
            _startValue = startValue;
            _startValueEvoDeltaMax = startValueEvoDeltaMax;
            _value = startValue;
            Delta = new DeltaBoundedNumber(deltaMax, deltaEvoMax, 0, deltaHardMax);
            Minimum = new BoundedNumber(valueMin, valueHardMin, valueMinMaxEvoMax);
            Maximum = new BoundedNumber(valueMax, valueMinMaxEvoMax, valueHardMax);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="parent">The parent to clone.</param>
        public EvoNumber(EvoNumber parent) : base(parent.Simulation)
        {
            _startValue = parent._startValue;
            _startValueEvoDeltaMax = parent._startValueEvoDeltaMax;
            _value = parent._value;
            Delta = new DeltaBoundedNumber(parent.Delta);
            Minimum = new BoundedNumber(parent.Minimum);
            Maximum = new BoundedNumber(parent.Maximum);
        }

        /// <summary>
        /// Gets or sets the start value.
        /// </summary>
        public double StartValue
        {
            get => _startValue;
            set => _startValue = ExtraMath.DeltaClamp(value, _value, -_startValueEvoDeltaMax, _startValueEvoDeltaMax, Minimum.Value, Maximum.Value);
        }

        /// <summary>
        /// Gets or sets the start value evo delta maximum.
        /// </summary>
        public double StartValueEvoDeltaMax
        {
            get => _startValueEvoDeltaMax;
            set
            {
                _startValueEvoDeltaMax = value;
                StartValue = _startValue;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value
        {
            get => _value;
            set => _value = ExtraMath.DeltaClamp(value, _value, -Delta.Value, Delta.Value, Minimum.Value, Maximum.Value);
        }

        /// <summary>
        /// Evolves the number.
        /// </summary>
        /// <returns>An evolved Evo number.</returns>
        public EvoNumber GetEvolvedNumber()
        {
            double newStartValue = EvolveValue(_startValue, StartValueEvoDeltaMax, Minimum.Value, Maximum.Value);
            double newValueMin = EvolveValue(Minimum.Value, Maximum.Value, Minimum.Value, Maximum.Value);
            double newValueMax = EvolveValue(Maximum.Value, Maximum.Value, Minimum.Value, Maximum.Value);
            double newDeltaMax = EvolveValue(Delta.Value, Delta.Value, 0, Delta.Value);

            return new EvoNumber(
                Simulation, newStartValue, StartValueEvoDeltaMax
                , newValueMin, newValueMax, Minimum.Value, Maximum.Value, Maximum.Value
                , newDeltaMax, Delta.Value, Delta.Value
            );
        }

        /// <summary>
        /// Evolves a value.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="deltaMax"></param>
        /// <param name="hardMin"></param>
        /// <param name="hardMax"></param>
        /// <returns>The evolved value.</returns>
        private double EvolveValue(double current, double deltaMax, double hardMin, double hardMax)
        {
            // exit early if no change is allowed
            if(deltaMax == 0)
            {
                return current;
            }

            double u1 = 1.0 - Simulation.Random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - Simulation.Random.NextDouble(); //uniform(0,1] random doubles

            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1))
                                   * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = EVOLUTION_MEAN + EVOLUTION_STANDARD_DEVIATION * randStdNormal;     //random normal(mean,stdDev^2)

            double delta = randNormal * deltaMax;
            //double delta = (Planet.World.NumberGen.NextDouble() * deltaMax)
            //               + (Planet.World.NumberGen.NextDouble() * deltaMax)
            //               - deltaMax;

            double moddedValue = current + delta;
            double clampedValue = ExtraMath.Clamp(moddedValue, hardMin, hardMax);
            return clampedValue;
        }
    }
}
