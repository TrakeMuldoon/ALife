using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using Avalonia;
using Avalonia.Media;
using System;
using AvColor = Avalonia.Media.Color;
using AvPoint = Avalonia.Point;
using Color = System.Drawing.Color;
using Point = ALife.Core.Geometry.Shapes.Point;

namespace ALife
{
    public class AvaloniaRenderer : AbstractRenderer
    {
        private Pen BLACKPEN = new Pen(Brushes.Black, 0.4);

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
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.DrawEllipse(null, new Pen(b, 1), new Avalonia.Point(centerPoint.X, centerPoint.Y), radius, radius);
        }
        public override void FillCircle(Point centerPoint, float radius, Color color)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.DrawEllipse(b, BLACKPEN, ConvertPoint(centerPoint), radius, radius);
        }

        public override void DrawLine(Point point1, Point point2, Color color, double strokeWidth)
        {
            Brush brush = new SolidColorBrush(ConvertColour(color), strokeWidth);
            Context.DrawLine(new Pen(brush), ConvertPoint(point1), ConvertPoint(point2));
        }

        public override void DrawRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Color color, double strokeWidth)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            Pen pen = new Pen(b, strokeWidth);
            AvPoint p1 = ConvertPoint(topLeft);
            AvPoint p2 = ConvertPoint(topRight);
            AvPoint p3 = ConvertPoint(bottomLeft);
            AvPoint p4 = ConvertPoint(bottomRight);

            Context.DrawLine(pen, p1, p2);
            Context.DrawLine(pen, p2, p3);
            Context.DrawLine(pen, p3, p4);
            Context.DrawLine(pen, p4, p1);
        }

        public override void FillRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Color color)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            PathGeometry pathGeometry = new PathGeometry();
            var geoStream = pathGeometry.Open();
            geoStream.BeginFigure(ConvertPoint(topLeft), true);
            geoStream.LineTo(ConvertPoint(topRight));
            geoStream.LineTo(ConvertPoint(bottomRight));
            geoStream.LineTo(ConvertPoint(bottomLeft));
            geoStream.EndFigure(true);
            Context.DrawGeometry(b, BLACKPEN, pathGeometry);
        }

        public override void DrawAARectangle(Point maxXY, Point minXY, Color color, double strokeWidth)
        {
            Rect rect = new Rect(ConvertPoint(minXY), ConvertPoint(maxXY));
            Brush b = new SolidColorBrush(ConvertColour(color));
            Pen p = new Pen(b, strokeWidth);
            Context.DrawRectangle(p, rect);
        }

        public override void FillAARectangle(Point maxXY, Point minXY, Color color)
        {
            Rect rect = new Rect(ConvertPoint(minXY), ConvertPoint(maxXY));
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.FillRectangle(b, rect);
        }


        public override void DrawSector(Sector currShape, bool fillIn)
        {
            Brush b = new SolidColorBrush(ConvertColour(currShape.Color));
            
            PathGeometry pathGeometry = new PathGeometry();
            var geoStream = pathGeometry.Open();
            geoStream.BeginFigure(ConvertPoint(currShape.CentrePoint), fillIn);
            geoStream.LineTo(ConvertPoint(currShape.LeftPoint));
            geoStream.ArcTo(ConvertPoint(currShape.RightPoint), new Size(currShape.Radius, currShape.Radius), 0, false, SweepDirection.Clockwise);
            geoStream.EndFigure(true);
            Context.DrawGeometry(b, BLACKPEN, pathGeometry);
        }

        public override void DrawText(string text, Point point, Color color)
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
