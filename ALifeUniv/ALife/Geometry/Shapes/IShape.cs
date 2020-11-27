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
        Color DebugColor
        {
            get;
            set;
        }

        ShapesEnum GetShapeEnum();

        void Reset();
    }
}
