using System;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;

namespace ALife.Core.Geometry.New
{
    /// <summary>
    /// Defines a Point in space
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Point
    {
        /// <summary>
        /// A point representing 0, 0
        /// </summary>
        public static Point Zero = new Point(0, 0);

        /// <summary>
        /// The x coordinate
        /// </summary>
        public double X;

        /// <summary>
        /// The y coordinate
        /// </summary>
        public double Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        [JsonConstructor]
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="Geometry.Shapes.Point">The Geometry.Shapes.Point.</param>
        public Point(Point Point)
        {
            X = Point.X;
            Y = Point.Y;
        }

        /// <summary>
        /// Gets the x int.
        /// </summary>
        /// <value>The x int.</value>
        [JsonIgnore]
        public int XInt => (int)Math.Round(X);

        /// <summary>
        /// Gets the y int.
        /// </summary>
        /// <value>The y int.</value>
        [JsonIgnore]
        public int YInt => (int)Math.Round(Y);

        /// <summary>
        /// Transforms the specified point by the matrix.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The transformed point.</returns>
        public static Point FromTransformation(Point point, Matrix matrix)
        {
            double newX = point.X * matrix.M11 + point.Y * matrix.M21 + matrix.M41;
            double newY = point.X * matrix.M12 + point.Y * matrix.M22 + matrix.M42;

            return new Point(newX, newY);
        }

        /// <summary>
        /// Implements the operator op_Inequality.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public Point Clone()
        {
            return new Point(X, Y);
        }

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True if equals, False otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Point && Equals((Point)obj);
        }

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True if equals, False otherwise.</returns>
        public bool Equals(Point value)
        {
            return X.Equals(value.X) && Y.Equals(value.Y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (X + Y).GetHashCode();
        }

        /// <summary>
        /// Gets the transformation matrix representing this point.
        /// </summary>
        /// <returns>The transformation matrix.</returns>
        public Matrix GetTransformationMatrix()
        {
            Matrix output = Matrix.CreateFromTranslation(this);
            return output;
        }

        /// <summary>
        /// Gets the transformation matrix representing this point and a specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The transformation matrix.</returns>
        public Matrix GetTransformationMatrix(Angle angle)
        {
            Matrix output = Matrix.CreateFromTranslationAndAngle(angle, this);
            return output;
        }

        /// <summary>
        /// Gets a transformed point using the current point against the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public Point GetTransformedPoint(Matrix matrix)
        {
            return FromTransformation(this, matrix);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>The string representation of the Geometry.Shapes.Point.</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        /// <summary>
        /// Converts a point to a Vector2.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>The Vector2</returns>
        public Vector2 ToVector2()
        {
            return new Vector2((float)X, (float)Y);
        }

        /// <summary>
        /// Transforms the current point using the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void Transform(Matrix matrix)
        {
            Point transformed = FromTransformation(this, matrix);
            X = transformed.X;
            Y = transformed.Y;
        }

        /// <summary>
        /// Transforms the current point using the specified translation.
        /// </summary>
        /// <param name="translation">The translation.</param>
        public void Transform(Point translation)
        {
            Matrix matrix = Matrix.CreateFromTranslation(translation);
            Transform(matrix);
        }

        /// <summary>
        /// Transforms the current point using the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void Transform(Angle angle)
        {
            Matrix matrix = Matrix.CreateFromAngle(angle);
            Transform(matrix);
        }

        /// <summary>
        /// Transforms the current point using the specified translation.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void Transform(double x, double y)
        {
            Matrix matrix = Matrix.CreateFromTranslation(x, y);
            Transform(matrix);
        }

        /// <summary>
        /// Transforms the current point using the specified angle in radians.
        /// </summary>
        /// <param name="radians">The radians.</param>
        public void Transform(double radians)
        {
            Matrix matrix = Matrix.CreateFromAngle(radians);
            Transform(matrix);
        }

        /// <summary>
        /// Transforms the current point using the specified translation and angle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="angle">The angle.</param>
        public void Transform(double x, double y, Angle angle)
        {
            Matrix matrix = Matrix.CreateFromTranslationAndAngle(angle, x, y);
            Transform(matrix);
        }

        /// <summary>
        /// Transforms the current point using the specified translation and angle in radians.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radians">The radians.</param>
        public void Transform(double x, double y, double radians)
        {
            Matrix matrix = Matrix.CreateFromTranslationAndAngle(radians, x, y);
            Transform(matrix);
        }

        /// <summary>
        /// Transforms the current point using the specified translation and angle.
        /// </summary>
        /// <param name="translation">The translation.</param>
        /// <param name="angle">The angle.</param>
        public void Transform(Point translation, Angle angle)
        {
            Matrix matrix = Matrix.CreateFromTranslationAndAngle(angle, translation);
            Transform(matrix);
        }

        /// <summary>
        /// Updates the x coord.
        /// </summary>
        /// <param name="newX">The new x.</param>
        public void UpdateX(double newX)
        {
            X = newX;
        }

        /// <summary>
        /// Updates the x and y coords.
        /// </summary>
        /// <param name="newX">The new x.</param>
        /// <param name="newY">The new y.</param>
        public void UpdateXY(double newX, double newY)
        {
            X = newX;
            Y = newY;
        }

        /// <summary>
        /// Updates the y coord.
        /// </summary>
        /// <param name="newY">The new y.</param>
        public void UpdateY(double newY)
        {
            Y = newY;
        }
    }
}
