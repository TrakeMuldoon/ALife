using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using ALife.Core.Utility.Maths;

namespace ALife.Core.Utility.Ranges
{
    /// <summary>
    /// Represents a range of values.
    /// TODO: once we're on .NET 8, let's use generic math here and bind this only be for numeric types.
    /// TODO: remove Microsoft.CSharp nuget package after the above is done and we won't need the dynamic keywords.
    /// </summary>
    /// <typeparam name="T">The type of the range.</typeparam>
    [DebuggerDisplay("({Minimum} -> {Maximum})")]
    public struct Range<T> where T : struct
    {
        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        [JsonIgnore]
        private T _maximum;

        /// <summary>
        /// The minimum value of the range.
        /// </summary>
        [JsonIgnore]
        private T _minimum;

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
        [JsonConstructor]
        public Range(T minimum, T maximum)
        {
            if(!NumericTypes.SupportedTypes.Contains(typeof(T)))
            {
                throw new ArgumentException($"Type {typeof(T)} is not supported by the Range class.");
            }

            _minimum = minimum;
            _maximum = maximum;
            if((dynamic)_maximum < (dynamic)_minimum)
            {
                (_minimum, _maximum) = (_maximum, _minimum);
            }
            Difference = (T)(object)((dynamic)_maximum - (dynamic)_minimum);
        }

        /// <summary>
        /// No easy support right now because we're stuck on .NET Standard 2.
        /// </summary>
        [JsonIgnore]
        public T Difference { get; private set; }

        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        [JsonPropertyName("maximum")]
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
        [JsonPropertyName("minimum")]
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
        /// Clamps the value to the range in a circular fashion.
        /// TODO: Generic math would make more performant...
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The clampped value.</returns>
        public T CircularClampValue(T value)
        {
            dynamic output = ExtraMaths.CircularClamp((dynamic)value, (dynamic)Minimum, (dynamic)Maximum);
            return output;
        }

        /// <summary>
        /// Clamps the value to the range.
        /// TODO: Generic math would make more performant...
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The clampped value.</returns>
        public T ClampValue(T value)
        {
            dynamic output = ExtraMaths.Clamp((dynamic)value, (dynamic)Minimum, (dynamic)Maximum);
            return output;
        }
    }
}
