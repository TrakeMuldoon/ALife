using ALifeUni.ALife;
using ALifeUni.ALife.UtilityClasses;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.UI
{
    public static class DrawingLogic
    {
        internal static void DrawZone(Zone zone, CanvasAnimatedDrawEventArgs args)
        {
            DrawAARectangle(zone, args);
            Color textColor = zone.Color;
            textColor.A = 255;
            args.DrawingSession.DrawText(zone.Name, zone.TopLeft.ToVector2(), textColor);
        }

        internal static void DrawWorldObject(WorldObject wo, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            if(!(wo is Agent)
                && uiSettings.ShowObjects)
            {
                DrawShape(wo.Shape, uiSettings, args);
            }
            else if(wo is Agent
                && uiSettings.ShowAgents)
            {
                DrawAgent((Agent)wo, uiSettings, args);
            }
            else
            {
                throw new Exception("What the hell is this??");
            }
        }

        private static void DrawAgent(Agent ag, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            IShape shape = ag.Shape;

            DrawShape(shape, uiSettings, args);

            if(!(shape is Circle))
            {
                throw new NotImplementedException("Only Circular Agents are supported");
            }

            //Draw Orientation
            Circle aC = shape as Circle;
            Point ori = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation.Radians, aC.Radius);
            Point ori2 = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation.Radians, aC.Radius-2);
            args.DrawingSession.DrawLine(ori.ToVector2(), ori2.ToVector2(), Colors.DarkRed, 1);

            if(uiSettings.ShowSenses)
            {
                foreach(IHasShape iHS in ag.Senses)
                {
                    DrawShape(iHS.Shape, uiSettings, args);
                }
            }
        }

        internal static void DrawAgentShadow(AgentShadow shadow, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            Color orig = shadow.DebugColor;
            shadow.DebugColor = Colors.White;
            DrawCircle(shadow, uiSettings, args);
            shadow.DebugColor = orig;
            Point ori = ExtraMath.TranslateByVector(shadow.CentrePoint, shadow.Orientation.Radians, shadow.Radius);
            args.DrawingSession.FillCircle(ori.ToVector2(), 1, Colors.DarkRed);
            foreach(IShape shape in shadow.SenseShapes)
            {
                DrawShape(shape, uiSettings, args);
            }
        }

        internal static void DrawPastObject(IShape shape, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            Color pastDebug = shape.DebugColor;
            shape.DebugColor = Colors.White;
            DrawShape(shape, uiSettings, args);
            shape.DebugColor = pastDebug;
        }

        private static void DrawShape(IShape shape, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            switch(shape)
            {
                case Circle cir: DrawCircle(cir, uiSettings, args); break;
                case Sector sec: DrawSector(sec, uiSettings, args); break;
                case Rectangle rect: DrawRectangle(rect, uiSettings, args); break;
                case AARectangle aar: DrawAARectangle(aar, args); break;
                default: throw new NotImplementedException("What the heck shape is this?");
            }
        }

        private static void DrawSector(IShape currShape, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            ChildSector sec = (ChildSector)currShape;
            if(uiSettings.ShowBoundingBoxes)
            {
                DrawBoundingBox(sec.BoundingBox, Colors.Black, args);
            }

            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(args.DrawingSession);
            Angle myAngle = sec.Orientation + sec.OrientationAroundParent + sec.Parent.Orientation;
            Vector2 centre = sec.CentrePoint.ToVector2();
            pathBuilder.BeginFigure(centre);
            pathBuilder.AddArc(centre, sec.Radius, sec.Radius, (float)myAngle.Radians, (float)sec.SweepAngle.Radians);
            pathBuilder.EndFigure(CanvasFigureLoop.Closed);
            CanvasGeometry cg = CanvasGeometry.CreatePath(pathBuilder);

            args.DrawingSession.FillGeometry(cg, sec.DebugColor);
            args.DrawingSession.DrawGeometry(cg, sec.Color, 1);
        }

        private static void DrawCircle(Circle wo, LayerUISettings ui, CanvasAnimatedDrawEventArgs args)
        {
            Vector2 objectCentre = wo.CentrePoint.ToVector2();
            //World Object Body
            args.DrawingSession.FillCircle(objectCentre, wo.Radius, wo.Color);
            //Core of the body is the debug colour
            args.DrawingSession.FillCircle(objectCentre, 2, wo.DebugColor);

            if(ui.ShowBoundingBoxes)
            {
                DrawBoundingBox(wo.BoundingBox, Colors.Black, args);
            }
        }


        private static void DrawRectangle(Rectangle rec, LayerUISettings ui, CanvasAnimatedDrawEventArgs args)
        {
            Point rovingPoint = rec.CentrePoint;
            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians, rec.FBLength / 2);
            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians - (Math.PI / 2), rec.RLWidth / 2);

            CanvasPathBuilder cpb = new CanvasPathBuilder(args.DrawingSession.Device);
            cpb.BeginFigure(rovingPoint.ToVector2());

            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + (Math.PI / 2), rec.RLWidth);
            cpb.AddLine(rovingPoint.ToVector2());

            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + Math.PI, rec.FBLength);
            cpb.AddLine(rovingPoint.ToVector2());

            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + (Math.PI * 3 / 2), rec.RLWidth);
            cpb.AddLine(rovingPoint.ToVector2());

            cpb.EndFigure(CanvasFigureLoop.Closed);
            CanvasGeometry cg = CanvasGeometry.CreatePath(cpb);

            args.DrawingSession.DrawGeometry(cg, rec.Color);
            args.DrawingSession.FillCircle(rec.CentrePoint.ToVector2(), 2, rec.DebugColor);

            if(ui.ShowBoundingBoxes)
            {
                DrawBoundingBox(rec.BoundingBox, Colors.Black, args);
            }
        }

        private static void DrawAARectangle(AARectangle rec, CanvasAnimatedDrawEventArgs args)
        {
            args.DrawingSession.FillRectangle((float)rec.TopLeft.X, (float)rec.TopLeft.Y, (float)rec.XWidth, (float)rec.YHeight, rec.Color);
            args.DrawingSession.FillCircle(rec.CentrePoint.ToVector2(), 2, rec.DebugColor);
        }

        private static void DrawBoundingBox(BoundingBox bb, Color color, CanvasAnimatedDrawEventArgs args)
        {
            Point maxPoints = new Point(bb.MaxX, bb.MaxY);
            Point minPoints = new Point(bb.MinX, bb.MinY);
            Rect drawRec = new Rect(maxPoints, minPoints);
            args.DrawingSession.DrawRectangle(drawRec, color, 0.3f);
        }

    }
}
