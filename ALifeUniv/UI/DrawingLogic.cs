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
        internal static void DrawWorldObject(WorldObject wo,LayerUISettings uiSettings,  CanvasAnimatedDrawEventArgs args)
        {
            if(!(wo is Agent)
                && uiSettings.ShowObjects)
            {
                DrawObject(wo, uiSettings, args);
            }
            if(wo is Agent
                && uiSettings.ShowAgents)
            {
                DrawObject(wo, uiSettings, args);

                Agent ag = (Agent)wo;
                //Draw Orientation
                float newX = (float)(wo.CentrePoint.X + wo.Radius * Math.Cos(ag.Orientation.Radians));
                float newY = (float)(wo.CentrePoint.Y + wo.Radius * Math.Sin(ag.Orientation.Radians));
                args.DrawingSession.FillCircle(new Vector2(newX, newY), 1, Colors.DarkCyan);

                if(uiSettings.ShowSenses)
                {
                    foreach(IHasShape shape in ag.Senses)
                    {
                        DrawSense(uiSettings, args, shape);
                    }
                }
            }
        }

        private static void DrawSense(LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args, IHasShape shape)
        {
            IShape currShape = shape.GetShape();
            if(currShape is Sector)
            {
                ChildSector sec = (ChildSector)currShape;
                if(uiSettings.ShowBoundingBoxes)
                {
                    DrawBoundingBox(sec.BoundingBox, Colors.PaleGreen, args);
                }

                CanvasPathBuilder pathBuilder = new CanvasPathBuilder(args.DrawingSession);
                Angle myAngle = sec.Orientation + sec.OrientationAroundParent + sec.Parent.Orientation;
                Vector2 centre = new Vector2((float)sec.CentrePoint.X, (float)sec.CentrePoint.Y);
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

        private static void DrawObject(WorldObject wo, LayerUISettings ui, CanvasAnimatedDrawEventArgs args)
        {
            Vector2 objectCentre = new Vector2((float)wo.CentrePoint.X, (float)wo.CentrePoint.Y);
            //World Object Body
            args.DrawingSession.FillCircle(objectCentre, wo.Radius, wo.Color);
            //Core of the body is the debug colour
            args.DrawingSession.FillCircle(objectCentre, 2, wo.GetShape().DebugColor);

            if(ui.ShowBoundingBoxes)
            {
                DrawBoundingBox(wo.BoundingBox, Colors.PaleGreen, args);
            }
        }

        internal static void DrawBoundingBox(BoundingBox bb, Color color, CanvasAnimatedDrawEventArgs args)
        {
            Point maxPoints = new Point(bb.MaxX, bb.MaxY);
            Point minPoints = new Point(bb.MinX, bb.MinY);
            Rect drawRec = new Rect(maxPoints, minPoints);
            args.DrawingSession.DrawRectangle(drawRec, color, 0.3f);
        }
    }
}
