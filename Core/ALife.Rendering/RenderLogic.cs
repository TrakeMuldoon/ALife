using ALife.Core;
using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using Color = System.Drawing.Color;

namespace ALife.Rendering
{
    /// <summary>
    /// Main Render Logic class.
    /// </summary>
    public static class RenderLogic
    {
        /// <summary>
        /// Draws a zone.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <param name="renderer">The renderer.</param>
        public static void DrawZone(Zone zone, AbstractRenderer renderer)
        {
            FillAARectangle(zone, renderer);
            
            Color textColor = Color.FromArgb(255, zone.Color);
            renderer.DrawText(zone.Name, zone.TopLeft, textColor);
        }

        /// <summary>
        /// Draws the world object.
        /// </summary>
        /// <param name="wo">The wo.</param>
        /// <param name="uiSettings">The UI settings.</param>
        /// <param name="auiSettings">The aui settings.</param>
        /// <param name="renderer">The renderer.</param>
        public static void DrawWorldObject(WorldObject wo, LayerUISettings uiSettings, AgentUISettings auiSettings, AbstractRenderer renderer)
        {
            if(uiSettings.ShowObjects)
            {
                if(wo is Agent ag)
                {
                    DrawAgent(ag, uiSettings, auiSettings, renderer);
                }
                else
                {
                    DrawShape(wo.Shape, uiSettings.ShowBoundingBoxes, renderer, true);
                }
            }
        }

        /// <summary>
        /// Draws the agent shadow.
        /// </summary>
        /// <param name="shadow">The shadow.</param>
        /// <param name="uiSettings">The UI settings.</param>
        /// <param name="auiSettings">The aui settings.</param>
        /// <param name="renderer">The renderer.</param>
        public static void DrawAgentShadow(AgentShadow shadow, LayerUISettings uiSettings, AgentUISettings auiSettings, AbstractRenderer renderer)
        {
            Color orig = shadow.Shape.DebugColor;
            shadow.Shape.DebugColor = Color.White;
            DrawShape(shadow.Shape, uiSettings.ShowBoundingBoxes, renderer, true);
            shadow.Shape.DebugColor = orig;
            //Draw Orientation
            DrawOrientation(renderer, shadow.Shape);

            if(auiSettings.ShowSenses)
            {
                foreach(IShape shape in shadow.SenseShapes)
                {
                    DrawShape(shape, auiSettings.ShowSenseBoundingBoxes, renderer, false);
                }
            }
        }

        /// <summary>
        /// Draws the inactive object.
        /// </summary>
        /// <param name="wo">The wo.</param>
        /// <param name="ui">The UI.</param>
        /// <param name="renderer">The renderer.</param>
        public static void DrawInactiveObject(WorldObject wo, LayerUISettings uiSettings, AbstractRenderer renderer)
        {
            Color color = wo.Shape.Color;
            wo.Shape.Color = Color.FromArgb(50, color.R, color.G, color.B);
            DrawShape(wo.Shape, uiSettings.ShowBoundingBoxes, renderer, true);
            wo.Shape.Color = Color.FromArgb(50, 255, 0, 0);
            DrawShape(wo.Shape, uiSettings.ShowBoundingBoxes, renderer, false);
            wo.Shape.Color = color;
        }

        /// <summary>
        /// Draws the past object.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <param name="uiSettings">The UI settings.</param>
        /// <param name="renderer">The renderer.</param>
        public static void DrawPastObject(IShape shape, LayerUISettings uiSettings, AbstractRenderer renderer)
        {
            Color pastDebug = shape.DebugColor;
            shape.DebugColor = Color.White;
            DrawShape(shape, uiSettings.ShowBoundingBoxes, renderer, true);
            shape.DebugColor = pastDebug;
        }

