using System.Drawing;

namespace ALife.Core.Geometry.Shapes
{
    public interface IShape
    {
        Geometry.Shapes.Point CentrePoint
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
