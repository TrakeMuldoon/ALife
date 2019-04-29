using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public interface ICollisionMap : IEnumerable<WorldObject>
    {
        bool Insert(WorldObject newObject);

        /// <summary>
        /// Query for collisions will examine the map and everything in it, and return a list of objects whose bounding boxes collide with the bounding box of the input
        /// </summary>
        /// <param name="queryObject">Query the collision map for this object</param>
        /// <returns></returns>
        List<WorldObject> QueryForCollisions(WorldObject queryObject);
        List<WorldObject> QueryForCollisions(BoundingBox queryBox);
        List<WorldObject> QueryForCollisions(BoundingBox queryBox, WorldObject self);

        void RemoveObject(WorldObject killMe);

        void MoveObject(WorldObject moveMe);

    }
}
