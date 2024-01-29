using ALife.Core.Distributors;
using ALife.Core.Utility.Colours;
using System;
using System.Diagnostics;
using ALife.Core.GeometryOld.Shapes;

namespace ALife.Core
{
    [DebuggerDisplay("Zone: {Name}")]
    public class Zone : AARectangle, IHasShape
    {
        public String Name
        {
            get;
            private set;
        }

        public IShape Shape
        {
            get
            {
                return this;
            }
        }

        public Zone OppositeZone;
        public double OrientationDegrees;

        public readonly WorldObjectDistributor Distributor;

        public Zone(String name, String distributorType, Colour color
                    , ALife.Core.GeometryOld.Shapes.Point topLeft, double xWidth, double yHeight) : base(topLeft, xWidth, yHeight, color)
        {
            Colour lowAlpha = new Colour(Colour)
            {
                A = 50,
            };
            Colour = lowAlpha;

            Name = name;

            //Distributor type is currently unused but will be used later I guess?
            Distributor = new RandomObjectDistributor(this, true, ReferenceValues.CollisionLevelPhysical);
        }
    }
}
