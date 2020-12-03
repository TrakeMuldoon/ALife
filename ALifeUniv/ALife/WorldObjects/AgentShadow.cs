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
                
                IShape clone = sc.Shape.CloneShape();
                if(clone is ChildSector cs)
                {
                    cs.Parent = this;
                }
                SenseShapes.Add(clone);
            }
        }
    }
}
