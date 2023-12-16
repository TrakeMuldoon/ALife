namespace ALife.Core.Distributors
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

        public abstract Geometry.Shapes.Point NextObjectCentre(double BBLength, double BBHeight);
    }
}
