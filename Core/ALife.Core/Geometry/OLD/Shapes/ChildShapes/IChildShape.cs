using ALife.Core.NewGeometry.OLD;
using ALife.Core.NewGeometry.OLD.Shapes;

namespace ALife.Core.NewGeometry.OLD.Shapes.ChildShapes
{
    public interface IChildShape
    {
        Angle RelativeOrientation { get; set; }
        Angle AbsoluteOrientation { get; set; }

        IShape CloneChildShape(IShape parent);
    }
}
