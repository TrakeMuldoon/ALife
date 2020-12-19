using ALifeUni.ALife.Utility;
using System;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Rectangle : IShape
    {
        public double FBLength;
        public double RLWidth;

        public Rectangle(Point centrePoint, double fbLength, double rlWidth, Color color)
        {
            CentrePoint = centrePoint;
            FBLength = fbLength;
            RLWidth = rlWidth;
            Color = color;
            DebugColor = Colors.IndianRed;

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

        public Color Color
        {
            get;
            set;
        }

        public Color DebugColor
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
            tempPoint = ExtraMath.TranslateByVector(tempPoint, Orientation.Radians, FBLength / 2);
            TopLeft = ExtraMath.TranslateByVector(tempPoint, Orientation.Radians - (Math.PI / 2), RLWidth / 2);
            TopRight = ExtraMath.TranslateByVector(TopLeft, Orientation.Radians + (Math.PI / 2), RLWidth);
            BottomRight = ExtraMath.TranslateByVector(TopRight, Orientation.Radians + Math.PI, FBLength);
            BottomLeft = ExtraMath.TranslateByVector(BottomRight, Orientation.Radians + (Math.PI * 3 / 2), RLWidth);

            double maxX = ExtraMath.MultiMax(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);
            double minX = ExtraMath.MultiMin(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);
            double maxY = ExtraMath.MultiMax(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);
            double minY = ExtraMath.MultiMin(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);

            BoundingBox bb = new BoundingBox(minX, minY, maxX, maxY);
            myBox = bb;
            return bb;
        }

        public IShape CloneShape()
        {
            Point cp = new Point(CentrePoint.X, CentrePoint.Y);
            Rectangle rec = new Rectangle(cp, FBLength, RLWidth, Color.Clone());
            rec.Orientation = Orientation.Clone();
            return rec;
        }
    }
}
