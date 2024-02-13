using ALife.Core.Geometry.OLD;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Geometry.OLD.Shapes
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

        Colour Colour
        {
            get;
            set;
        }

        Colour DebugColour
        {
            get;
            set;
        }

        ShapesEnum GetShapeEnum();

        void Reset();
        IShape CloneShape();
    }
}
