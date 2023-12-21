using ALife.Core.Geometry.Shapes;
namespace ALife.Core.Geometry.Shapes
{
    public interface IHasShape
    {
        IShape Shape
        {
            get;
        }
    }
}
