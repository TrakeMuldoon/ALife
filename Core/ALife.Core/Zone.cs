﻿using ALife.Core.Distributors;
using ALife.Core.Geometry.Shapes;
using System;
using System.Diagnostics;
using System.Drawing;

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

        public Zone(String name, String distributorType, Color color
                    , Geometry.Shapes.Point topLeft, double xWidth, double yHeight) : base(topLeft, xWidth, yHeight, color)
        {
            Color lowAlpha = Color.FromArgb(50, Color);
            Color = lowAlpha;

            Name = name;

            //Distributor type is currently unused but will be used later I guess?
            Distributor = new RandomObjectDistributor(this, true, ReferenceValues.CollisionLevelPhysical);
        }
    }
}
