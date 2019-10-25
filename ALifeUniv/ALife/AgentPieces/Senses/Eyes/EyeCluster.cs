using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALifeUni.ALife.Inputs.SenseClusters;
using ALifeUni.ALife.UtilityClasses;

namespace ALifeUni.ALife
{
    public class EyeCluster : SenseCluster
    {
        ChildSector myShape;

        public EyeCluster(Agent parent, String name) : base(parent, name)
        {
            myShape = new ChildSector(new Angle(0), new Angle(0), parent);
            SubInputs.Add(new EyeInput(name + ".Eye"));
            SubInputs.Add(new RedInput(name + ".Red"));
        }

        public override IShape GetShape()
        {
            return myShape;
        }
    }
}
