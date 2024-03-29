﻿using ALife.Core.Utility.Colours;
using System;

namespace ALife.Core.Geometry.Shapes
{
    public class AARectangle : IShape
    {
        public Point CentrePoint
        {
            get
            {
                double cpX = TopLeft.X + XWidth / 2;
                double cpY = TopLeft.Y + YHeight / 2;
                return new Point(cpX, cpY);
            }
            set
            {
                throw new InvalidOperationException("You cannot set the centrepoint of an AARectangle");
            }
        }

        private Angle ori = new Angle(0);

        public AARectangle(Point topLeft, double xWidth, double yHeight, Colour color)
        {
            XWidth = xWidth;
            YHeight = yHeight;
            Colour = color;
            TopLeft = topLeft;
        }

        public double XWidth
        {
            get;
            set;
        }
        public double YHeight
        {
            get;
            set;
        }

        public Point TopLeft
        {
            get;
            set;
        }

        public Angle Orientation
        {
            get { return ori; }
            set
            {
                throw new InvalidOperationException("Orientation cannot be set for AARectangle");
            }
        }
        public BoundingBox BoundingBox
        {
            get { return new BoundingBox(TopLeft.X, TopLeft.Y, TopLeft.X + XWidth, TopLeft.Y + YHeight); }
        }

        public Colour Colour
        {
            get;
            set;
        }

        public Colour DebugColour
        {
            get;
            set;
        }

        public ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.AARectangle;
        }

        public void Reset()
        {
            //This does nothing for AARectangles
        }

        public IShape CloneShape()
        {
            return new AARectangle(TopLeft, XWidth, YHeight, Colour);
        }
    }
}
