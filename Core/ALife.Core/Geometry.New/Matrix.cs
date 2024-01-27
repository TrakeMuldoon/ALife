using ALife.Core.Utility.Maths;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;

namespace ALife.Core.Geometry.New
{
    /// <summary>
    /// Represents a right-handed 4x4 matrix. Used by us for translation and rotation info.
    /// Note: overkill atm a little bit since the simulation is in 2D, but maybe we'll add a third dimension later.
    /// TODO: Add a third dimension later.
    /// TODO: If staying 2D and performance/memory is an issue, consider using a 3x3 matrix instead.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Matrix
    {
        /// <summary>
        /// A matrix pre-populated with 0's for everything.
        /// </summary>
        [JsonIgnore]
        public static Matrix Zero = new Matrix();

        /// <summary>
        /// Row 1, Column 1
        /// </summary>
        public double M11;

        /// <summary>
        /// Row 1, Column 2
        /// </summary>
        public double M12;

        /// <summary>
        /// Row 1, Column 3
        /// </summary>
        public double M13;

        /// <summary>
        /// Row 1, Column 4
        /// </summary>
        public double M14;

        /// <summary>
        /// Row 2, Column 1
        /// </summary>
        public double M21;

        /// <summary>
        /// Row 2, Column 2
        /// </summary>
        public double M22;

        /// <summary>
        /// Row 2, Column 3
        /// </summary>
        public double M23;

        /// <summary>
        /// Row 2, Column 4
        /// </summary>
        public double M24;

        /// <summary>
        /// Row 3, Column 1
        /// </summary>
        public double M31;

        /// <summary>
        /// Row 3, Column 2
        /// </summary>
        public double M32;

        /// <summary>
        /// Row 3, Column 3
        /// </summary>
        public double M33;

        /// <summary>
        /// Row 3, Column 4
        /// </summary>
        public double M34;

        /// <summary>
        /// Row 3, Column 1
        /// </summary>
        public double M41;

        /// <summary>
        /// Row 3, Column 2
        /// </summary>
        public double M42;

        /// <summary>
        /// Row 3, Column 3
        /// </summary>
        public double M43;

        /// <summary>
        /// Row 3, Column 4
        /// </summary>
        public double M44;

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> struct.
        /// </summary>
        /// <param name="m11">Row 1, Column 1.</param>
        /// <param name="m12">Row 1, Column 2.</param>
        /// <param name="m13">Row 1, Column 3.</param>
        /// <param name="m14">Row 1, Column 4.</param>
        /// <param name="m21">Row 2, Column 1.</param>
        /// <param name="m22">Row 2, Column 2.</param>
        /// <param name="m23">Row 2, Column 3.</param>
        /// <param name="m24">Row 2, Column 4.</param>
        /// <param name="m31">Row 3, Column 1.</param>
        /// <param name="m32">Row 3, Column 2.</param>
        /// <param name="m33">Row 3, Column 3.</param>
        /// <param name="m34">Row 3, Column 4.</param>
        /// <param name="m41">Row 4, Column 1.</param>
        /// <param name="m42">Row 4, Column 2.</param>
        /// <param name="m43">Row 4, Column 3.</param>
        /// <param name="m44">Row 4, Column 4.</param>
        [JsonConstructor]
        public Matrix(
            double m11 = 0, double m12 = 0, double m13 = 0, double m14 = 0,
            double m21 = 0, double m22 = 0, double m23 = 0, double m24 = 0,
            double m31 = 0, double m32 = 0, double m33 = 0, double m34 = 0,
            double m41 = 0, double m42 = 0, double m43 = 0, double m44 = 0
                     )
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        /// <summary>
        /// Creates a matrix from an angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromAngle(Angle angle, Nullable<Point> axis = null)
        {
            return CreateFromAngle(angle.Radians, axis);
        }

        /// <summary>
        /// Creates a matrix from an angle.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromAngle(double radians, Nullable<Point> axis = null)
        {
            Point actualAxis = axis ?? Point.Zero;
            Matrix output = new Matrix();

            double x = actualAxis.X;
            double y = actualAxis.Y;

            /*

            // Leaving this here for completeness, but Z is always 0 since we're only dealing with 2D

            double z = 0;

            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);

            double xx = x * x;
            double yy = y * y;
            double zz = z * z; // Z is always 0 for 2d, but leaving it in for completeness

            double xy = x * y;
            double xz = x * z; // Z is always 0 for 2d, but leaving it in for completeness
            double yz = y * z; // Z is always 0 for 2d, but leaving it in for completeness

            double m11 = xx + cos * (1 - xx);
            double m12 = (xy - cos * xy) + sin * z;
            double m13 = (xz - cos * xz) - sin * y;
            double m14 = 0;

            double m21 = (xy - cos * xy) - sin * z;
            double m22 = yy + cos * (1 - yy);
            double m23 = (yz - cos * yz) + sin * x;
            double m24 = 0;

            double m31 = (xz - cos * xz) + sin * y;
            double m32 = (yz - cos * yz) - sin * x;
            double m33 = zz + cos * (1 - zz);
            double m34 = 0;
            */

            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);

            double xx = x * x;
            double yy = y * y;

            double xy = x * y;

            output.M11 = xx + cos * (1 - xx);
            output.M12 = xy - cos * xy;
            output.M13 = -(sin * y);
            output.M14 = 0;

            output.M21 = xy - cos * xy;
            output.M22 = yy + cos * (1 - yy);
            output.M23 = sin * x;
            output.M24 = 0;

            // This is Z - Not needed atm!
            //output.M31 = sin * y;
            //output.M32 = -(sin * x);
            //output.M33 = cos;
            //output.M34 = 0;

            output.M41 = 0;
            output.M42 = 0;
            output.M43 = 0;
            output.M44 = 1;

            return output;
        }

