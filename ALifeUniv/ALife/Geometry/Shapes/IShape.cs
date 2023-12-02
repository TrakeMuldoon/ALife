using ALifeUni.ALife.Geometry;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Shapes
{
    public interface IShape
    {
        Point CentrePoint
        {
            get;
            set;
        }
        Angle Orientation
        {
            get;
            set;
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
        IShape CloneShape();
    }
}
