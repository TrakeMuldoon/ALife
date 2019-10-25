using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public interface IShape
    {
        Point CentrePoint
        {
            get;
        }
        Angle Orientation
        {
            get;
        }

        BoundingBox BoundingBox
        {
            get;
        }

        Color Color
        {
            get;
            set;
        }
        ShapesEnum GetShapeEnum();

        void Reset();
    }
}
