using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Senses;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects
{
    public class AgentShadow : WorldObjectShadow
    {
        public readonly List<IShape> SenseShapes = new List<IShape>();

        public AgentShadow(Agent self) : base(self)
        {
            foreach(SenseCluster sc in self.Senses)
            {
                IChildShape cs = sc.Shape as IChildShape;

                IShape clone = cs.CloneChildShape(shape);
                clone.Colour = Colour.White;
                SenseShapes.Add(clone);
            }
        }
    }
}