        /// <summary>
        /// Creates a matrix from a translation.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslation(double x, double y, double z = 0)
        {
            Matrix output = new Matrix();

            output.M11 = 1;
            output.M22 = 1;
            output.M33 = 1;

            output.M41 = x;
            output.M42 = y;
            output.M43 = z;
            output.M44 = 1;

            return output;
        }

        /// <summary>
        /// Creates a matrix from a translation.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslation(Point point)
        {
            return CreateFromTranslation(point.X, point.Y);
        }

        /// <summary>
        /// Creates a matrix from a translation.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslation(Vector2 point)
        {
            return CreateFromTranslation(point.X, point.Y);
        }

        /// <summary>
        /// Creates a matrix from a translation and angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslationAndAngle(Angle angle, Point translation, Nullable<Point> axis = null)
        {
            return CreateFromTranslationAndAngle(angle.Radians, translation.X, translation.Y, 0, axis);
        }

        /// <summary>
        /// Creates a matrix from a translation and angle.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslationAndAngle(double radians, Point translation, Nullable<Point> axis = null)
        {
            return CreateFromTranslationAndAngle(radians, translation.X, translation.Y, 0, axis);
        }

        /// <summary>
        /// Creates a matrix from a translation and angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslationAndAngle(Angle angle, Vector2 translation, Nullable<Point> axis = null)
        {
            return CreateFromTranslationAndAngle(angle.Radians, translation.X, translation.Y, 0, axis);
        }

        /// <summary>
        /// Creates a matrix from a translation and angle.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="">The .</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslationAndAngle(double radians, Vector2 translation, Nullable<Point> axis = null)
        {
            return CreateFromTranslationAndAngle(radians, translation.X, translation.Y, 0, axis);
        }

        /// <summary>
        /// Creates a matrix from a translation and angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslationAndAngle(Angle angle, double x, double y, double z = 0, Nullable<Point> axis = null)
        {
            return CreateFromTranslationAndAngle(angle.Radians, x, y, z, axis);
        }

        /// <summary>
        /// Creates a matrix from a translation and angle.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix CreateFromTranslationAndAngle(double radians, double x, double y, double z = 0, Nullable<Point> axis = null)
        {
            Matrix output = CreateFromAngle(radians, axis);

            output.M41 = x;
            output.M42 = y;
            //output.M43 = z;

            return output;
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix mat &&
              M11 == mat.M11 &&
                     M12 == mat.M12 &&
                     M13 == mat.M13 &&
                     M14 == mat.M14 &&
                     M21 == mat.M21 &&
                     M22 == mat.M22 &&
                     M23 == mat.M23 &&
                     M24 == mat.M24 &&
                     M31 == mat.M31 &&
                     M32 == mat.M32 &&
                     M33 == mat.M33 &&
                     M34 == mat.M34 &&
                     M41 == mat.M41 &&
                     M42 == mat.M42 &&
                     M43 == mat.M43 &&
                     M44 == mat.M44;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
        }

        public override string ToString()
        {
            string output = $"[[[{M11}], [{M12}], [{M13}], [{M14}]], [[{M21}], [{M22}], [{M23}], [{M24}]], [[{M31}], [{M32}], [{M33}], [{M34}]], [[{M41}], [{M42}], [{M43}], [{M44}]]]";
            return output;
        }
    }
}