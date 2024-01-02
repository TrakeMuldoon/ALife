using System.Diagnostics;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Defines a circle.
    /// </summary>
    /// <seealso cref="AbstractShape"/>
    [DebuggerDisplay("{ToString()}")]
    public class Circle : AbstractShape
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
