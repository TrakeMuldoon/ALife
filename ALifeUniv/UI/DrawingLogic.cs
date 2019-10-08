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

                BoundingBox bb = ag.GetBoundingBox();
                Point maxPoints = new Point(bb.MaxX, bb.MaxY);
                Point minPoints = new Point(bb.MinX, bb.MinY);
                Rect drawRec = new Rect(maxPoints, minPoints);
                args.DrawingSession.DrawRectangle(drawRec, Colors.AntiqueWhite);

                foreach(IHasShape shape in ag.Senses)
                {
                    IShape currShape = shape.GetShape();
                    if(currShape is Sector)
                    {
                        Sector sec = (Sector)currShape;
                        CanvasPathBuilder pathBuilder = new CanvasPathBuilder(args.DrawingSession);

                        //Angle myAngle = sec.OrientationAngle + sec.OrientationAroundParent + sec.OrientationOfParent;
                        //Vector2 vec2 = new Vector2((float)sec.CentrePoint.X, (float)sec.CentrePoint.Y);
                        //pathBuilder.AddArc(vec2, sec.Radius, sec.Radius, (float)myAngle.Radians, (float)sec.SweepAngle.Radians);

                    }
                }
            }
        }
    }
}
