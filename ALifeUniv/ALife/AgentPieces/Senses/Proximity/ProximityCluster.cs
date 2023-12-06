using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using Windows.UI;

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

        [Obsolete("ProximityDefault is deprecated, please use ProximityCluster with EvoNumbers instead.")]
        public ProximityCluster(WorldObject parent, string name)
            : this(parent, name, new ROEvoNumber(30, 2, 5, 50))
        {
        }

        public ProximityCluster(WorldObject parent, string name, EvoNumber radius)
            : base(parent, name)
        {
            evoRadius = radius;

            myShape = new ChildCircle(parent.Shape, new Angle(0), 0, (float)radius.StartValue);

            SubInputs.Add(new AnyInput(name + ".SomethingClose"));
        }
        public ProximityCluster(WorldObject parent, string name, EvoNumber radius, Color newColor)
            : this(parent, name, radius)
        {
            myShape.Color = newColor;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, evoRadius.Evolve(), myShape.Color.Clone());
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, evoRadius.Evolve(), myShape.Color.Clone());
        }
    }
}
