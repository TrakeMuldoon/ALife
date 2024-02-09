using ALife.Core.CollisionDetection.Shapes;

namespace ALife.Core.CollisionDetection.Geometry
{
    /// <summary>
    /// Defines that an object has a Shape available to detect collisions with
    /// </summary>
    public interface IHasShape
    {
        Shape Shape { get; }
    }
}
