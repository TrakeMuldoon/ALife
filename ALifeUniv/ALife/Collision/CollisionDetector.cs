using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    public static class CollisionDetector
    {
        private static Dictionary<ShapesEnum, Dictionary<ShapesEnum, Func<IShape, IShape, Boolean>>> detectorMarshaller;
        private static Dictionary<ShapesEnum, Dictionary<ShapesEnum, Func<IShape, IShape, Boolean>>> DetectorMarshaller
        {
            get
            {
                if(detectorMarshaller == null)
                {
                    InitializeDetectorMarshaller();
                }
                return detectorMarshaller;
            }
        }
        private static void InitializeDetectorMarshaller()
        {
            var ddf = new Dictionary<ShapesEnum, Dictionary<ShapesEnum, Func<IShape, IShape, Boolean>>>();
            ddf.Add(ShapesEnum.AARectangle
                    , new Dictionary<ShapesEnum, Func<IShape, IShape, Boolean>>
                    {
                        [ShapesEnum.AARectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => throw new NotImplementedException("AAR -> AAR not implemented")),
                        [ShapesEnum.Rectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => throw new NotImplementedException("AAR -> REC not implemented")),
                        [ShapesEnum.Sector] = new Func<IShape, IShape, bool>((wo1, wo2) => throw new NotImplementedException("AAR -> SEC not implemented")),
                        [ShapesEnum.Circle] = new Func<IShape, IShape, bool>((wo1, wo2) => throw new NotImplementedException("AAR -> CIR not implemented"))
                    });

            ddf.Add(ShapesEnum.Rectangle
                    , new Dictionary<ShapesEnum, Func<IShape, IShape, Boolean>>
                    {
                        [ShapesEnum.AARectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => throw new NotImplementedException("REC -> AAR not implemented")),
                        [ShapesEnum.Rectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Rectangle)wo1, (Rectangle)wo2)),
                        [ShapesEnum.Sector] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Sector)wo2, (Rectangle)wo1)), //reversed
                        [ShapesEnum.Circle] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Circle)wo2, (Rectangle)wo1)) //reversed
                    });
            ddf.Add(ShapesEnum.Sector
                    , new Dictionary<ShapesEnum, Func<IShape, IShape, Boolean>>
                    {
                        [ShapesEnum.AARectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => throw new NotImplementedException("SEC -> AAR not implemented")),
                        [ShapesEnum.Rectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Sector)wo1, (Rectangle)wo2)),
                        [ShapesEnum.Sector] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Sector)wo1, (Sector)wo2)),
                        [ShapesEnum.Circle] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Circle)wo2, (Sector)wo1)) //reversed
                    });
            ddf.Add(ShapesEnum.Circle
                    , new Dictionary<ShapesEnum, Func<IShape, IShape, Boolean>>
                    {
                        [ShapesEnum.AARectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => throw new NotImplementedException("CIR -> AAR not implemented")),
                        [ShapesEnum.Rectangle] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Circle)wo1, (Rectangle)wo2)),
                        [ShapesEnum.Sector] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Circle)wo1, (Sector)wo2)),
                        [ShapesEnum.Circle] = new Func<IShape, IShape, bool>((wo1, wo2) => IndividualShapeCollision((Circle)wo1, (Circle)wo2))
                    });
            detectorMarshaller = ddf;
        }


        public static List<IHasShape> FineGrainedCollisionDetection(IEnumerable<IHasShape> toCollide, IShape me)
        {
            List<IHasShape> collisions = new List<IHasShape>();
            foreach(IHasShape wo in toCollide)
            {
                IShape woShape = wo.Shape;
                if(DetectorMarshaller[me.GetShapeEnum()][woShape.GetShapeEnum()](me, woShape))
                {
                    collisions.Add(wo);
                }
            }
            return collisions;
        }

        public static Boolean IndividualShapeCollision(Circle circle1, Circle circle2)
        {
            return CircleCircleCollision(circle1, circle2);
        }
        public static Boolean IndividualShapeCollision(Circle circle, Sector sector)
        {
            //All Collision Detection has the following cases
            //1. a is contained by b
            //2. b is contained by a
            //3. a breaks one of the edges of b (and reciprocally b, to a)
            //3a (left sector segment)
            //3b (right sector segment)
            //3c (rounded segment)
            //The algorithm for circle sector collision will be as follows

            //Check if the circle is within the sector or breaks the circle portion
            //Check if any of the three points of sector are within B
            //Check if the circle breaks the line segments

            //Check if the centre point is within the sweep range
            if(PointWithinSweep(circle.CentrePoint, sector))
            {
                //if it is, the either the target is within the radius distance or it's too far.
                return CircleCircleCollision(circle, new Circle(sector.CentrePoint, sector.Radius));
            }

            //Check the centrepoint of the Sector
            if(PointCircleCollision(sector.CentrePoint, circle)) return true;

            //Check the left point of the sector
            Point leftPoint = ExtraMath.TranslateByVector(sector.CentrePoint, sector.AbsoluteOrientation.Radians, sector.Radius);
            if(PointCircleCollision(leftPoint, circle)) return true;

            //Check the right point of the sector
            Point rightPoint = ExtraMath.TranslateByVector(sector.CentrePoint, (sector.AbsoluteOrientation + sector.SweepAngle).Radians, sector.Radius);
            if(PointCircleCollision(rightPoint, circle)) return true;

            //Now we're checking the line segment collisions
            if(LineSegmentCircleCollision(sector.CentrePoint, leftPoint, circle)) return true;

            if(LineSegmentCircleCollision(sector.CentrePoint, rightPoint, circle)) return true;

            //All Options Exhausted
            return false;
        }

        public static Boolean IndividualShapeCollision(Circle circle, Rectangle rectangle)
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
        
        private static Boolean PointRadiusPointRadiusCollision(Point a, float radA, Point b, float radB)
        {
            //If the distance between the points is closer or equal to this, then they overlap/collide
            float minimumDistance = radA + radB;

            double xDeltaSq = Math.Pow(a.X - b.X, 2);
            double yDeltaSq = Math.Pow(a.Y - b.Y, 2);

            double distanceSq = xDeltaSq + yDeltaSq;
            double minSq = minimumDistance * minimumDistance;

            return distanceSq <= minSq;
        }

        private static Boolean CircleCircleCollision(Circle a, Circle b)
        {
            return PointRadiusPointRadiusCollision(a.CentrePoint, a.Radius, b.CentrePoint, b.Radius);
        }

        private static Boolean PointCircleCollision(Point a, Circle c)
        {
            return PointRadiusPointRadiusCollision(a, 0, c.CentrePoint, c.Radius);
        }

        private static Boolean LineSegmentCircleCollision(Point p1, Point p2, Circle c)
        {
            //http://www.jeffreythompson.org/collision-detection/line-circle.php
            Point closest = ClosestPoint_PointToLineSegment(c.CentrePoint, p1, p2);
            bool onLine = IsPointOnLine(closest, p1, p2);
            if(!onLine) return false;

            return PointCircleCollision(closest, c);
        }

        private static Boolean IsPointOnLine(Point pt, Point line1, Point line2)
        {
            double lineX = line1.X - line2.X;
            double lineY = line1.Y - line2.Y;

            double ptLine1X = pt.X - line1.X;
            double ptLine1Y = pt.Y - line1.Y;

            double ptLine2X = pt.X - line2.X;
            double ptLine2Y = pt.Y - line2.Y;

            double lenSQ = (lineX * lineX) + (lineY * lineY);
            double ptLine1SQ = (ptLine1X * ptLine1X) + (ptLine1Y * ptLine1Y);
            double ptLine2SQ = (ptLine2X * ptLine2X) + (ptLine2Y * ptLine2Y);

            double diff = Math.Sqrt(lenSQ) - (Math.Sqrt(ptLine1SQ) + Math.Sqrt(ptLine2SQ));
            return Math.Round(diff, 3) == 0;
        }

        private static Point ClosestPoint_PointToLineSegment(Point cCP, Point p1, Point p2)
        {
            double distX = p1.X - p2.X;
            double distY = p1.Y - p2.Y;

            double lenSQ = (distX * distX) + (distY * distY);
            double dotProduct = (((cCP.X - p1.X) * (p2.X - p1.X)) + ((cCP.Y - p1.Y) * (p2.Y - p1.Y))) / lenSQ;

            double closestX = p1.X + (dotProduct * (p2.X - p1.X));
            double closestY = p1.Y + (dotProduct * (p2.Y - p1.Y));
            Point closest = new Point(closestX, closestY);
            return closest;
        }

        private static bool PointWithinSweep(Point targetPoint, Sector sector)
        {
            double deltaX = targetPoint.X - sector.CentrePoint.X;
            double deltaY = targetPoint.Y - sector.CentrePoint.Y;

            double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
            Angle abp = new Angle(angleBetweenPoints, true);

            Angle minimum = new Angle(0);
            abp -= sector.AbsoluteOrientation;
            Angle maximum = sector.SweepAngle;

            return abp.Degrees < maximum.Degrees;
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