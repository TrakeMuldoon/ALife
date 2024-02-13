using ALife.Core.Geometry.OLD;
using ALife.Core.Geometry.OLD.Shapes;

namespace ALife.Core.Geometry.OLD.Shapes.ChildShapes
{
    public interface IChildShape
    {
        Angle RelativeOrientation { get; set; }
        Angle AbsoluteOrientation { get; set; }

        IShape CloneChildShape(IShape parent);
    }
}
