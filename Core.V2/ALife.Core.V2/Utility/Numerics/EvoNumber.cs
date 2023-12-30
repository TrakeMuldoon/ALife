using ALife.Core.Utility.Random;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A number that can be evolved within a range.
    /// </summary>
    /// <seealso cref="ALife.Core.BaseObject"/>
    [DebuggerDisplay("EvoNumber: {Value} (Original: {OriginalValue})")]
    public struct EvoNumber
    {
        /// <summary>
        /// The mean for the evolution of a value.
        /// TODO: Find out why this is 0. And why it is a constant.
        /// </summary>
        [JsonIgnore]
        public static double EVOLUTION_MEAN = 0;

        /// <summary>
        /// The standard deviation for the evolution of a value.
        /// Devin: This is a magic number to approximate the distribution I like.
        /// </summary>
        [JsonIgnore]
        public static double EVOLUTION_STANDARD_DEVIATION = 0.2;

        /// <summary>
        /// Gets the value maximum and minimum evolution delta maximum. This is the maximum amount htat hte ValueMaximum
        /// and ValueMinimum can change when they are evolved.
        /// </summary>
        /// <value>The value maximum and minimum evolution delta maximum.</value>
        public double ValueMaximumAndMinimumEvolutionDeltaMax;

        /// <summary>
        /// The actual value represented by this object.
        /// </summary>
        protected double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="valueMaximumAndMinimumEvolutionDeltaMax">The value maximum and minimum evolution delta maximum.</param>
        /// <param name="valueDelta">The value delta.</param>
        /// <param name="cloneBoundedNumbers">if set to <c>true</c> [clone bounded numbers].</param>
        [JsonConstructor]
        public EvoNumber(double value, double originalValueEvolutionDeltaMax, BoundedNumber minimumValue, BoundedNumber maximumValue, double valueMaximumAndMinimumEvolutionDeltaMax, DeltaBoundedNumber valueDelta, bool cloneBoundedNumbers = true)
        {
            OriginalValue = value;
            _value = value;
            OriginalValueEvolutionDeltaMax = originalValueEvolutionDeltaMax;

            ValueDeltaMaximum = cloneBoundedNumbers ? valueDelta.Clone() : valueDelta;
            ValueMinimum = cloneBoundedNumbers ? minimumValue.Clone() : minimumValue;
            ValueMaximum = cloneBoundedNumbers ? maximumValue.Clone() : maximumValue;
            ValueMaximumAndMinimumEvolutionDeltaMax = valueMaximumAndMinimumEvolutionDeltaMax;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="absoluteMinimumValue">The absolute minimum value.</param>
        /// <param name="absoluteMaximumValue">The absolute maximum value.</param>
        /// <param name="valueMaximumAndMinimumEvolutionDeltaMax">The value maximum and minimum evolution delta maximum.</param>
        /// <param name="valueDeltaMax">The value delta maximum.</param>
        /// <param name="valueDeltaMaxEvolutionMax">The value delta maximum evolution maximum.</param>
        /// <param name="valueDeltaMaxEvolutionAbsoluteMax">The value delta maximum evolution absolute maximum.</param>
        public EvoNumber(double value, double originalValueEvolutionDeltaMax, double minimumValue, double maximumValue, double absoluteMinimumValue, double absoluteMaximumValue, double valueMaximumAndMinimumEvolutionDeltaMax, double valueDeltaMax, double valueDeltaMaxEvolutionMax, double valueDeltaMaxEvolutionAbsoluteMax) : this(value, originalValueEvolutionDeltaMax, new BoundedNumber(minimumValue, minValue: absoluteMinimumValue), new BoundedNumber(maximumValue, maxValue: absoluteMaximumValue), valueMaximumAndMinimumEvolutionDeltaMax, new DeltaBoundedNumber(valueDeltaMax, valueDeltaMaxEvolutionMax, 0, valueDeltaMaxEvolutionAbsoluteMax), cloneBoundedNumbers: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public EvoNumber(EvoNumber parent) : this(parent.Value, parent.OriginalValueEvolutionDeltaMax, parent.ValueMinimum, parent.ValueMaximum, parent.ValueMaximumAndMinimumEvolutionDeltaMax, parent.ValueDeltaMaximum)
        {
        }

        /// <summary>
        /// Gets the original (start) value.
        /// </summary>
        /// <value>The original value.</value>
        public double OriginalValue { get; }

        /// <summary>
        /// Gets the original value evolution delta maximum.
        /// </summary>
        /// <value>The original value evolution delta maximum.</value>
        public double OriginalValueEvolutionDeltaMax { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonIgnore]
        public double Value
        {
            get => _value;
            set => _value = ExtraMath.DeltaClamp(value, _value, -ValueDeltaMaximum, ValueDeltaMaximum, ValueMinimum.Value, ValueMaximum.Value);
        }

        /// <summary>
        /// Gets the value delta. This is the maximum amount that the Value can change when it is updated.
        /// </summary>
        /// <value>The value delta.</value>
        public DeltaBoundedNumber ValueDeltaMaximum { get; }

        /// <summary>
        /// The maximum that the value can be.
        /// </summary>
        public BoundedNumber ValueMaximum { get; }

        /// <summary>
        /// The minimum that the value can be.
        /// </summary>
        public BoundedNumber ValueMinimum { get; }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EvoNumber left, EvoNumber right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(EvoNumber left, EvoNumber right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(EvoNumber left, EvoNumber right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EvoNumber left, EvoNumber right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(EvoNumber left, EvoNumber right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(EvoNumber left, EvoNumber right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public EvoNumber Clone()
        {
            return new EvoNumber(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is EvoNumber number &&
                   OriginalValue == number.OriginalValue &&
                   OriginalValueEvolutionDeltaMax == number.OriginalValueEvolutionDeltaMax &&
                   Value == number.Value &&
                   ValueDeltaMaximum.Equals(number.ValueDeltaMaximum) &&
                   ValueMaximum.Equals(number.ValueMaximum) &&
                   ValueMinimum.Equals(number.ValueMinimum) &&
                   ValueMaximumAndMinimumEvolutionDeltaMax == number.ValueMaximumAndMinimumEvolutionDeltaMax;
        }

        /// <summary>
        /// Creates a cloned instance with evolved values.
        /// </summary>
        /// <param name="randomGenerator">The random generator.</param>
        /// <returns>The evolved instance.</returns>
        public EvoNumber Evolve(IRandom randomGenerator)
        {
            double newOriginalValue = EvolveValue(randomGenerator, OriginalValue, OriginalValueEvolutionDeltaMax, ValueMinimum, ValueMaximum);

            double newMinimumValue = EvolveValue(randomGenerator, ValueMinimum.Value, ValueMaximumAndMinimumEvolutionDeltaMax, ValueMinimum.MinValue, ValueMaximum.MaxValue);
            BoundedNumber newMinimum = new BoundedNumber(newMinimumValue, minValue: ValueMinimum.MinValue);

            double newMaximumValue = EvolveValue(randomGenerator, ValueMaximum.Value, ValueMaximumAndMinimumEvolutionDeltaMax, ValueMinimum.MinValue, ValueMaximum.MaxValue);
            BoundedNumber newMaximum = new BoundedNumber(newMaximumValue, maxValue: ValueMaximum.MaxValue);

            double newValueDeltaMaximum = EvolveValue(randomGenerator, ValueDeltaMaximum, ValueDeltaMaximum.DeltaMaximum, 0, ValueDeltaMaximum.DeltaMaximum.MaxValue);
            DeltaBoundedNumber newValueDelta = new DeltaBoundedNumber(newValueDeltaMaximum, ValueDeltaMaximum.DeltaMaximum, 0, ValueDeltaMaximum.DeltaMaximum.MaxValue);

            return new EvoNumber(newOriginalValue, OriginalValueEvolutionDeltaMax, newMinimum, newMaximum, ValueMaximumAndMinimumEvolutionDeltaMax, newValueDelta);
        }

        /// <summary>
        /// Creates a cloned instance with evolved values.
        /// </summary>
        /// <param name="sim">The simulation to source the random number generator from.</param>
        /// <returns>The evolved instance.</returns>
        public EvoNumber Evolve(Simulation sim)
        {
            return Evolve(sim.Random);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            double hash = 17;
            hash = hash * 23 + OriginalValue.GetHashCode();
            hash = hash * 23 + OriginalValueEvolutionDeltaMax.GetHashCode();
            hash = hash * 23 + Value.GetHashCode();
            hash = hash * 23 + ValueDeltaMaximum.GetHashCode();
            hash = hash * 23 + ValueMaximum.GetHashCode();
            hash = hash * 23 + ValueMinimum.GetHashCode();
            hash = hash * 23 + ValueMaximumAndMinimumEvolutionDeltaMax.GetHashCode();
            return (int)hash;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"EvoNumber: {Value} (Original: {OriginalValue})";
        }

        /// <summary>
        /// Evolves the value.
        /// </summary>
        /// <param name="sim">The random number generator.</param>
        /// <param name="current">The current.</param>
        /// <param name="deltaMax">The delta maximum.</param>
        /// <param name="hardMin">The hard minimum.</param>
        /// <param name="hardMax">The hard maximum.</param>
        /// <returns>The evolved number.</returns>
        private double EvolveValue(IRandom rand, double current, double deltaMax, double hardMin, double hardMax)
        {
            if(deltaMax == 0)
            {
                return current;
            }

            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1))
                                   * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = EVOLUTION_MEAN + EVOLUTION_STANDARD_DEVIATION * randStdNormal;     //random normal(mean,stdDev^2)

            double delta = randNormal * deltaMax;
            //double delta = (Simulation.Random.NextDouble() * deltaMax)
            //               + (Simulation.Random.NextDouble() * deltaMax)
            //               - deltaMax;

            double moddedValue = current + delta;
            double clampedValue = ExtraMath.Clamp(moddedValue, hardMin, hardMax);
            return clampedValue;
        }
    }
}
