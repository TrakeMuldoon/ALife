using ALifeUni.ALife.UtilityClasses;

namespace ALifeUni.ALife
{
    class SquareSenseCluster : SenseCluster
    {
        private ChildRectangle myShape;
        public override IShape Shape
        {
            get
            {
                return myShape;
            }
        }

        public SquareSenseCluster(WorldObject parent, string name) : this(parent, name, 80, 30) //TODO: Hardcoded proximity length
        {
        }

        public SquareSenseCluster(WorldObject parent, string name, double FBLength, double RLWidth) : base(parent, name)
        {
            myShape = new ChildRectangle(parent.Shape, new Angle(45), 5.0, FBLength, RLWidth);

            SubInputs.Add(new AnyInput(name + ".SomethingClose"));
            SubInputs.Add(new CountInput(name + ".HowMany"));
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new SquareSenseCluster(newParent, Name, myShape.FBLength, myShape.RLWidth);
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            return new SquareSenseCluster(newParent, Name, myShape.FBLength, myShape.RLWidth);
        }
    }
}
