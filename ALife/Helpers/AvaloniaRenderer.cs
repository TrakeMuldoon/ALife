using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using System.Drawing;

namespace ALife.Helpers
{
    internal class AvaloniaRenderer : AbstractRenderer
    {
        public override void DrawAgentAncestry(Agent agent)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawCircle(Core.Geometry.Shapes.Point centerPoint, float radius, Color colo)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawLine(Core.Geometry.Shapes.Point point1, Core.Geometry.Shapes.Point point2, Color color, double strokeWidth)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawRectangle(double x, double y, double width, double height, Color color, double strokeWidth, bool widthAndHeightAreCoords = false)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawRectangleWithFillIn(Core.Geometry.Shapes.Rectangle rectangle, bool fillIn)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawSector(IShape currShape, bool fillIn)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawSector(IShape currShape, AbstractRenderer renderer, bool fillIn)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawText(string text, Core.Geometry.Shapes.Point point, Color color)
        {
            throw new System.NotImplementedException();
        }

        public override void FillCircle(Core.Geometry.Shapes.Point centerPoint, float radius, Color color)
        {
            throw new System.NotImplementedException();
        }

        public override void FillRectangle(double x, double y, double width, double height, Color color)
        {
            throw new System.NotImplementedException();
        }
    }
}
