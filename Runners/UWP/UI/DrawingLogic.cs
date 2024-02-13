using ALife.Core;
using ALife.Core.Geometry.OLD;
using ALife.Core.Geometry.OLD.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
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
            Color textColor = zone.Colour.ToWinUiColor();
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
            Agent arbitraryAgent = Planet.World.CollisionLevels[ui.LayerName].EnumerateItems().OfType<Agent>().FirstOrDefault();
            if(arbitraryAgent == null
                || arbitraryAgent.Shadow == null)
            {
                //We are not in a ShadowGenerating circumstance.
                return;
            }

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

            if(ag.JustReproduced)
            {
                Circle c = ag.Shape as Circle;
                DrawCircle(new Circle(c.CentrePoint, c.Radius + 2) { Colour = Colour.HotPink }, args, false);
            }

            //Draw Orientation
            DrawOrientation(args, shape);
        }

        private static void DrawOrientation(CanvasAnimatedDrawEventArgs args, IShape shape)
        {
            Windows.Foundation.Point ori = GeometryMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 4).ToUwpPoint();
            Windows.Foundation.Point ori2 = GeometryMath.TranslateByVector(shape.CentrePoint, shape.Orientation, 1).ToUwpPoint();
            args.DrawingSession.DrawLine(ori.ToVector2(), ori2.ToVector2(), Colors.DarkRed, 1);
        }

        internal static void DrawAgentShadow(AgentShadow shadow, LayerUISettings uiSettings, AgentUISettings auiSettings, CanvasAnimatedDrawEventArgs args)
        {
            Colour orig = shadow.Shape.DebugColour;
            shadow.Shape.DebugColour = Colour.White;
            DrawShape(shadow.Shape, uiSettings.ShowBoundingBoxes, args, true);
            shadow.Shape.DebugColour = orig;
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
            Colour color = wo.Shape.Colour;
            wo.Shape.Colour = Colour.FromARGB(50, color.R, color.G, color.B);
            DrawShape(wo.Shape, ui.ShowBoundingBoxes, args, true);
            wo.Shape.Colour = Colour.FromARGB(50, 255, 0, 0);
            DrawShape(wo.Shape, ui.ShowBoundingBoxes, args, false);
            wo.Shape.Colour = color;
        }

        internal static void DrawPastObject(IShape shape, LayerUISettings uiSettings, CanvasAnimatedDrawEventArgs args)
        {
            Colour pastDebug = shape.DebugColour;
            shape.DebugColour = Colour.White;
            DrawShape(shape, uiSettings.ShowBoundingBoxes, args, true);
            shape.DebugColour = pastDebug;
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
                DrawBoundingBox(shape.BoundingBox, Colour.Black, args);
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


            //args.DrawingSession.FillGeometry(cg, sec.DebugColour);
            if(fillIn)
            {
                args.DrawingSession.FillGeometry(cg, sec.Colour.ToWinUiColor());
            }
            else
            {
                args.DrawingSession.DrawGeometry(cg, sec.Colour.ToWinUiColor(), 1);
            }

            DrawOrientation(args, currShape);
        }

        private static void DrawCircle(Circle wo, CanvasAnimatedDrawEventArgs args, bool fillIn)
        {
            Vector2 objectCentre = wo.CentrePoint.ToVector2();
            //World Object Body
            if(fillIn)
            {
                args.DrawingSession.FillCircle(objectCentre, wo.Radius, wo.Colour.ToWinUiColor());
            }
            else
            {
                args.DrawingSession.DrawCircle(objectCentre, wo.Radius, wo.Colour.ToWinUiColor());
            }

            //Core of the body is the debug colour
            args.DrawingSession.FillCircle(objectCentre, 2, wo.DebugColour.ToWinUiColor());

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
                args.DrawingSession.FillGeometry(cg, rec.Colour.ToWinUiColor());
            }
            else
            {
                args.DrawingSession.DrawGeometry(cg, rec.Colour.ToWinUiColor(), 1);
            }

            args.DrawingSession.FillCircle(rec.CentrePoint.ToVector2(), 2, rec.DebugColour.ToWinUiColor());

            DrawOrientation(args, rec);
        }

        private static void DrawAARectangle(AARectangle rec, CanvasAnimatedDrawEventArgs args)
        {
            args.DrawingSession.FillRectangle((float)rec.TopLeft.X, (float)rec.TopLeft.Y, (float)rec.XWidth, (float)rec.YHeight, rec.Colour.ToWinUiColor());
            args.DrawingSession.FillCircle(rec.CentrePoint.ToVector2(), 2, rec.DebugColour.ToWinUiColor());
        }

        private static void DrawBoundingBox(BoundingBox bb, Colour color, CanvasAnimatedDrawEventArgs args)
        {
            Windows.Foundation.Point maxPoints = new Windows.Foundation.Point(bb.MaxX, bb.MaxY);
            Windows.Foundation.Point minPoints = new Windows.Foundation.Point(bb.MinX, bb.MinY);
            Rect drawRec = new Rect(maxPoints, minPoints);
            args.DrawingSession.DrawRectangle(drawRec, color.ToWinUiColor(), 0.3f);
        }

    }
}
