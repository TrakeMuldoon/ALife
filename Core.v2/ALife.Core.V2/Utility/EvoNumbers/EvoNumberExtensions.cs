using ALife.Core.Utility.Numerics;
using ALife.Core.Utility.Random;
using System;

namespace ALife.Core.Utility.EvoNumbers
{
    public static class EvoNumberExtensions
    {
        /// <summary>
        /// Initializes a new IEvoNumber from the specified IEvoNumber.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The cloned IEvoNumber.</returns>
        public static IEvoNumber Clone(this IEvoNumber number)
        {
            IEvoNumber output = GetEvoNumber(number.GetType(), number.Value, number.OriginalValueEvolutionDeltaMax, number.ValueMinimum, number.ValueMaximum, number.ValueMaximumAndMinimumEvolutionDeltaMax, number.ValueDeltaMaximum);
            return output;
        }

        /// <summary>
        /// Creates a cloned instance with evolved values.
        /// </summary>
        /// <param name="sim">The simulation to source the random number generator from.</param>
        /// <returns>The evolved instance.</returns>
        public static IEvoNumber Evolve(this IEvoNumber number, Simulation simulation)
        {
            return Evolve(number, simulation.Random);
        }

        /// <summary>
        /// Creates a cloned instance with evolved values.
        /// </summary>
        /// <param name="randomGenerator">The random generator.</param>
        /// <returns>The evolved instance.</returns>
        public static IEvoNumber Evolve(this IEvoNumber number, IRandom randomGenerator)
        {
            double newOriginalValue = EvoNumberHelpers.EvolveValue(randomGenerator, number.OriginalValue, number.OriginalValueEvolutionDeltaMax, number.ValueMinimum, number.ValueMaximum);

            double newMinimumValue = EvoNumberHelpers.EvolveValue(randomGenerator, number.ValueMinimum.Value, number.ValueMaximumAndMinimumEvolutionDeltaMax, number.ValueMinimum.MinValue, number.ValueMaximum.MaxValue);
            BoundedNumber newMinimum = new BoundedNumber(newMinimumValue, minValue: number.ValueMinimum.MinValue);

            double newMaximumValue = EvoNumberHelpers.EvolveValue(randomGenerator, number.ValueMaximum.Value, number.ValueMaximumAndMinimumEvolutionDeltaMax, number.ValueMinimum.MinValue, number.ValueMaximum.MaxValue);
            BoundedNumber newMaximum = new BoundedNumber(newMaximumValue, maxValue: number.ValueMaximum.MaxValue);

            double newValueDeltaMaximum = EvoNumberHelpers.EvolveValue(randomGenerator, number.ValueDeltaMaximum, number.ValueDeltaMaximum.DeltaMaximum, 0, number.ValueDeltaMaximum.DeltaMaximum.MaxValue);
            DeltaBoundedNumber newValueDelta = new DeltaBoundedNumber(newValueDeltaMaximum, number.ValueDeltaMaximum.DeltaMaximum, 0, number.ValueDeltaMaximum.DeltaMaximum.MaxValue);

            IEvoNumber output = GetEvoNumber(number.GetType(), newOriginalValue, number.OriginalValueEvolutionDeltaMax, newMinimum, newMaximum, number.ValueMaximumAndMinimumEvolutionDeltaMax, newValueDelta);
            return output;
        }

        /// <summary>
        /// Converts the ROEvoNumber to a EvoNumber.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The EvoNumber.</returns>
        public static EvoNumber ToEvoNumber(this ReadOnlyEvoNumber number)
        {
            IEvoNumber output = GetEvoNumber(typeof(EvoNumber), number.Value, number.OriginalValueEvolutionDeltaMax, number.ValueMinimum, number.ValueMaximum, number.ValueMaximumAndMinimumEvolutionDeltaMax, number.ValueDeltaMaximum);
            return (EvoNumber)output;
        }

        /// <summary>
        /// Converts the EvoNumber to a ROEvoNumber.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The RPEvoNumber.</returns>
        public static ReadOnlyEvoNumber ToReadOnlyEvoNumber(this EvoNumber number)
        {
            IEvoNumber output = GetEvoNumber(typeof(ReadOnlyEvoNumber), number.Value, number.OriginalValueEvolutionDeltaMax, number.ValueMinimum, number.ValueMaximum, number.ValueMaximumAndMinimumEvolutionDeltaMax, number.ValueDeltaMaximum);
            return (ReadOnlyEvoNumber)output;
        }

        /// <summary>
        /// Generates an EvoNumber of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="newOriginalValue">The new original value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="newMinimum">The new minimum.</param>
        /// <param name="newMaximum">The new maximum.</param>
        /// <param name="valueMaximumAndMinimumEvolutionDeltaMax">The value maximum and minimum evolution delta maximum.</param>
        /// <param name="newValueDelta">The new value delta.</param>
        /// <returns>The EvoNumber.</returns>
        /// <exception cref="System.NotImplementedException">The type {type} is not supported.</exception>
        private static IEvoNumber GetEvoNumber(Type type, double newOriginalValue, double originalValueEvolutionDeltaMax, BoundedNumber newMinimum, BoundedNumber newMaximum, double valueMaximumAndMinimumEvolutionDeltaMax, DeltaBoundedNumber newValueDelta)
        {
            switch(type)
            {
                case Type t when t == typeof(EvoNumber):
                    return new EvoNumber(newOriginalValue, originalValueEvolutionDeltaMax, newMinimum, newMaximum, valueMaximumAndMinimumEvolutionDeltaMax, newValueDelta);

                case Type t when t == typeof(ReadOnlyEvoNumber):
                    return new ReadOnlyEvoNumber(newOriginalValue, originalValueEvolutionDeltaMax, newMinimum, newMaximum, valueMaximumAndMinimumEvolutionDeltaMax, newValueDelta);

                default:
                    throw new NotImplementedException($"The type {type} is not supported.");
            }
        }
    }
}
