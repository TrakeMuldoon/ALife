using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public class ProximityCluster : SenseCluster
    {
        private ChildCircle myShape;
        public override IShape Shape
        {
            get
            {
                return myShape;
            }
        }

        public ProximityCluster(WorldObject parent, string name) : this(parent, name, 8) //TODO: Hardcoded proximity length
        {
        }

        public ProximityCluster(WorldObject parent, string name, double radius) : base(parent, name)
        {
            myShape = new ChildCircle(parent.Shape, new Angle(0), 0, (float)radius);

            SubInputs.Add(new ProximityInput(name + ".SomethingClose"));
        }


        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, myShape.Radius);
        }
    }
}
