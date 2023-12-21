using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;

namespace ALife.Core.Geometry.Shapes.ChildShapes
{
    public interface IChildShape
    {
        Angle RelativeOrientation { get; set; }
        Angle AbsoluteOrientation { get; set; }

        IShape CloneChildShape(IShape parent);
    }
}
