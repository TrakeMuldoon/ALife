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
        internal static void DrawWorldObject(WorldObject wo, CanvasAnimatedDrawEventArgs args)
        {
            Vector2 agentCentre = new Vector2((float)wo.CentrePoint.X, (float)wo.CentrePoint.Y);
            //Agent Body
            args.DrawingSession.FillCircle(agentCentre, wo.Radius, wo.Color);
            //Core of the body is the debug colour
            args.DrawingSession.FillCircle(agentCentre, 2, wo.GetShape().DebugColor);

            //Agent Orientation
            if(wo is Agent)
            {
                Agent ag = (Agent)wo;
                //Draw Orientation
                float newX = (float)(wo.CentrePoint.X + wo.Radius * Math.Cos(ag.Orientation.Radians));
                float newY = (float)(wo.CentrePoint.Y + wo.Radius * Math.Sin(ag.Orientation.Radians));
                args.DrawingSession.FillCircle(new Vector2(newX, newY), 1, Colors.DarkCyan);

                DrawBoundingBox(ag.BoundingBox, args, Colors.AntiqueWhite);

                foreach(IHasShape shape in ag.Senses)
                {
                    IShape currShape = shape.GetShape();
                    if(currShape is Sector)
                    {
                        ChildSector sec = (ChildSector)currShape;
                        DrawBoundingBox(sec.BoundingBox, args, Colors.Green);


                        CanvasPathBuilder pathBuilder = new CanvasPathBuilder(args.DrawingSession);
                        Angle myAngle = sec.Orientation + sec.OrientationAroundParent + sec.Parent.Orientation;
                        Vector2 centre = new Vector2((float)sec.CentrePoint.X, (float)sec.CentrePoint.Y);
                        pathBuilder.BeginFigure(centre);
                        pathBuilder.AddArc(centre, sec.Radius, sec.Radius, (float)myAngle.Radians, (float)sec.SweepAngle.Radians);
                        pathBuilder.EndFigure(CanvasFigureLoop.Closed);
                        CanvasGeometry cg = CanvasGeometry.CreatePath(pathBuilder);

                        args.DrawingSession.FillGeometry(cg, sec.Color);
                    }
                }
            }
        }

        internal static void DrawBoundingBox(BoundingBox bb, CanvasAnimatedDrawEventArgs args, Color color)
        {
            Point maxPoints = new Point(bb.MaxX, bb.MaxY);
            Point minPoints = new Point(bb.MinX, bb.MinY);
            Rect drawRec = new Rect(maxPoints, minPoints);
            args.DrawingSession.DrawRectangle(drawRec, color, 0.3f);
        }
    }
}
