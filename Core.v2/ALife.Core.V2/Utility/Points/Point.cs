namespace ALife.Core.Utility.Points
{
    /// <summary>
    /// Defines a Point in space
    /// </summary>
    public class Point
    {
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
        /// Converts to string.
        /// </summary>
        /// <returns>The string representation of the Geometry.Shapes.Point.</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
