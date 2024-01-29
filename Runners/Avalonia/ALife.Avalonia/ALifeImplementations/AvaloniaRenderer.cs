using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using Avalonia;
using Avalonia.Media;
using System;
using System.Globalization;
using AvColor = Avalonia.Media.Color;
using AvPoint = Avalonia.Point;
using AvSize = Avalonia.Size;
using Point = ALife.Core.GeometryOld.Shapes.Point;
using ALife.Core.GeometryOld.Shapes;

namespace ALife.Avalonia.ALifeImplementations
{
    public class AvaloniaRenderer : AbstractRenderer
    {
        private readonly Pen BLACKPEN = new(Brushes.Black, 0.4);

        private DrawingContext? context = null;

        private DrawingContext Context
        {
            get => context is null ? throw new Exception("Please don't use context before setting. It upsets the natural order.") : context;
            set => context = value;
        }

        public override void DrawAARectangle(Point maxXY, Point minXY, Colour color, double strokeWidth)
        {
            Rect rect = new(ConvertPoint(minXY), ConvertPoint(maxXY));
            Brush b = new SolidColorBrush(ConvertColour(color));
            Pen p = new(b, strokeWidth);
            Context.DrawRectangle(p, rect);
        }

        public override void DrawAgentAncestry(Agent agent)
        {
            throw new NotImplementedException();
        }

        public override void DrawCircle(Point centerPoint, float radius, Colour color)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.DrawEllipse(null, new Pen(b, 1), new AvPoint(centerPoint.X, centerPoint.Y), radius, radius);
        }

        public override void DrawLine(Point point1, Point point2, Colour color, double strokeWidth)
        {
            Brush brush = new SolidColorBrush(ConvertColour(color), strokeWidth);
            Context.DrawLine(new Pen(brush), ConvertPoint(point1), ConvertPoint(point2));
        }

        public override void DrawRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Colour color, double strokeWidth)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            Pen pen = new(b, strokeWidth);
            AvPoint p1 = ConvertPoint(topLeft);
            AvPoint p2 = ConvertPoint(topRight);
            AvPoint p3 = ConvertPoint(bottomLeft);
            AvPoint p4 = ConvertPoint(bottomRight);

            Context.DrawLine(pen, p1, p2);
            Context.DrawLine(pen, p2, p3);
            Context.DrawLine(pen, p3, p4);
            Context.DrawLine(pen, p4, p1);
        }

        public override void DrawSector(Sector currShape, bool fillIn)
        {
            Brush b = new SolidColorBrush(ConvertColour(currShape.Colour));
            Pen p = new Pen(b);

            PathGeometry pathGeometry = new();
            StreamGeometryContext geoStream = pathGeometry.Open();
            geoStream.BeginFigure(ConvertPoint(currShape.CentrePoint), fillIn);
            geoStream.LineTo(ConvertPoint(currShape.LeftPoint));
            geoStream.ArcTo(ConvertPoint(currShape.RightPoint), new AvSize(currShape.Radius, currShape.Radius), 0, false, SweepDirection.Clockwise);
            geoStream.EndFigure(true);
            Context.DrawGeometry(b, p, pathGeometry);
        }

        public override void DrawText(string text, Point point, Colour color)
        {
            FormattedText ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, 12, new SolidColorBrush(ConvertColour(color)));
            Context.DrawText(ft, ConvertPoint(point));
        }

        public override void FillAARectangle(Point minXY, Point maxXY, Colour color)
        {
            Rect rect = new(ConvertPoint(minXY), ConvertPoint(maxXY));
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.FillRectangle(b, rect);
        }

        public override void FillCircle(Point centerPoint, float radius, Colour color)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            Context.DrawEllipse(b, BLACKPEN, ConvertPoint(centerPoint), radius, radius);
        }

        public override void FillRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Colour color)
        {
            Brush b = new SolidColorBrush(ConvertColour(color));
            PathGeometry pathGeometry = new();
            StreamGeometryContext geoStream = pathGeometry.Open();
            geoStream.BeginFigure(ConvertPoint(topLeft), true);
            geoStream.LineTo(ConvertPoint(topRight));
            geoStream.LineTo(ConvertPoint(bottomRight));
            geoStream.LineTo(ConvertPoint(bottomLeft));
            geoStream.EndFigure(true);
            Context.DrawGeometry(b, BLACKPEN, pathGeometry);
        }

        public override void SetContext(object drawingContext)
        {
            Context = (DrawingContext)drawingContext;
        }

        private AvColor ConvertColour(Colour c)
        {
            return AvColor.FromArgb(c.A, c.R, c.G, c.B);
        }

        private AvPoint ConvertPoint(Point p)
        {
            return new AvPoint(p.X, p.Y);
        }
    }
}
