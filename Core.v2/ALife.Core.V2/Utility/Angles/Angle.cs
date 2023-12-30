using ALife.Core.Utility.Numerics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Angles
{
    /// <summary>
    /// Defines an angle in degrees or radians.
    /// </summary>
    [DebuggerDisplay("Deg:{Degrees}, Rads:{Radians}")]
    public class Angle
    {
        /// <summary>
        /// The degrees
        /// </summary>
        private CircularBoundedNumber _degrees;

        /// <summary>
        /// The radians
        /// </summary>
        private CircularBoundedNumber _radians;

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> class.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        public Angle(double degrees)
        {
            _degrees = new CircularBoundedNumber(degrees, 0, 360);
            _radians = new CircularBoundedNumber(degrees * Math.PI / 180.00, 0, 2 * Math.PI);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> class.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public Angle(Angle angle)
        {
            _degrees = new CircularBoundedNumber(angle._degrees);
            _radians = new CircularBoundedNumber(angle._radians);
        }

        /// <summary>
        /// Gets or sets the degrees.
        /// </summary>
        /// <value>The degrees.</value>
        [JsonIgnore]
        public double Degrees
        {
            get => _degrees;
            set
            {
                _degrees.Value = value;
                _radians.Value = DegreesToRadians(_degrees);
            }
        }

        /// <summary>
        /// Gets or sets the radians.
        /// </summary>
        /// <value>The radians.</value>
        [JsonIgnore]
        public double Radians
        {
            get => _radians;
            set
            {
                _radians.Value = value;
                _degrees.Value = RadiansToDegrees(_radians);
            }
        }

        /// <summary>
        /// Froms the degrees.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        /// <returns>A new angle.</returns>
        public static Angle FromDegrees(double degrees)
        {
            return new Angle(degrees);
        }

        /// <summary>
        /// Froms the radians.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>A new angle.</returns>
        public static Angle FromRadians(double radians)
        {
            double TwoPi = Math.PI * 2;
            while(radians < 0)
            {
                radians += TwoPi;
            }
            while(radians > 360)
            {
                radians -= TwoPi;
            }
            double degrees = RadiansToDegrees(radians);
            return new Angle(degrees);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator -(Angle a, Angle b)
        {
            return new Angle(a.Degrees - b.Degrees);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator -(Angle a, double b)
        {
            return new Angle(a.Degrees - b);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator -(double a, Angle b)
        {
            return new Angle(a - b.Degrees);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator *(Angle a, Angle b)
        {
            return new Angle(a.Degrees * b.Degrees);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator *(Angle a, double b)
        {
            return new Angle(a.Degrees * b);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator *(double a, Angle b)
        {
            return new Angle(a * b.Degrees);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator /(Angle a, Angle b)
        {
            return new Angle(a.Degrees / b.Degrees);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator /(Angle a, double b)
        {
            return new Angle(a.Degrees / b);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator /(double a, Angle b)
        {
            return new Angle(a / b.Degrees);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator +(Angle a, double b)
        {
            return new Angle(a.Degrees + b);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator +(double a, Angle b)
        {
            return new Angle(a + b.Degrees);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Angle operator +(Angle a, Angle b)
        {
            return new Angle(a.Degrees + b.Degrees);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Angle left, Angle right)
        {
            return left.Degrees < right.Degrees;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(Angle left, Angle right)
        {
            return left.Degrees <= right.Degrees;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Angle left, Angle right)
        {
            return left.Degrees > right.Degrees;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(Angle left, Angle right)
        {
            return left.Degrees >= right.Degrees;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public Angle Clone()
        {
            return new Angle(this);
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
            return obj is Angle angle &&
                angle.Degrees == Degrees;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _degrees.GetHashCode() ^ _radians.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Deg:{Degrees}, Rads:{Radians}";
        }

        /// <summary>
        /// Converts the degrees to radians.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        /// <returns>The radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.00;
        }

        /// <summary>
        /// Converts the radians to degrees.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>The degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.00 / Math.PI;
        }
    }
}
