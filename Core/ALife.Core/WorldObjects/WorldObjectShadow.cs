using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;

namespace ALife.Core.WorldObjects
{
    public class WorldObjectShadow : IHasShape
    {
        protected IShape shape;

        public WorldObjectShadow(WorldObject self)
        {
            shape = self.Shape.CloneShape();
            shape.DebugColour = Colour.Yellow;
            shape.Orientation = self.Shape.Orientation.Clone();
        }

        public IShape Shape => shape;
    }
}
