using ALife.Core.Utility.Colours;

namespace ALifeUni
{
    public static class Extensions
    {
        /// <summary>
        /// Convert a Windows.Foundation.Point to an ALife.Core.Geometry.Shapes.Point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static ALife.Core.Geometry.OLD.Shapes.Point ToALifePoint(this Windows.Foundation.Point point)
        {
            return new ALife.Core.Geometry.Shapes.Point(point.X, point.Y);
        }

        /// <summary>
        /// Converts an ALife.Core.Geometry.Shapes.Point to a Windows.Foundation.Point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Windows.Foundation.Point ToUwpPoint(this ALife.Core.Geometry.OLD.Shapes.Point point)
        {
            return new Windows.Foundation.Point(point.X, point.Y);
        }

        /// <summary>
        /// Convert a Colour to a Windows.UI.Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Windows.UI.Color ToWinUiColor(this Colour color)
        {
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a Windows.UI.Color to a Colour
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Colour ToALifeColor(this Windows.UI.Color color)
        {
            return Colour.FromARGB(color.A, color.R, color.G, color.B);
        }
    }
}