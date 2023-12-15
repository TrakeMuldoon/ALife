using Newtonsoft.Json;
using Windows.Foundation;
using Windows.UI;
using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;

namespace ALife.Core.Geometry.Shapes
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
