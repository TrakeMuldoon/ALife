using ALifeUni.ALife.UtilityClasses;

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

        public ProximityCluster(WorldObject parent, string name) : this(parent, name, 30) //TODO: Hardcoded proximity length
        {
        }

        public ProximityCluster(WorldObject parent, string name, double radius) : base(parent, name)
        {
            myShape = new ChildCircle(parent.Shape, new Angle(0), 0, (float)radius);

            SubInputs.Add(new AnyInput(name + ".SomethingClose"));
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, myShape.Radius);
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, myShape.Radius);
        }
    }
}
