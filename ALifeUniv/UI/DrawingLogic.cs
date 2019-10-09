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
            //Agent Orientation
            if (wo is Agent)
            {
                Agent ag = (Agent)wo;
                float newX = (float)(wo.CentrePoint.X + wo.Radius * Math.Cos(ag.Orientation.Radians));
                float newY = (float)(wo.CentrePoint.Y + wo.Radius * Math.Sin(ag.Orientation.Radians));
                args.DrawingSession.FillCircle(new Vector2(newX, newY), 1, Colors.DarkCyan);

                DrawBoundingBox(ag.GetBoundingBox(), args, Colors.AntiqueWhite);

                foreach(IHasShape shape in ag.Senses)
                {
                    IShape currShape = shape.GetShape();
                    if(currShape is Sector)
                    {
                        ChildSector sec = (ChildSector)currShape;
                        DrawBoundingBox(sec.GetBoundingBox(), args, Colors.Green);


                        CanvasPathBuilder pathBuilder = new CanvasPathBuilder(args.DrawingSession);
                        Angle myAngle = sec.OrientationAngle + sec.OrientationAroundParent + sec.Parent.GetOrientation();
                        Vector2 centre = new Vector2((float)sec.GetCentrePoint().X, (float)sec.GetCentrePoint().Y);
                        pathBuilder.BeginFigure(centre);
                        pathBuilder.AddArc(centre, sec.Radius, sec.Radius, (float)myAngle.Radians, (float)sec.SweepAngle.Radians);
                        pathBuilder.EndFigure(CanvasFigureLoop.Closed);
                        CanvasGeometry cg = CanvasGeometry.CreatePath(pathBuilder);

                        args.DrawingSession.FillGeometry(cg, Colors.Black);
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
