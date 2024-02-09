using ALife.Core.CollisionDetection.Geometry;

namespace ALife.Core.CollisionDetection.CollisionGrids
{
    public struct Element
    {
        public Point Center;
        public Point HalfSize;
        public int Id;
        public int Next;
    }
}
