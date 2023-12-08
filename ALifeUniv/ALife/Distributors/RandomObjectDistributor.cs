using ALifeUni.ALife.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.Distributors
{
    public class RandomObjectDistributor : WorldObjectDistributor
    {
        public RandomObjectDistributor(Zone startZone, bool trackCollisions, string collisionLevel) : base(startZone, trackCollisions, collisionLevel)
        {
        }

        public override Point NextObjectCentre(double BBLength, double BBHeight)
        {
            double halfLength = BBLength / 2;
            double halfHeight = BBHeight / 2;

            double xMin = StartZone.TopLeft.X + halfLength;
            double xMax = StartZone.TopLeft.X + StartZone.XWidth - halfLength;
            double yMin = StartZone.TopLeft.Y + halfHeight;
            double yMax = StartZone.TopLeft.Y + StartZone.YHeight - halfHeight;

            //If we aren't tracking collisions, then any point in the area is valid
            if(!TrackCollisions)
            {
                double X = Planet.World.NumberGen.Next((int)xMin, (int)xMax);
                double Y = Planet.World.NumberGen.Next((int)yMin, (int)yMax);
                return new Point(X, Y);
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
                return new Point(newX, newY);
            }
            else
            {
                throw new Exception("Unable to place Agent (random)");
            }
        }
    }
}
