using System.Diagnostics;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Defines a circle.
    /// </summary>
    /// <seealso cref="IShape"/>
    [DebuggerDisplay("{ToString()}")]
    public class Circle : IShape
    {
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Circle: CentrePoint={CentrePoint}, Radius={Radius}";
        }
    }
}
