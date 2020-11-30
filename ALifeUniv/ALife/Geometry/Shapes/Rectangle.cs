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
            Point topLeft = ExtraMath.TranslateByVector(tempPoint, Orientation.Radians - (Math.PI / 2), RLWidth / 2);

            Point topRight = ExtraMath.TranslateByVector(topLeft, Orientation.Radians + (Math.PI / 2), RLWidth);
            Point bottomRight = ExtraMath.TranslateByVector(topRight, Orientation.Radians + Math.PI, FBLength);
            Point bottomLeft = ExtraMath.TranslateByVector(bottomRight, Orientation.Radians + (Math.PI * 3 / 2), RLWidth);

            double maxX = ExtraMath.MultiMax(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            double minX = ExtraMath.MultiMin(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            double maxY = ExtraMath.MultiMax(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
            double minY = ExtraMath.MultiMin(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

            BoundingBox bb = new BoundingBox(minX, minY, maxX, maxY);
            myBox = bb;
            return bb;
        }

        public IShape Clone()
        {
            Rectangle rec =  new Rectangle(CentrePoint, FBLength, RLWidth, Color);
            rec.Orientation = Orientation;
            return rec;
        }
    }
}
