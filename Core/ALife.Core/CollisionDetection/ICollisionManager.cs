using System.Collections.Generic;
using ALife.Core.Shapes;

namespace ALife.Core.CollisionDetection
{
    public interface ICollisionManager<T> where T : IHasShape
    {
        /****** Interface Defintion *******
                List<T> QueryForBoundingBoxCollisions(T queryObject);
                List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox);
                List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox, T self);

                List<T> DetectCollisions(T self);
                List<T> DetectCollisions(IHasShape detector, T self);

                bool Insert(T newObject);
                void RemoveObject(T killMe);
                void MoveObject(T moveMe);

                IEnumerable<T> EnumerateItems();
        */

        /// <summary>
        /// Query for collisions between the object and any other object on the collision grid.
        /// </summary>
        /// <param name="self">The object to detect collisions against</param>
        /// <returns>A list of objects which have collisions</returns>
        List<T> DetectCollisions(T self);

        /// <summary>
        /// Query for collisions between the detector shape, and any other object on the collision grid.
        /// </summary>
        /// <param name="detector">The shape to detect collisions against</param>
        /// <param name="self">The parent object, to be excluded from collision results</param>
        /// <returns>A list of objects which have collisions</returns>
        List<T> DetectCollisions(IHasShape detector, T self);

        /// <summary>
        /// Enumerate the objects currently on the collision grid.
        /// </summary>
        /// <returns>IEnumerable of items on the grid</returns>
        IEnumerable<T> EnumerateItems();

        /// <summary>
        /// Add a new object onto the collision grid
        /// </summary>
        /// <param name="newObject">The object ot add on to the grid</param>
        /// <returns></returns>
        bool Insert(T newObject);

        /// <summary>
        /// Move an object on the grid. The object owns its own "position" in space.
        /// </summary>
        /// <param name="moveMe">The object to move</param>
        void MoveObject(T moveMe);

        /// <summary>
        /// Query for collisions will examine the map and everything in it, and return a list of objects whose bounding
        /// boxes collide with the bounding box of the input
        /// </summary>
        /// <param name="queryObject">Query the collision map for this object</param>
        /// <returns>A list of objects which have bounding box collisions</returns>
        List<T> QueryForBoundingBoxCollisions(T queryObject);

        /// <summary>
        /// Query for collisions will examine the map and everything in it, and return a list of objects whose bounding
        /// boxes collide with the bounding box of the input
        /// </summary>
        /// <param name="queryBox">A bounding box to detect collisions against</param>
        /// <returns>A list of objects which have bounding box collisions</returns>
        List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox);

        /// <summary>
        /// Query for collisions will examine the map and everything in it, and return a list of objects whose bounding
        /// boxes collide with the bounding box of the input
        /// </summary>
        /// <param name="queryBox">A bounding box to detect collisions against</param>
        /// <param name="self">An object to exclude from the result list</param>
        /// <returns>A list of objects which have bounding box collisions</returns>
        List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox, T self);

        /// <summary>
        /// Remove an object from the collision grid
        /// </summary>
        /// <param name="killMe">The object to remove from the grid.</param>
        void RemoveObject(T killMe);
    }
}