        /// <summary>
        /// Draws the ancestry.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public static void DrawAncestry(AbstractRenderer renderer)
        {
            for(int index = 0; index < Planet.World.StableActiveObjects.Count; index++)
            {
                WorldObject wo = Planet.World.StableActiveObjects[index];
                if(wo is Agent ag && ag.Alive)
                {
                    while(ag.LivingAncestor != null && !ag.LivingAncestor.Alive)
                    {
                        ag.LivingAncestor = ag.LivingAncestor.LivingAncestor;
                    }

                    if(ag.LivingAncestor == null)
                    {
                        Circle c = (Circle)ag.Shape;
                        renderer.DrawCircle(ag.Shape.CentrePoint, c.Radius + 3, Color.Blue);
                    }

                    else
                    {

                        renderer.DrawAgentAncestry(ag);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a past state.
        /// </summary>
        /// <param name="uiSettings">The UI settings.</param>
        /// <param name="auiSettings">The aui settings.</param>
        /// <param name="renderer">The renderer.</param>
        /// <param name="compnumber">The compnumber.</param>
        public static void DrawPastState(LayerUISettings uiSettings, AgentUISettings auiSettings, AbstractRenderer renderer, int compnumber)
        {
            foreach(WorldObject wo in Planet.World.CollisionLevels[uiSettings.LayerName].EnumerateItems())
            {
                if(compnumber < wo.ExecutionOrder)
                {
                    RenderLogic.DrawPastObject(wo.Shape, uiSettings, renderer);
                    continue;
                }

                if(!(wo is Agent))
                {
                    //TODO: Should ALL objects have shadows??
                    RenderLogic.DrawPastObject(wo.Shape, uiSettings, renderer);
                    continue;
                }

                Agent ag = (Agent)wo;

                if(compnumber == wo.ExecutionOrder)
                {
                    RenderLogic.DrawAgentShadow(ag.Shadow, uiSettings, auiSettings, renderer);
                    continue;
                }

                RenderLogic.DrawPastObject(ag.Shadow.Shape, uiSettings, renderer);
            }
        }

        /// <summary>
        /// Draws the AA rectangle.
        /// </summary>
        /// <param name="rec">The rectangle.</param>
        /// <param name="renderer">The renderer.</param>
        public static void DrawAARectangle(AARectangle rec, AbstractRenderer renderer)
        {
            Point tl = rec.TopLeft;
            renderer.DrawAARectangle(tl, new Point(tl.X + rec.XWidth, tl.Y + rec.YHeight), rec.Color, 0.4f);
        }

        /// <summary>
        /// Fills an AA rectangle.
        /// </summary>
        /// <param name="rec">The rectangle.</param>
        /// <param name="renderer">The renderer.</param>
        public static void FillAARectangle(AARectangle rec, AbstractRenderer renderer)
        {
            Point tl = rec.TopLeft;
            renderer.FillAARectangle(tl, new Point(tl.X + rec.XWidth, tl.Y + rec.YHeight), rec.Color);
        }

        /// <summary>
        /// Draws the bounding box.
        /// </summary>
        /// <param name="bb">The bb.</param>
        /// <param name="color">The color.</param>
        /// <param name="renderer">The renderer.</param>
        private static void DrawBoundingBox(BoundingBox bb, Color color, AbstractRenderer renderer)
        {
            renderer.DrawAARectangle(new Point(bb.MaxX, bb.MaxY), new Point(bb.MinX, bb.MinY), color, 0.4f);
        }

        /// <summary>
        /// Draws a line to indicate an orientation
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="shape">The shape.</param>
        private static void DrawOrientation(AbstractRenderer renderer, IShape shape)
        {
            renderer.DrawOrientation(shape);
        }

        /// <summary>
        /// Draws an agent.
        /// </summary>
        /// <param name="ag">The ag.</param>
        /// <param name="uiSettings">The UI settings.</param>
        /// <param name="auiSettings">The aui settings.</param>
        /// <param name="renderer">The renderer.</param>
        /// <returns></returns>
        private static void DrawAgent(Agent ag, LayerUISettings uiSettings, AgentUISettings auiSettings, AbstractRenderer renderer)
        {
            IShape shape = ag.Shape;

            DrawShape(shape, uiSettings.ShowBoundingBoxes, renderer, true);

            if(auiSettings.ShowSenses)
            {
                foreach(IHasShape iHS in ag.Senses)
                {
                    DrawShape(iHS.Shape, auiSettings.ShowSenseBoundingBoxes, renderer, false);
                }
            }

            if(ag.JustReproduced)
            {
                //TODO: this just assumed the agent is a circle which will eventually not always be true.
                Circle c = ag.Shape as Circle;
                renderer.DrawCircle(c.CentrePoint, c.Radius + 2, Color.HotPink);
            }

            //Draw Orientation
            DrawOrientation(renderer, shape);
        }

        /// <summary>
        /// Draws the shape.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <param name="showBoundingBox">if set to <c>true</c> [show bounding box].</param>
        /// <param name="renderer">The renderer.</param>
        /// <param name="fillIn">if set to <c>true</c> [fill in].</param>
        /// <exception cref="NotImplementedException">What the heck shape is this?</exception>
        private static void DrawShape(IShape shape, bool showBoundingBox, AbstractRenderer renderer, bool fillIn)
        {
            switch(shape)
            {
                case Circle cir: DrawCircle(cir, fillIn, renderer); break;
                case Sector sec: renderer.DrawSector(sec, fillIn); break;
                case Rectangle rect: renderer.DrawRectangleWithFillIn(rect, fillIn); break;
                case AARectangle aar: DrawAARectangle(aar, renderer); break;
                default: throw new NotImplementedException("What the heck shape is this?");
            }
            if(showBoundingBox)
            {
                DrawBoundingBox(shape.BoundingBox, Color.Black, renderer);
            }
        }

        private static void DrawCircle(Circle wo, bool fillIn, AbstractRenderer renderer)
        {
            //World Object Body
            if(fillIn)
            {
                renderer.FillCircle(wo.CentrePoint, wo.Radius, wo.Color);
            }
            else
            {
                renderer.DrawCircle(wo.CentrePoint, wo.Radius, wo.Color);
            }

            //Core of the body is the debug colour
            renderer.FillCircle(wo.CentrePoint, 2, wo.DebugColor);

            DrawOrientation(renderer, wo);
        }
    }
}