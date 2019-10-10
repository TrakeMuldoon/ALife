using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALifeUni.ALife.UtilityClasses;

namespace ALifeUni.ALife
{
    public class EyeCluster : SenseCluster
    {
        ChildSector myShape;

        public EyeCluster(Agent Parent) : base(Parent)
        {
            myShape = new ChildSector(new Angle(0), new Angle(0), Parent);
        }

        public override IShape GetShape()
        {
            return myShape;
        }
    }
}
