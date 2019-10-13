using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public static class CollisionDetector
    {
        public static List<WorldObject> FineGrainedCollisionDetection(List<WorldObject> toCollide, IShape me)
        {
            switch (me.GetShape())
            {
                case ShapesEnum.Circle: return FineGrainedCollisionDetection(toCollide, (Circle)me);
                case ShapesEnum.Sector: return FineGrainedCollisionDetection(toCollide, (Sector)me);
                case ShapesEnum.Rectangle: return FineGrainedCollisionDetection(toCollide, (Rectangle)me);
            }
            return null;
        }

        public static List<WorldObject> FineGrainedCollisionDetection(List<WorldObject> toCollide, Circle me)
        {
            List<WorldObject> collisions = new List<WorldObject>();
            foreach(WorldObject wo in toCollide)
            {
                if(IndividualShapeCollision(wo, me))
                {
                    collisions.Add(wo);
                }
            }

            return collisions;
        }

        public static List<WorldObject> FineGrainedCollisionDetection(List<WorldObject> toCollide, Rectangle me)
        {
            List<WorldObject> collisions = new List<WorldObject>();
            foreach (WorldObject wo in toCollide)
            {
                if (IndividualShapeCollision(wo, me))
                {
                    collisions.Add(wo);
                }
            }

            return collisions;
        }

        public static List<WorldObject> FineGrainedCollisionDetection(List<WorldObject> toCollide, Sector me)
        {
            List<WorldObject> collisions = new List<WorldObject>();
            foreach (WorldObject wo in toCollide)
            {
                if (IndividualShapeCollision(wo, me))
                {
                    collisions.Add(wo);
                }
            }

            return collisions;
        }

        public static Boolean IndividualShapeCollision(Circle a, Circle b)
        {
            //If the distance between the circles is closer than this, then they overlap/collide
            float minimumDistance = a.Radius + b.Radius;

            double xDeltaSq = Math.Pow(a.CentrePoint.X - b.CentrePoint.X, 2);
            double yDeltaSq = Math.Pow(a.CentrePoint.Y - b.CentrePoint.Y, 2);

            double distanceSq = xDeltaSq + yDeltaSq;
            double distance = Math.Sqrt(distanceSq);

            return distance < minimumDistance;
        }
        public static Boolean IndividualShapeCollision(Circle a, Sector b)
        {
            return false;
        }
        public static Boolean IndividualShapeCollision(Circle a, Rectangle b)
        {
            throw new NotImplementedException();
        }
        public static Boolean IndividualShapeCollision(Sector a, Sector b)
        {
            throw new NotImplementedException();
        }
        public static Boolean IndividualShapeCollision(Sector a, Rectangle b)
        {
            throw new NotImplementedException();
        }
        public static Boolean IndividualShapeCollision(Rectangle a, Rectangle b)
        {
            throw new NotImplementedException();
        }


        //Line Segment Collision
        //http://devmag.org.za/2009/04/13/basic-collision-detection-in-2d-part-1/

        //This works on lines of inifinite length. There is additional code for line segments, outlined at the link.

        //        Input
        //    LineA1  Point First point on line A

        //    LineA2 Point   Second point on line A
        //   LineB1  Point First point on line B

        //    LineB2 Point   Second point on line B
        //Output

        //    True if lines collide
        //Method
        //    denom = ((LineB2.Y – LineB1.Y) * (LineA2.X – LineA1.X)) –
        //		((LineB2.X – lineB1.X) * (LineA2.Y - LineA1.Y))
        //	return denom != 0
    }
}