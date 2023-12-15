﻿using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects;

namespace ALife.Core.Distributors
{
    public class RandomObjectDistributor : WorldObjectDistributor
    {
        public RandomObjectDistributor(Zone startZone, bool trackCollisions, string collisionLevel) : base(startZone, trackCollisions, collisionLevel)
        {
        }

        public override Geometry.Shapes.Point NextObjectCentre(double BBLength, double BBHeight)
        {
            double halfLength = BBLength / 2;
            double halfHeight = BBHeight / 2;

            double xMin = StartZone.TopLeft.X + halfLength;
            double xMax = StartZone.TopLeft.X + StartZone.XWidth - halfLength;
            double yMin = StartZone.TopLeft.Y + halfHeight;
            double yMax = StartZone.TopLeft.Y + StartZone.YHeight - halfHeight;

            //If we aren't tracking collisions, then any Geometry.Shapes.Point in the area is valid
            if(!TrackCollisions)
            {
                double X = Planet.World.NumberGen.Next((int)xMin, (int)xMax);
                double Y = Planet.World.NumberGen.Next((int)yMin, (int)yMax);
                return new Geometry.Shapes.Point(X, Y);
            }

            int attempts = 0;
            List<WorldObject> collisions;
            double newX, newY;
            do
            {
                newX = Planet.World.NumberGen.Next((int)xMin, (int)xMax);
                newY = Planet.World.NumberGen.Next((int)yMin, (int)yMax);

                BoundingBox bb = new BoundingBox(newX - halfLength, newY - halfHeight, newX + halfLength, newY + halfHeight);
                collisions = Planet.World.CollisionLevels[CollisionLevel].QueryForBoundingBoxCollisions(bb);
                attempts++;
            } while(collisions.Count > 0
                    && attempts < 15); //TODO: number of attempts is hardcoded here

            if(collisions.Count == 0)
            {
                return new Geometry.Shapes.Point(newX, newY);
            }
            else
            {
                throw new Exception("Unable to place Agent (random)");
            }
        }
    }
}
