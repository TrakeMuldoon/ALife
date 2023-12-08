using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    public abstract class WorldObjectDistributor
    {
        protected readonly Zone StartZone;
        protected readonly bool TrackCollisions;
        protected readonly string CollisionLevel;

        protected WorldObjectDistributor(Zone startZone, bool trackCollisions, string collisionLevel)
        {
            StartZone = startZone;
            TrackCollisions = trackCollisions;
            CollisionLevel = collisionLevel;
        }

        public abstract Point NextObjectCentre(double BBLength, double BBHeight);
    }
}
