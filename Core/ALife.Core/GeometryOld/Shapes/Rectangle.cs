using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;
using System;
using ALife.Core.GeometryOld;
using ALife.Core.GeometryOld.Shapes;

namespace ALife.Core.GeometryOld.Shapes
{
    public class Rectangle : IShape
    {
        public double FBLength;
        public double RLWidth;

        public Rectangle(Point centrePoint, double fbLength, double rlWidth, Colour color)
        {
            CentrePoint = centrePoint;
            FBLength = fbLength;
            RLWidth = rlWidth;
            Colour = color;
            DebugColour = Colour.IndianRed;

            Orientation = new Angle(0);
        }

        internal Rectangle(double fbLength, double rlWidth, Colour color)
        {
            FBLength = fbLength;
            RLWidth = rlWidth;
            Colour = color;
            DebugColour = Colour.IndianRed;

            Orientation = new Angle(0);
        }

        public virtual Point CentrePoint
        {
            get;
            set;
        }

        public virtual Angle Orientation
        {
            get;
            set;
        }

        public virtual BoundingBox BoundingBox
        {
            get
            {
                return GetBoundingBox();
            }
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

        public virtual ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.Rectangle;
        }

        public virtual void Reset()
        {
            myBox = null;
        }

        private BoundingBox? myBox;

        private Point topLeft, topRight, bottomLeft, bottomRight;
        public Point TopLeft
        {
            get
            {
                if(myBox == null)
                {
                    GetBoundingBox();
                }
                return topLeft;
            }
            private set { topLeft = value; }
        }
        public Point TopRight
        {
            get
            {
                if(myBox == null)
                {
                    GetBoundingBox();
                }
                return topRight;
            }
            private set { topRight = value; }
        }
        public Point BottomRight
        {
            get
            {
                if(myBox == null)
                {
                    GetBoundingBox();
                }
                return bottomRight;
            }
            private set { bottomRight = value; }
        }
        public Point BottomLeft
        {
            get
            {
                if(myBox == null)
                {
                    GetBoundingBox();
                }
                return bottomLeft;
            }
            private set { bottomLeft = value; }
        }

        private BoundingBox GetBoundingBox()
        {
            if(myBox != null)
            {
                try
                {
                    return myBox.Value;
                }
                catch(InvalidOperationException)
                {
                    //swallow, and just build the bb
                }
            }

            Point tempPoint = CentrePoint;
            tempPoint = GeometryMath.TranslateByVector(tempPoint, Orientation, FBLength / 2);
            topLeft = GeometryMath.TranslateByVector(tempPoint, Orientation.Radians - (Math.PI / 2), RLWidth / 2);
            topRight = GeometryMath.TranslateByVector(topLeft, Orientation.Radians + (Math.PI / 2), RLWidth);
            bottomRight = GeometryMath.TranslateByVector(topRight, Orientation.Radians + Math.PI, FBLength);
            bottomLeft = GeometryMath.TranslateByVector(bottomRight, Orientation.Radians + (Math.PI * 3 / 2), RLWidth);

            double maxX = ExtraMath.Maximum(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            double minX = ExtraMath.Minimum(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            double maxY = ExtraMath.Maximum(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
            double minY = ExtraMath.Minimum(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

            BoundingBox bb = new BoundingBox(minX, minY, maxX, maxY);
            myBox = bb;
            return bb;
        }

        public virtual IShape CloneShape()
        {
            Point cp = new Point(CentrePoint.X, CentrePoint.Y);
            Rectangle rec = new Rectangle(cp, FBLength, RLWidth, (Colour)Colour.Clone());
            rec.Orientation = Orientation.Clone();
            return rec;
        }
    }
}
