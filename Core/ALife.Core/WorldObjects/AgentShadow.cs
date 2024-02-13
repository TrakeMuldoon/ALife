using ALife.Core.Geometry.OLD.Shapes;
using ALife.Core.Geometry.OLD.Shapes.ChildShapes;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Senses;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects
{
    public class AgentShadow : IHasShape
    {
        public readonly List<IShape> SenseShapes = new List<IShape>();

        public AgentShadow(Agent self)
        {
            shape = self.Shape.CloneShape();
            shape.DebugColour = Colour.Yellow;
            shape.Orientation = self.Shape.Orientation.Clone();
            foreach(SenseCluster sc in self.Senses)
            {
                IChildShape cs = sc.Shape as IChildShape;

                IShape clone = cs.CloneChildShape(shape);
                clone.Colour = Colour.White;
                SenseShapes.Add(clone);
            }
        }
        private IShape shape;
        public IShape Shape
        {
            get { return shape; }
        }
    }
}
