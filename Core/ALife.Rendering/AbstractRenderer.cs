using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility;
using ALife.Core.WorldObjects.Agents;
using Color = System.Drawing.Color;

namespace ALife.Rendering
{
    /// <summary>
    /// Represents an abstract renderer that can be used with different rendering engines to display the ALife simulation.
    /// </summary>
    public abstract class AbstractRenderer
    {
        /// <summary>
        /// Most Renderers require context or a drawing session of some kind
        /// </summary>
        /// <typeparam name="T">The type of the drawing session</typeparam>
        /// <param name="context"></param>
        public abstract void SetContext(object context);

        public abstract void DrawText(string text, Point point, Color color);

        public abstract void DrawAgentAncestry(Agent agent);

        public abstract void DrawCircle(Point centerPoint, float radius, Color color);

        public abstract void FillCircle(Point centerPoint, float radius, Color color);

        public void DrawRectangle(Rectangle rectangle, Color color, double strokeWidth)
        {
            DrawRectangle(rectangle.TopLeft, rectangle.TopRight, rectangle.BottomLeft, rectangle.BottomRight, color, strokeWidth);
        }
        public void DrawRectangleWithFillIn(Rectangle rectangle, bool fillIn)
        {
            if(fillIn)
            {
                FillRectangle(rectangle.TopLeft, rectangle.TopRight, rectangle.BottomLeft, rectangle.BottomRight, rectangle.Color);
            }
            else
            {
                DrawRectangle(rectangle.TopLeft, rectangle.TopRight, rectangle.BottomLeft, rectangle.BottomRight, rectangle.Color, 0.4f);
            }
        }

        public abstract void DrawRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Color color, double strokeWidth);

        public abstract void FillRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Color color);

        public abstract void DrawAARectangle(Point topLeft, Point bottomRight, Color color, double strokeWidth);

        public abstract void FillAARectangle(Point topLeft, Point bottomRight, Color color);

        public abstract void DrawSector(Sector currShape, bool fillIn);

        public abstract void DrawLine(Point point1, Point point2, Color color, double strokeWidth);

        public void DrawOrientation(IShape shape)
        {
            Point ori = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 4);
            Point ori2 = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 1);
            DrawLine(ori, ori2, Color.DarkRed, 0.4);
        }
    }
}
