using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents.Senses.GenericInputs;
using System;
using ALife.Core.GeometryOld;
using ALife.Core.GeometryOld.Shapes.ChildShapes;
using ALife.Core.NewGeometry.OLD.Shapes;

namespace ALife.Core.WorldObjects.Agents.Senses
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

        public SquareSenseCluster(WorldObject parent, string name, double FBLength, double RLWidth)
            : base(parent, name)
        {
            myShape = new ChildRectangle(parent.Shape, new Angle(45), 5.0, FBLength, RLWidth);

            SubInputs.Add(new AnyInput(name + ".SomethingClose"));
            SubInputs.Add(new CountInput(name + ".HowMany"));
        }

        //TODO: Implement SquareSenseCluster with EvoNumbers... whoops
        [Obsolete("SquareSenseClusterDefault is deprecated, please use SquareSenseCluster with EvoNumbers instead.")]
        public SquareSenseCluster(WorldObject parent, string name)
            : this(parent, name, 80, 30)
        {
        }

        public SquareSenseCluster(WorldObject parent, string name, double FBLength, double RLWidth, Colour myColor)
            : this(parent, name, FBLength, RLWidth)
        {
            myShape.Colour = myColor;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new SquareSenseCluster(newParent, Name, myShape.FBLength, myShape.RLWidth, (Colour)myShape.Colour.Clone());
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            return new SquareSenseCluster(newParent, Name, myShape.FBLength, myShape.RLWidth, (Colour)myShape.Colour.Clone());
        }
    }
}
