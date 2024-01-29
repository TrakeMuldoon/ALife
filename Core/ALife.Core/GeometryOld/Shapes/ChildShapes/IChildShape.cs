using ALife.Core.GeometryOld;
using ALife.Core.GeometryOld.Shapes;

namespace ALife.Core.GeometryOld.Shapes.ChildShapes
{
    public interface IChildShape
    {
        Angle RelativeOrientation { get; set; }
        Angle AbsoluteOrientation { get; set; }

        IShape CloneChildShape(IShape parent);
    }
}
