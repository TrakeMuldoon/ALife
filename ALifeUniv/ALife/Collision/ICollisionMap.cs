using ALifeUni.ALife.UtilityClasses;
using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public interface ICollisionMap
    {
        /// <summary>
        /// Query for collisions will examine the map and everything in it, and return a list of objects whose bounding boxes collide with the bounding box of the input
        /// </summary>
        /// <param name="queryObject">Query the collision map for this object</param>
        /// <returns></returns>
        List<WorldObject> QueryForBoundingBoxCollisions(WorldObject queryObject);
        List<WorldObject> QueryForBoundingBoxCollisions(BoundingBox queryBox);
        List<WorldObject> QueryForBoundingBoxCollisions(BoundingBox queryBox, WorldObject self);

        bool Insert(WorldObject newObject);

        void RemoveObject(WorldObject killMe);

        void MoveObject(WorldObject moveMe);

        IEnumerable<WorldObject> EnumerateItems();

    }
}
