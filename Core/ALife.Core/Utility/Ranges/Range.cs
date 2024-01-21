using System;
using System.Diagnostics;
using System.Linq;

namespace ALife.Core.Utility.Ranges
{
    /// <summary>
    /// Represents a range of values.
    /// TODO: once we're on .NET 8, let's use 
    /// </summary>
    /// <typeparam name="T">The type of the range.</typeparam>
    [DebuggerDisplay("({Minimum} -> {Maximum})")]
    public struct Range<T> where T : struct
    {
        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        private T _maximum;

        /// <summary>
        /// The minimum value of the range.
        /// </summary>
        private T _minimum;

        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        public T Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if((dynamic)_maximum < (dynamic)_minimum)
                {
                    (_minimum, _maximum) = (_maximum, _minimum);
                }
                Difference = (T)(object)((dynamic)_maximum - (dynamic)_minimum);
            }
        }

        /// <summary>
        /// The minimum value of the range.
        /// </summary>
        public T Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if((dynamic)_maximum < (dynamic)_minimum)
                {
                    (_minimum, _maximum) = (_maximum, _minimum);
                }
                Difference = (T)(object)((dynamic)_maximum - (dynamic)_minimum);
            }
        }

        /// <summary>
        /// No easy support right now because we're stuck on .NET Standard 2.
        /// </summary>
        public T Difference { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ALife.Core.Utility.Ranges.Range`1"/> struct.
        /// </summary>
        /// <param name="value"></param>
        public Range(Range<T> parent)
        {
            _minimum = parent.Minimum;
            _maximum = parent.Maximum;
            Difference = parent.Difference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ALife.Core.Utility.Ranges.Range`1"/> struct.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public Range(T minimum, T maximum)
        {
            if (!NumericTypes.SupportedTypes.Contains(typeof(T)))
            {
                throw new ArgumentException($"Type {typeof(T)} is not supported by the Range class.");
            }

            _minimum = minimum;
            _maximum = maximum;
            if ((dynamic)_maximum < (dynamic)_minimum)
            {
                (_minimum, _maximum) = (_maximum, _minimum);
            }
            Difference = (T)(object)((dynamic)_maximum - (dynamic)_minimum);
        }

        /// <summary>
        /// Clamps the value to the range.
        /// TODO: Generic math would make more performant...
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The clampped value.</returns>
        public T ClampValue(T value)
        {
            dynamic min = Minimum;
            dynamic max = Maximum;
            dynamic v = value;
            if (v < min)
            {
                return Minimum;
            }
            if (v > max)
            {
                return Maximum;
            }
            return value;
        }

        /// <summary>
        /// Clamps the value to the range in a circular fashion.
        /// TODO: Generic math would make more performant...
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The clampped value.</returns>
        public T CircularClampValue(T value)
        {
            Difference = (T)(object)((dynamic)Maximum - (dynamic)Minimum);
            dynamic remainder = (dynamic)value % (dynamic)Difference;
            dynamic output = (dynamic)Minimum + remainder;
            return output;
        }
    }
}