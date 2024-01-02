using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ALife.Core.Utility;
using ALife.Core.Utility.Numerics;

namespace ALife.Core.Geometry
{
    /// <summary>
    /// Defines an angle in degrees or radians.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Angle
    {
        /// <summary>
        /// An angle representing zero degrees.
        /// </summary>
        public static readonly Angle Zero = new Angle(0d);

        /// <summary>
        /// The degrees
        /// </summary>
        private CircularBoundedNumber _degrees;

        /// <summary>
        /// The radians
        /// </summary>
        [JsonIgnore]
        private CircularBoundedNumber _radians;

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> class.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        [JsonConstructor]
        public Angle(double degrees)
        {
            _degrees = new CircularBoundedNumber(degrees, GeometryConstants.MinDegrees, GeometryConstants.MaxDegrees);
            _radians = new CircularBoundedNumber(degrees * GeometryConstants.Pi / GeometryConstants.HalfDegrees, GeometryConstants.ZeroPi, GeometryConstants.TwoPi);
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
        /// Gets the inverse degrees.
        /// </summary>
        /// <value>The inverse degrees.</value>
        [JsonIgnore]
        public double InverseDegrees => -(GeometryConstants.MaxDegrees - Degrees);

        /// <summary>
        /// Gets the inverse radians.
        /// </summary>
        /// <value>The inverse radians.</value>
        [JsonIgnore]
        public double InverseRadians => -(GeometryConstants.TwoPi - Radians);

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
            while(radians < GeometryConstants.ZeroPi)
            {
                radians += GeometryConstants.TwoPi;
            }
            while(radians > GeometryConstants.TwoPi)
            {
                radians -= GeometryConstants.TwoPi;
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
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
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
        /// Gets the transformation matrix representing this angle.
        /// </summary>
        /// <returns>The transformation matrix.</returns>
        public Matrix GetTransformationMatrix()
        {
            Matrix output = Matrix.CreateFromAngle(this);
            return output;
        }

        /// <summary>
        /// Gets the transformation matrix representing this angle and a specified translation.
        /// </summary>
        /// <param name="translation">The translation.</param>
        /// <returns>The transformation matrix.</returns>
        public Matrix GetTransformationMatrix(Point translation)
        {
            Matrix output = Matrix.CreateFromTranslationAndAngle(this, translation);
            return output;
        }

        /// <summary>
        /// Gets the transformation matrix representing this angle and a specified translation.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The transformation matrix.</returns>
        public Matrix GetTransformationMatrix(double x, double y)
        {
            Matrix output = Matrix.CreateFromTranslationAndAngle(this, x, y);
            return output;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
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
            return degrees * GeometryConstants.Pi / GeometryConstants.HalfDegrees;
        }

        /// <summary>
        /// Converts the radians to degrees.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>The degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double RadiansToDegrees(double radians)
        {
            return radians * GeometryConstants.HalfDegrees / GeometryConstants.Pi;
        }
    }
}
