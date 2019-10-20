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
            return true;
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


        //http://devmag.org.za/2009/04/17/basic-collision-detection-in-2d-part-2/
        //        LineLineCollision
        //Input
        //    LineA1  Point   First point on line A
        //    LineA2  Point   Second point on line A
        //    LineB1  Point   First point on line B
        //    LineB2  Point   Second point on line B
        //Output
        //    The point of the collision, or null if no collision exists.
        //Method
        //    denom = ((LineB2.Y – LineB1.Y) * (LineA2.X – LineA1.X)) – ((LineB2.X – LineB1.X) * (LineA2.Y - LineA1.Y))
        //	if (denom == 0)
        //		return null
        //	else
        //		ua = (((LineB2.X – LineB1.X) * (LineA1.Y – LineB1.Y)) – ((LineB2.Y – LineB1.Y) * (LineA1.X – LineB1.X))) / denom
        //      //The following 3 lines are only necessary if we are checking line segments instead of infinite-length lines */
        //      ub = (((LineA2.X – LineA1.X) * (LineA1.Y – LineB1.Y)) – ((LineA2.Y – LineA1.Y) * (LineA1.X – LineB1.X))) / denom
        //		if (ua < 0) || (ua > 1) || (ub< 0) || (ub > 1)
        //			return null
        //		return LineA1 + ua * (LineA2 – LineA1)


//            CircleLineCollision
//Input
//    LineP1        Point   First point describing the line
//    LineP2        Point   Second point describing the line
//    CircleCentre  Point   The centre of the circle
//    Radius        Floating-point The circle's radius
//Output
//    The point(s) of the collision, or null if no collision exists.
//Method
//    // Transform to local coordinates
//    LocalP1 = LineP1 – CircleCentre
//    LocalP2 = LineP2 – CircleCentre
//    // Precalculate this value. We use it often
//    P2MinusP1 = LocalP2 – LocalP1
//    a = (P2MinusP1.X) * (P2MinusP1.X) + (P2MinusP1.Y) * (P2MinusP1.Y)
//    b = 2 * ((P2MinusP1.X * LocalP1.X) + (P2MinusP1.Y * LocalP1.Y))
//    c = (LocalP1.X* LocalP1.X) + (LocalP1.Y* LocalP1.Y) – (Radius* Radius)
//    delta = b * b – (4 * a* c)
//	  if (delta < 0) // No intersection
//          return null;
//    else if (delta == 0) // One intersection
//          u = -b / (2 * a)
//          return LineP1 + (u* P2MinusP1)
//          /* Use LineP1 instead of LocalP1 because we want our answer in global
//              space, not the circle's local space */
//    else if (delta > 0) // Two intersections
//		SquareRootDelta = sqrt(delta)
//        u1 = (-b + SquareRootDelta) / (2 * a)
//		u2 = (-b - SquareRootDelta) / (2 * a)
//		return { LineP1 + (u1* P2MinusP1) ; LineP1 + (u2* P2MinusP1)
    }
}