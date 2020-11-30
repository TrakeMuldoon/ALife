using ALifeUni.ALife.UtilityClasses;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class AgentShadow : Circle
    {
        public readonly List<IShape> SenseShapes = new List<IShape>();

        public AgentShadow(Agent self) : base(self.CentrePoint, self.Radius)
        {
            Color = self.Color;
            DebugColor = Colors.Yellow;
            Orientation = new Angle(self.Orientation.Degrees);
            foreach(SenseCluster sc in self.Senses)
            {
                IShape shape = sc.GetShape();
                IShape clone = shape.CloneShape();
                if(shape is ChildSector)
                {
                    ((ChildSector)clone).Parent = this;
                }
                SenseShapes.Add(clone);
            }
        }
    }
}
