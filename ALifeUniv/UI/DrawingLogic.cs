using ALifeUni.ALife;
using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using Microsoft.Graphics.Canvas.Brushes;
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

        internal static void DrawWorldObject(WorldObject wo, LayerUISettings uiSettings, AgentUISettings auisettings, CanvasAnimatedDrawEventArgs args)
        {
            if(uiSettings.ShowObjects)
            {
                if(wo is Agent ag)
                {

                    DrawAgent(ag, uiSettings, auisettings, args);
                }
                else
                {

                    DrawShape(wo.Shape, uiSettings.ShowBoundingBoxes, args, true);
                }
            }
        }

        public static void DrawAncestry(CanvasAnimatedDrawEventArgs args)
        {
            for(int index = 0; index < Planet.World.StableActiveObjects.Count; index++)
            {
                WorldObject wo = Planet.World.StableActiveObjects[index];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    while(ag.LivingAncestor != null
                         && !ag.LivingAncestor.Alive)
                    {
                        ag.LivingAncestor = ag.LivingAncestor.LivingAncestor;
                    }

                    if(ag.LivingAncestor == null)
                    {
                        Circle c = (Circle)ag.Shape;
                        args.DrawingSession.DrawCircle(ag.Shape.CentrePoint.ToVector2(), c.Radius + 3, Colors.Blue);
                    }

                    else
                    {
                        CanvasLinearGradientBrush gradientBrush = new CanvasLinearGradientBrush(args.DrawingSession, Colors.Red, Colors.Black);
                        gradientBrush.StartPoint = ag.Shape.CentrePoint.ToVector2();
                        gradientBrush.EndPoint = ag.LivingAncestor.Shape.CentrePoint.ToVector2();
                        args.DrawingSession.DrawLine(ag.Shape.CentrePoint.ToVector2()
                                                     , ag.LivingAncestor.Shape.CentrePoint.ToVector2()
                                                     , gradientBrush);
                    }
                }
            }
        }

        public static void DrawPastState(LayerUISettings ui, AgentUISettings aui, CanvasAnimatedDrawEventArgs args, int compnumber)
        {
            foreach(WorldObject wo in Planet.World.CollisionLevels[ui.LayerName].EnumerateItems())
            {
                if(compnumber < wo.ExecutionOrder)
                {
                    DrawingLogic.DrawPastObject(wo.Shape, ui, args);
                    continue;
                }

                if(!(wo is Agent))
                {
                    //TODO: Should ALL objects have shadows??
                    DrawingLogic.DrawPastObject(wo.Shape, ui, args);
                    continue;
                }

                Agent ag = (Agent)wo;

                if(compnumber == wo.ExecutionOrder)
                {
                    DrawingLogic.DrawAgentShadow(ag.Shadow, ui, aui, args);
                    continue;
                }

                DrawingLogic.DrawPastObject(ag.Shadow.Shape, ui, args);
            }
        }

        private static void DrawAgent(Agent ag, LayerUISettings uiSettings, AgentUISettings auisettings, CanvasAnimatedDrawEventArgs args)
        {
            IShape shape = ag.Shape;

            DrawShape(shape, uiSettings.ShowBoundingBoxes, args, true);

            if(auisettings.ShowSenses)
            {
                foreach(IHasShape iHS in ag.Senses)
                {
                    DrawShape(iHS.Shape, auisettings.ShowSenseBoundingBoxes, args, false);
                }
            }

            //Draw Orientation
            DrawOrientation(args, shape);
        }

        private static void DrawOrientation(CanvasAnimatedDrawEventArgs args, IShape shape)
        {
            Point ori = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 4);
            Point ori2 = ExtraMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 1);
            args.DrawingSession.DrawLine(ori.ToVector2(), ori2.ToVector2(), Colors.DarkRed, 1);
        }

        internal static void DrawAgentShadow(AgentShadow shadow, LayerUISettings uiSettings, AgentUISettings auiSettings, CanvasAnimatedDrawEventArgs args)
        {
            Color orig = shadow.Shape.DebugColor;
            shadow.Shape.DebugColor = Colors.White;
            DrawShape(shadow.Shape, uiSettings.ShowBoundingBoxes, args, true);
            shadow.Shape.DebugColor = orig;
            //Draw Orientation
            DrawOrientation(args, shadow.Shape);

            if(auiSettings.ShowSenses)
            {
                foreach(IShape shape in shadow.SenseShapes)
                {
                    DrawShape(shape, auiSettings.ShowSenseBoundingBoxes, args, false);
                }
            }

        }

        internal static void DrawInactiveObject(WorldObject wo, LayerUISettings ui, CanvasAnimatedDrawEventArgs args)
        {
            Color color = wo.Shape.Color;
            wo.Shape.Color = Color.FromArgb(50, color.R, color.G, color.B);
            DrawShape(wo.Shape, ui.ShowBoundingBoxes, args, true);
            wo.Shape.Color = Color.FromArgb(50, 255, 0, 0);
            DrawShape(wo.Shape, ui.ShowBoundingBoxes, args, false);
            wo.Shape.Color = color;
        }

        internal static void DrawPastObject(IShape shape, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            Color pastDebug = shape.DebugColor;
            shape.DebugColor = Colors.White;
            DrawShape(shape, uiSettings.ShowBoundingBoxes, args, true);
            shape.DebugColor = pastDebug;
        }

        private static void DrawShape(IShape shape, bool showBoundingBox, CanvasAnimatedDrawEventArgs args, bool fillIn)
        {
            switch(shape)
            {
                case Circle cir: DrawCircle(cir, args, fillIn); break;
                case Sector sec: DrawSector(sec, args, fillIn); break;
                case Rectangle rect: DrawRectangle(rect, args, fillIn); break;
                case AARectangle aar: DrawAARectangle(aar, args); break;
                default: throw new NotImplementedException("What the heck shape is this?");
            }
            if(showBoundingBox)
            {
                DrawBoundingBox(shape.BoundingBox, Colors.Black, args);
            }
        }

        private static void DrawSector(IShape currShape, CanvasAnimatedDrawEventArgs args, bool fillIn)
        {
            //ChildSector sec = (ChildSector)currShape;

            Sector sec = (Sector)currShape;
            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(args.DrawingSession);
            Angle myAngle = sec.Orientation;
            Vector2 centre = sec.CentrePoint.ToVector2();
            pathBuilder.BeginFigure(centre);
            pathBuilder.AddArc(centre, sec.Radius, sec.Radius, (float)myAngle.Radians, (float)sec.SweepAngle.Radians);
            pathBuilder.EndFigure(CanvasFigureLoop.Closed);
            CanvasGeometry cg = CanvasGeometry.CreatePath(pathBuilder);


            //args.DrawingSession.FillGeometry(cg, sec.DebugColor);
            if(fillIn)
            {
                args.DrawingSession.FillGeometry(cg, sec.Color);
            }
            else
            {
                args.DrawingSession.DrawGeometry(cg, sec.Color, 1);
            }

            DrawOrientation(args, currShape);
        }

        private static void DrawCircle(Circle wo, CanvasAnimatedDrawEventArgs args, bool fillIn)
        {
            Vector2 objectCentre = wo.CentrePoint.ToVector2();
            //World Object Body
            if(fillIn)
            {
                args.DrawingSession.FillCircle(objectCentre, wo.Radius, wo.Color);
            }
            else
            {
                args.DrawingSession.DrawCircle(objectCentre, wo.Radius, wo.Color);
            }

            //Core of the body is the debug colour
            args.DrawingSession.FillCircle(objectCentre, 2, wo.DebugColor);

            DrawOrientation(args, wo);
        }

        private static void DrawRectangle(Rectangle rec, CanvasAnimatedDrawEventArgs args, bool fillIn)
        {
            //Point rovingPoint = rec.CentrePoint;
            //rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians, rec.FBLength / 2);
            //rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians - (Math.PI / 2), rec.RLWidth / 2);

            //CanvasPathBuilder cpb = new CanvasPathBuilder(args.DrawingSession.Device);
            //cpb.BeginFigure(rovingPoint.ToVector2());

            //rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + (Math.PI / 2), rec.RLWidth);
            //cpb.AddLine(rovingPoint.ToVector2());

            //rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + Math.PI, rec.FBLength);
            //cpb.AddLine(rovingPoint.ToVector2());

            //rovingPoint = ExtraMath.TranslateByVector(rovingPoint, rec.Orientation.Radians + (Math.PI * 3 / 2), rec.RLWidth);
            //cpb.AddLine(rovingPoint.ToVector2());
            CanvasPathBuilder cpb = new CanvasPathBuilder(args.DrawingSession.Device);
            cpb.BeginFigure(rec.TopLeft.ToVector2());
            cpb.AddLine(rec.TopRight.ToVector2());
            cpb.AddLine(rec.BottomRight.ToVector2());
            cpb.AddLine(rec.BottomLeft.ToVector2());
            cpb.EndFigure(CanvasFigureLoop.Closed);
            CanvasGeometry cg = CanvasGeometry.CreatePath(cpb);

            if(fillIn)
            {
                args.DrawingSession.FillGeometry(cg, rec.Color);
            }
            else
            {
                args.DrawingSession.DrawGeometry(cg, rec.Color, 1);
            }

            args.DrawingSession.FillCircle(rec.CentrePoint.ToVector2(), 2, rec.DebugColor);

            DrawOrientation(args, rec);
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
