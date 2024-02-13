using ALife.Core.Utility.Colours;
using ALife.Core.NewGeometry.OLD;

namespace ALife.Core.NewGeometry.OLD.Shapes
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
