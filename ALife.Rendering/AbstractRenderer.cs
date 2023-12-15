using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility;
using ALife.Core.WorldObjects.Agents;
using System.Drawing;
using Point = ALife.Core.Geometry.Shapes.Point;

namespace ALife.Rendering
{
    /// <summary>
    /// Represents an abstract renderer that can be used with different rendering engines to display the ALife simulation.
    /// </summary>
    public abstract class AbstractRenderer
    {
        public abstract void DrawText(string text, Point point, Color color);

        public abstract void DrawAgentAncestry(Agent agent);

        public void DrawRectangle(Core.Geometry.Shapes.Rectangle rectangle, Color color, double strokeWidth)
        {
            DrawRectangle(rectangle.TopLeft.X, rectangle.TopLeft.Y, rectangle.BottomRight.X, rectangle.BottomRight.Y, color, strokeWidth, true);
        }

        public abstract void DrawRectangle(double x, double y, double width, double height, Color color, double strokeWidth, bool widthAndHeightAreCoords = false);

        public void FillRectangle(Point xy, double width, double height, Color color)
        {
            FillRectangle(xy.X, xy.Y, width, height, color);
        }

        public abstract void FillRectangle(double x, double y, double width, double height, Color color);

        public abstract void DrawSector(IShape currShape, bool fillIn);

        public abstract void DrawRectangleWithFillIn(Core.Geometry.Shapes.Rectangle rectangle, bool fillIn);

        public abstract void FillCircle(Point centerPoint, float radius, Color color);

        public abstract void DrawCircle(Point centerPoint, float radius, Color colo);

        public abstract void DrawLine(Point point1, Point point2, Color color, double strokeWidth);

        public abstract void DrawSector(IShape currShape, AbstractRenderer renderer, bool fillIn);

        public void DrawOrientation(IShape shape)
        {
            Core.Geometry.Shapes.Point ori = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 4);
            Core.Geometry.Shapes.Point ori2 = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 1);
            DrawLine(ori, ori2, Color.DarkRed, 1);
        }
    }
}
