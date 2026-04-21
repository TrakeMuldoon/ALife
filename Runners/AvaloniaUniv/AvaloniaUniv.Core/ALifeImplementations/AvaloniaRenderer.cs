using ALife.Core.Geometry.Shapes;
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
using Point = ALife.Core.Geometry.Shapes.Point;

namespace AvaloniaUniv.Core.ALifeImplementations;

public class AvaloniaRenderer : AbstractRenderer
{
    private readonly Pen _blackPen = new(Brushes.Black, 0.4);
    private DrawingContext? _context;

    private DrawingContext Context
    {
        get => _context ?? throw new InvalidOperationException("Renderer context not set.");
        set => _context = value;
    }

    public override void SetContext(object context) => Context = (DrawingContext)context;

    public override void DrawText(string text, Point point, Colour color)
    {
        var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            Typeface.Default, 12, new SolidColorBrush(ToAvColor(color)));
        Context.DrawText(ft, ToAvPoint(point));
    }

    public override void DrawAgentAncestry(Agent agent)
    {
        if (agent.LivingAncestor == null) return;
        var start = ToAvPoint(agent.Shape.CentrePoint);
        var end = ToAvPoint(agent.LivingAncestor.Shape.CentrePoint);
        var pen = new Pen(new SolidColorBrush(ToAvColor(agent.Shape.Colour), 0.6), 1.0);
        Context.DrawLine(pen, start, end);
    }

    public override void DrawCircle(Point centerPoint, float radius, Colour color)
    {
        var brush = new SolidColorBrush(ToAvColor(color));
        Context.DrawEllipse(null, new Pen(brush, 1), ToAvPoint(centerPoint), radius, radius);
    }

    public override void FillCircle(Point centerPoint, float radius, Colour color)
    {
        var brush = new SolidColorBrush(ToAvColor(color));
        Context.DrawEllipse(brush, _blackPen, ToAvPoint(centerPoint), radius, radius);
    }

    public override void DrawLine(Point point1, Point point2, Colour color, double strokeWidth)
    {
        var brush = new SolidColorBrush(ToAvColor(color));
        Context.DrawLine(new Pen(brush, strokeWidth), ToAvPoint(point1), ToAvPoint(point2));
    }

    public override void DrawRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Colour color, double strokeWidth)
    {
        var pen = new Pen(new SolidColorBrush(ToAvColor(color)), strokeWidth);
        Context.DrawLine(pen, ToAvPoint(topLeft), ToAvPoint(topRight));
        Context.DrawLine(pen, ToAvPoint(topRight), ToAvPoint(bottomRight));
        Context.DrawLine(pen, ToAvPoint(bottomRight), ToAvPoint(bottomLeft));
        Context.DrawLine(pen, ToAvPoint(bottomLeft), ToAvPoint(topLeft));
    }

    public override void FillRectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, Colour color)
    {
        var brush = new SolidColorBrush(ToAvColor(color));
        var geo = new PathGeometry();
        var ctx = geo.Open();
        ctx.BeginFigure(ToAvPoint(topLeft), true);
        ctx.LineTo(ToAvPoint(topRight));
        ctx.LineTo(ToAvPoint(bottomRight));
        ctx.LineTo(ToAvPoint(bottomLeft));
        ctx.EndFigure(true);
        Context.DrawGeometry(brush, _blackPen, geo);
    }

    public override void DrawAARectangle(Point topLeft, Point bottomRight, Colour color, double strokeWidth)
    {
        var rect = new Rect(ToAvPoint(topLeft), ToAvPoint(bottomRight));
        var pen = new Pen(new SolidColorBrush(ToAvColor(color)), strokeWidth);
        Context.DrawRectangle(pen, rect);
    }

    public override void FillAARectangle(Point minXY, Point maxXY, Colour color)
    {
        var rect = new Rect(ToAvPoint(minXY), ToAvPoint(maxXY));
        var brush = new SolidColorBrush(ToAvColor(color));
        Context.FillRectangle(brush, rect);
    }

    public override void DrawSector(Sector sector, bool fillIn)
    {
        var brush = new SolidColorBrush(ToAvColor(sector.Colour));
        var pen = new Pen(brush);
        var geo = new PathGeometry();
        var ctx = geo.Open();
        ctx.BeginFigure(ToAvPoint(sector.CentrePoint), fillIn);
        ctx.LineTo(ToAvPoint(sector.LeftPoint));
        ctx.ArcTo(ToAvPoint(sector.RightPoint), new AvSize(sector.Radius, sector.Radius), 0, false, SweepDirection.Clockwise);
        ctx.EndFigure(true);
        Context.DrawGeometry(fillIn ? brush : null, pen, geo);
    }

    private static AvColor ToAvColor(Colour c) => AvColor.FromArgb(c.A, c.R, c.G, c.B);
    private static AvPoint ToAvPoint(Point p) => new(p.X, p.Y);
}
