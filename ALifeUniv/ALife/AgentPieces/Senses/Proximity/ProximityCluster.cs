using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using System;

namespace ALifeUni.ALife
{
    public class ProximityCluster : SenseCluster
    {
        private EvoNumber evoRadius;
        private ChildCircle myShape;
        public override IShape Shape
        {
            get
            {
                return myShape;
            }
        }

        [Obsolete("EyeClusterDefault is deprecated, please use EyeCluster with EvoNumbers instead.")]
        public ProximityCluster(WorldObject parent, string name) : this(parent, name, new ROEvoNumber(30, 2, 6, 40, 5, 50, 1))
        {
        }

        public ProximityCluster(WorldObject parent, string name, EvoNumber radius) : base(parent, name)
        {
            evoRadius = radius;

            myShape = new ChildCircle(parent.Shape, new Angle(0), 0, (float)radius.StartValue);

            SubInputs.Add(new AnyInput(name + ".SomethingClose"));
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, evoRadius.Evolve());
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, evoRadius.Evolve());
        }
    }
}
