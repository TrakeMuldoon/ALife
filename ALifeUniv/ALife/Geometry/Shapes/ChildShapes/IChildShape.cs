using ALifeUni.ALife.Utility;

namespace ALifeUni.ALife.Shapes
{
    public interface IChildShape
    {
        Angle RelativeOrientation { get; set; }
        Angle AbsoluteOrientation { get; set; }

        IShape CloneChildShape(IShape parent);
    }
}
