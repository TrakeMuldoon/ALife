using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using Avalonia.Media;
using System;
using AvColor = Avalonia.Media.Color;
using AvPoint = Avalonia.Point;
using Color = System.Drawing.Color;

namespace ALife.Helpers
{
    public class AvaloniaRenderer : AbstractRenderer
    {
        private Pen BLACKPEN = new Pen(Brushes.Black, 1);

        private DrawingContext context = null;
        private DrawingContext Context
        {
            get
            {
                if(context is null)
                {
                    throw new Exception("Please don't use context before setting. It upsets the natural order.");
                }
                return context;
            }
            set { context = value; }
        }

        public override void SetContext(Object drawingContext)
        {
            Context = (DrawingContext)drawingContext;
        }

        public override void DrawAgentAncestry(Agent agent)
        {
            throw new NotImplementedException();
        }

        public override void DrawCircle(Point centerPoint, float radius, Color color)
        {
            Color c = color;
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.DrawEllipse(null, new Pen(b, 1), new Avalonia.Point(centerPoint.X, centerPoint.Y), radius, radius);
        }

        public override void DrawLine(Point point1, Point point2, Color color, double strokeWidth)
        {
            Brush brush = new SolidColorBrush(ConvertColour(color), strokeWidth);
            Context.DrawLine(new Pen(brush), ConvertPoint(point1), ConvertPoint(point2));
        }

        public override void DrawRectangle(double x, double y, double width, double height, Color color, double strokeWidth, bool widthAndHeightAreCoords = false)
        {
            //throw new NotImplementedException();
        }

        public override void DrawRectangleWithFillIn(Rectangle rectangle, bool fillIn)
        {
            throw new NotImplementedException();
        }

        public override void DrawSector(IShape currShape, bool fillIn)
        {
            throw new NotImplementedException();
        }

        public override void DrawSector(IShape currShape, AbstractRenderer renderer, bool fillIn)
        {
            throw new NotImplementedException();
        }

        public override void DrawText(string text, Point point, Color color)
        {
            throw new NotImplementedException();
        }

        public override void FillCircle(Point centerPoint, float radius, Color color)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.DrawEllipse(b, BLACKPEN, ConvertPoint(centerPoint), radius, radius);
        }

        public override void FillRectangle(double x, double y, double width, double height, Color color)
        {
            throw new NotImplementedException();
        }

        private AvPoint ConvertPoint(Point p)
        {
            return new AvPoint(p.X, p.Y);
        }

        private AvColor ConvertColour(Color c)
        {
            return AvColor.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}
