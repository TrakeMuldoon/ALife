using ALifeUni.ALife.Agents;
using ALifeUni.ALife.Agents.Senses;
using ALifeUni.ALife.Shapes;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife.Utility.WorldObjects
{
    public class AgentShadow : IHasShape
    {
        public readonly List<IShape> SenseShapes = new List<IShape>();

        public AgentShadow(Agent self)
        {
            shape = self.Shape.CloneShape();
            shape.DebugColor = Colors.Yellow;
            shape.Orientation = self.Shape.Orientation.Clone();
            foreach(SenseCluster sc in self.Senses)
            {
                IChildShape cs = sc.Shape as IChildShape;

                IShape clone = cs.CloneChildShape(shape);
                clone.Color = Colors.White;
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
