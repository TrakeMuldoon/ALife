using System;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;
using ALife.Core.Utility;

namespace ALife.Core.Geometry
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
        [JsonIgnore]
        public static Point Zero = new Point(0, 0);

        /// <summary>
        /// The x coordinate
        /// </summary>
        private double _x;

        /// <summary>
        /// The y coordinate
        /// </summary>
        private double _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        [JsonConstructor]
        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="Geometry.Shapes.Point">The Geometry.Shapes.Point.</param>
        public Point(Point Point)
        {
            _x = Point.X;
            _y = Point.Y;
        }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>The x.</value>
        [JsonIgnore]
        public double X => _x;

        /// <summary>
        /// Gets the x int.
        /// </summary>
        /// <value>The x int.</value>
        [JsonIgnore]
        public int XInt => (int)Math.Round(_x);

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>The y.</value>
        [JsonIgnore]
        public double Y => _y;

        /// <summary>
        /// Gets the y int.
        /// </summary>
        /// <value>The y int.</value>
        [JsonIgnore]
        public int YInt => (int)Math.Round(_y);

        /// <summary>
        /// Transforms the specified point by the matrix.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The transformed point.</returns>
        public static Point FromTransformation(Point point, Matrix matrix)
        {
            double newX = point._x * matrix.M11 + point._y * matrix.M21 + matrix.M41;
            double newY = point._x * matrix.M12 + point._y * matrix.M22 + matrix.M42;

            return new Point(newX, newY);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator -(Point left, Point right)
        {
            return new Point(left._x - right._x, left._y - right._y);
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
        /// Implements the operator +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator +(Point left, Point right)
        {
            return new Point(left._x + right._x, left._y + right._y);
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
            return new Point(_x, _y);
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
            return _x.Equals(value.X) && _y.Equals(value.Y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(_x, _y);
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
        /// Gets a transformed point using the current point and the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The transformed point.</returns>
        public Point GetTransformedPoint(Matrix matrix)
        {
            Point newPoint = Clone();
            newPoint.TransformPoint(matrix);
            return newPoint;
        }

        /// <summary>
        /// Updates the x coord.
        /// </summary>
        /// <param name="newX">The new x.</param>
        public void SetX(double newX)
        {
            _x = newX;
        }

        /// <summary>
        /// Updates the x and y coords.
        /// </summary>
        /// <param name="newX">The new x.</param>
        /// <param name="newY">The new y.</param>
        public void SetXY(double newX, double newY)
        {
            _x = newX;
            _y = newY;
        }

        /// <summary>
        /// Updates the y coord.
        /// </summary>
        /// <param name="newY">The new y.</param>
        public void SetY(double newY)
        {
            _y = newY;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>The string representation of the Geometry.Shapes.Point.</returns>
        public override string ToString()
        {
            return $"({_x:0.00}, {_y:0.00})";
        }

        /// <summary>
        /// Converts a point to a Vector2.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>The Vector2</returns>
        public Vector2 ToVector2()
        {
            return new Vector2((float)_x, (float)_y);
        }

        /// <summary>
        /// Transforms the current point using the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void TransformPoint(Matrix matrix)
        {
            Point newPosition = FromTransformation(this, matrix);
            _x = newPosition.X;
            _y = newPosition.Y;
        }
    }
}
