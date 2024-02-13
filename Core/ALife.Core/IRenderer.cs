using ALife.Core.NewGeometry;
using ALife.Core.Utility.Colours;

namespace ALife.Core
{
    public interface IRenderer
    {
        void DrawCircle(Point centrePoint, double radius, bool drawFill, IColour fillColour, bool drawOutline, IColour outlineColour, double strokeWidth);
    }
}
