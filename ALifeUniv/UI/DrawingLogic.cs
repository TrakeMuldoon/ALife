using ALifeUni.ALife;
using ALifeUni.ALife.UtilityClasses;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.UI
{
    public static class DrawingLogic
    {
        internal static void DrawZone(AARectangle zone, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {

        }

        internal static void DrawWorldObject(WorldObject wo, LayerUISettings uiSettings,  CanvasAnimatedDrawEventArgs args)
        {
            if(!(wo is Agent)
                && uiSettings.ShowObjects)
            {
                DrawObjectBase(wo, uiSettings, args);
            }
            else if(wo is Agent
                && uiSettings.ShowAgents)
            {
                DrawObjectBase(wo, uiSettings, args);

                Agent ag = (Agent)wo;
                //Draw Orientation
                float newX = (float)(wo.CentrePoint.X + wo.Radius * Math.Cos(ag.Orientation.Radians));
                float newY = (float)(wo.CentrePoint.Y + wo.Radius * Math.Sin(ag.Orientation.Radians));
                args.DrawingSession.FillCircle(new Vector2(newX, newY), 1, Colors.DarkCyan);

                if(uiSettings.ShowSenses)
                {
                    foreach(IHasShape shape in ag.Senses)
                    {
                        DrawSense(shape, uiSettings, args);
                    }
                }
            }
            else
            {
                throw new Exception("What the hell is this??");
            }
        }

        internal static void DrawAgentShadow(AgentShadow wo, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            Color orig = wo.DebugColor;
            wo.DebugColor = Colors.White;
            DrawWorldObject(wo, uiSettings, args);
            wo.DebugColor = orig;
            foreach(IHasShape shape in wo.Senses)
            {
                DrawSense(shape, uiSettings, args);
            }
        }

        private static void DrawSense(IHasShape shape, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            IShape currShape = shape.GetShape();
            if(currShape is Sector)
            {
                ChildSector sec = (ChildSector)currShape;
                if(uiSettings.ShowBoundingBoxes)
                {
                    DrawBoundingBox(sec.BoundingBox, Colors.Black, args);
                }

                CanvasPathBuilder pathBuilder = new CanvasPathBuilder(args.DrawingSession);
                Angle myAngle = sec.Orientation + sec.OrientationAroundParent + sec.Parent.Orientation;
                Vector2 centre = ExtraMath.PointToVector(sec.CentrePoint);
                pathBuilder.BeginFigure(centre);
                pathBuilder.AddArc(centre, sec.Radius, sec.Radius, (float)myAngle.Radians, (float)sec.SweepAngle.Radians);
                pathBuilder.EndFigure(CanvasFigureLoop.Closed);
                CanvasGeometry cg = CanvasGeometry.CreatePath(pathBuilder);

                args.DrawingSession.FillGeometry(cg, sec.DebugColor);
                args.DrawingSession.DrawGeometry(cg, sec.Color, 1);
            }
            else
            {
                throw new NotImplementedException("What the hell shape is this?");
            }    
        }

        private static void DrawObjectBase(WorldObject wo, LayerUISettings ui, CanvasAnimatedDrawEventArgs args)
        {
            Vector2 objectCentre = new Vector2((float)wo.CentrePoint.X, (float)wo.CentrePoint.Y);
            //World Object Body
            args.DrawingSession.FillCircle(objectCentre, wo.Radius, wo.Color);
            //Core of the body is the debug colour
            args.DrawingSession.FillCircle(objectCentre, 2, wo.GetShape().DebugColor);

            if(ui.ShowBoundingBoxes)
            {
                DrawBoundingBox(wo.BoundingBox, Colors.Black, args);
            }
        }

        internal static void DrawBoundingBox(BoundingBox bb, Color color, CanvasAnimatedDrawEventArgs args)
        {
            Point maxPoints = new Point(bb.MaxX, bb.MaxY);
            Point minPoints = new Point(bb.MinX, bb.MinY);
            Rect drawRec = new Rect(maxPoints, minPoints);
            args.DrawingSession.DrawRectangle(drawRec, color, 0.3f);
        }

        internal static void DrawRectangle(Rectangle rec, CanvasAnimatedDrawEventArgs args)
        {
            Point rovingPoint = rec.TopLeft;
            CanvasPathBuilder cpb = new CanvasPathBuilder(args.DrawingSession.Device);
            cpb.BeginFigure(ExtraMath.PointToVector(rovingPoint));
            
            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians, rec.Width);
            cpb.AddLine(ExtraMath.PointToVector(rovingPoint));
            
            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + Math.PI/2, rec.Width);
            cpb.AddLine(ExtraMath.PointToVector(rovingPoint));

            rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + Math.PI, rec.Width);
            cpb.AddLine(ExtraMath.PointToVector(rovingPoint));

            cpb.EndFigure(CanvasFigureLoop.Closed);
            CanvasGeometry cg = CanvasGeometry.CreatePath(cpb);

            args.DrawingSession.DrawGeometry(cg, rec.Color);
        }
    }
}
