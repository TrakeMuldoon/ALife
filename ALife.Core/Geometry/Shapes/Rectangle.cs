using ALife.Core.Utility;
using System.Drawing;

namespace ALife.Core.Geometry.Shapes
{
    public class Rectangle : IShape
    {
        public double FBLength;
        public double RLWidth;

        public Rectangle(Geometry.Shapes.Point centrePoint, double fbLength, double rlWidth, Color color)
        {
            CentrePoint = centrePoint;
            FBLength = fbLength;
            RLWidth = rlWidth;
            Color = color;
            DebugColor = System.Drawing.Color.IndianRed;

            Orientation = new Angle(0);
        }

        internal Rectangle(double fbLength, double rlWidth, Color color)
        {
            FBLength = fbLength;
            RLWidth = rlWidth;
            Color = color;
            DebugColor = System.Drawing.Color.IndianRed;

            Orientation = new Angle(0);
        }

        public virtual Geometry.Shapes.Point CentrePoint
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

        private Geometry.Shapes.Point topLeft, topRight, bottomLeft, bottomRight;
        public Geometry.Shapes.Point TopLeft
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
        public Geometry.Shapes.Point TopRight
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
        public Geometry.Shapes.Point BottomRight
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
        public Geometry.Shapes.Point BottomLeft
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

            Geometry.Shapes.Point tempPoint = CentrePoint;
            tempPoint = ExtraMath.TranslateByVector(tempPoint, Orientation, FBLength / 2);
            topLeft = ExtraMath.TranslateByVector(tempPoint, Orientation.Radians - (Math.PI / 2), RLWidth / 2);
            topRight = ExtraMath.TranslateByVector(topLeft, Orientation.Radians + (Math.PI / 2), RLWidth);
            bottomRight = ExtraMath.TranslateByVector(topRight, Orientation.Radians + Math.PI, FBLength);
            bottomLeft = ExtraMath.TranslateByVector(bottomRight, Orientation.Radians + (Math.PI * 3 / 2), RLWidth);

            double maxX = ExtraMath.MultiMax(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            double minX = ExtraMath.MultiMin(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            double maxY = ExtraMath.MultiMax(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
            double minY = ExtraMath.MultiMin(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

            BoundingBox bb = new BoundingBox(minX, minY, maxX, maxY);
            myBox = bb;
            return bb;
        }

        public virtual IShape CloneShape()
        {
            Geometry.Shapes.Point cp = new Geometry.Shapes.Point(CentrePoint.X, CentrePoint.Y);
            Rectangle rec = new Rectangle(cp, FBLength, RLWidth, Color.Clone());
            rec.Orientation = Orientation.Clone();
            return rec;
        }
    }
}
