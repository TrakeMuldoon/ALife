using System.Collections.Generic;
using ALife.Core.GeometryOld.Shapes;

namespace ALife.Core.Collision
{
    public interface ICollisionMap<T> where T : IHasShape
    {
        /// <summary>
        /// Query for collisions will examine the map and everything in it, and return a list of objects whose bounding boxes collide with the bounding box of the input
        /// </summary>
        /// <param name="queryObject">Query the collision map for this object</param>
        /// <returns></returns>
        List<T> QueryForBoundingBoxCollisions(T queryObject);
        List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox);
        List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox, T self);

        List<T> DetectCollisions(T self);
        List<T> DetectCollisions(IHasShape detector, T self);

        bool Insert(T newObject);

        void RemoveObject(T killMe);

        void MoveObject(T moveMe);

        IEnumerable<T> EnumerateItems();

    }
}
