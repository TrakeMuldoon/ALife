using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class AARectangle : Rectangle
    {
        public override Point CentrePoint
        {
            get
            {
                double cpX = TopLeft.X + Width / 2;
                double cpY = TopLeft.Y + Length / 2;
                return new Point(cpX, cpY);
            }
        }

        private Angle ori = new Angle(0);

        public AARectangle() : base(3,4, new Point(0,0), Colors.Red)
        {
        }

        public override Angle Orientation
        {
            get { return ori; }
        }
        public override BoundingBox BoundingBox
        {
            get { return new BoundingBox(TopLeft.X, TopLeft.Y, TopLeft.X + Width, TopLeft.Y + Length); }
        }

        public override ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.AARectangle;
        }

        public override void Reset()
        {
            //This does nothing for AARectangles
        }
    }
}
