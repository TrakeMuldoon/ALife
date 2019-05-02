using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public static class CollisionDetector
    {

        public static HashSet<WorldObject> FineGrainedCollision(List<WorldObject> toCollide, Circle me)
        {
            HashSet<WorldObject> collisions = new HashSet<WorldObject>();
            foreach(WorldObject wo in toCollide)
            {
                //If the distance between the circles is closer than this, then they overlap/collide
                float minimumDistance = wo.Radius + me.Radius;
                
                double xDeltaSq = Math.Pow(wo.CentrePoint.X - me.CentrePoint.X, 2);
                double yDeltaSq = Math.Pow(wo.CentrePoint.Y - me.CentrePoint.Y, 2);

                double distanceSq = xDeltaSq + yDeltaSq;
                double distance = Math.Sqrt(distanceSq);

                if(distance < minimumDistance)
                {
                    collisions.Add(wo);
                }
            }

            return collisions;
        }

    }
}