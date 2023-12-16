using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Senses;

namespace ALife.Core.WorldObjects
{
    public class AgentShadow : IHasShape
    {
        public readonly List<IShape> SenseShapes = new List<IShape>();

        public AgentShadow(Agent self)
        {
            shape = self.Shape.CloneShape();
            shape.DebugColor = System.Drawing.Color.Yellow;
            shape.Orientation = self.Shape.Orientation.Clone();
            foreach(SenseCluster sc in self.Senses)
            {
                IChildShape cs = sc.Shape as IChildShape;

                IShape clone = cs.CloneChildShape(shape);
                clone.Color = System.Drawing.Color.White;
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
