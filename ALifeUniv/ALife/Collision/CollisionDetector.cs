using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
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
                ShapesEnum meShape = me.GetShapeEnum();
                ShapesEnum woShapeType = woShape.GetShapeEnum();
                if(DetectorMarshaller[meShape][woShapeType](me, woShape))
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
            if(IsPointWithinSweep(circle.CentrePoint, sector))
            {
                //if it is, the either the target is within the radius distance or it's too far.
                return CircleCircleCollision(circle, new Circle(sector.CentrePoint, sector.Radius));
            }

            //Check the centrepoint of the Sector
            if(PointCircleCollision(sector.CentrePoint, circle)) return true;

            //Check the left point of the sector
            Point leftPoint = ExtraMath.TranslateByVector(sector.CentrePoint, sector.AbsoluteOrientation, sector.Radius);
            if(PointCircleCollision(leftPoint, circle)) return true;

            //Check the right point of the sector
            Point rightPoint = ExtraMath.TranslateByVector(sector.CentrePoint, (sector.AbsoluteOrientation + sector.SweepAngle), sector.Radius);
            if(PointCircleCollision(rightPoint, circle)) return true;

            //Now we're checking the line segment collisions
            if(LineSegmentCircleCollision(sector.CentrePoint, leftPoint, circle)) return true;

            if(LineSegmentCircleCollision(sector.CentrePoint, rightPoint, circle)) return true;

            //All Options Exhausted
            return false;
        }

        public static Boolean IndividualShapeCollision(Circle circle, Rectangle rectangle)
        {
            //First check if an arbitrary rectangle point is within the Circle.
            //Then check if the circle intersects with any of the line segments
            //Then check if the circle is entirely within the rectangle.

            //First check if any arbitrary rectangle point is within the circle.
            //DNB: Check all four points, because the corner detection isn't perfect.
            if(PointCircleCollision(rectangle.TopLeft, circle)
                || PointCircleCollision(rectangle.TopRight, circle)
                || PointCircleCollision(rectangle.BottomRight, circle)
                || PointCircleCollision(rectangle.BottomLeft, circle))
            {
                return true;
            }

            if(LineSegmentCircleCollision(rectangle.TopLeft, rectangle.TopRight, circle)
                || LineSegmentCircleCollision(rectangle.TopRight, rectangle.BottomRight, circle)
                || LineSegmentCircleCollision(rectangle.BottomRight, rectangle.BottomLeft, circle)
                || LineSegmentCircleCollision(rectangle.BottomLeft, rectangle.TopLeft, circle))
            {
                return true;
            }

            if(IsPointWithinRectangle(circle.CentrePoint, rectangle))
            {
                return true;
            }

            return false;
        }
        public static Boolean IndividualShapeCollision(Sector a, Sector b)
        {
            throw new NotImplementedException();
        }

        public static Boolean IndividualShapeCollision(Sector sector, Rectangle rectangle)
        {
            //Very annoying.
            //First we need to check if any arbitrary point is within the Sector
            if(IsPointWithinSector(rectangle.TopLeft, sector)
                || IsPointWithinRectangle(sector.LeftPoint, rectangle))
            {
                return true;
            }

            Sector s = sector;
            //then we check if the lengths intersect the rectangle
            bool segmentCollision = (DoesLineSegmentIntersectRectangle(s.CentrePoint, s.LeftPoint, rectangle)
                                     || DoesLineSegmentIntersectRectangle(s.CentrePoint, s.RightPoint, rectangle));
            if(segmentCollision)
                return true;

            //then we check if the linesegments intersect the circle, and if they do, if the points are within the sweep.
            bool RecArcCollision = DoesRectangleIntersectArc(sector, rectangle);

            return RecArcCollision;
        }


        public static Boolean IndividualShapeCollision(Rectangle a, Rectangle b)
        {
            bool segmentCollision = (DoesLineSegmentIntersectRectangle(a.TopLeft, a.TopRight, b)
                                     || DoesLineSegmentIntersectRectangle(a.TopRight, a.BottomRight, b)
                                     || DoesLineSegmentIntersectRectangle(a.BottomRight, a.BottomLeft, b)
                                     || DoesLineSegmentIntersectRectangle(a.BottomLeft, a.TopLeft, b));
            if(segmentCollision)
                return true;

            //No line segments collide, now we check if one is inside the other.
            return IsPointWithinRectangle(a.TopLeft, b)
                  || IsPointWithinRectangle(b.TopLeft, a);
        }

        private static Boolean DoesRectangleIntersectArc(Sector sector, Rectangle rectangle)
        {
            Rectangle r = rectangle;
            return (DoesLineSegmentIntersectSector(r.TopLeft, r.TopRight, sector)
                    || DoesLineSegmentIntersectSector(r.TopRight, r.BottomRight, sector)
                    || DoesLineSegmentIntersectSector(r.BottomRight, r.BottomLeft, sector)
                    || DoesLineSegmentIntersectSector(r.BottomLeft, r.TopLeft, sector));
        }

        private static Boolean DoesLineSegmentIntersectSector(Point a1, Point a2, Sector sector)
        {
            Circle sectorAsCircle = new Circle(sector.CentrePoint, sector.Radius);
            List<Point> potentialPoints = FindLineCircleIntersections(sectorAsCircle, a1, a2);
            List<Point> pointsInLineSegment = new List<Point>();
            foreach(Point p in potentialPoints)
            {
                if(IsValueBetweenValues(p.X, a1.X, a2.X))
                {
                    pointsInLineSegment.Add(p);
                }
            }

            foreach(Point px in pointsInLineSegment)
            {
                if(IsPointWithinSweep(px, sector))
                {
                    return true;
                }
            }
            //includes the no points case.
            return false;
        }

        private static Boolean IsValueBetweenValues(double eval, double a, double b)
        {
            if(a < b)
            {
                return a <= eval
                        && eval <= b;
            }
            else
            {
                return b <= eval
                        && eval <= a;
            }
        }

        private static Boolean DoesLineSegmentIntersectRectangle(Point a1, Point a2, Rectangle rectangle)
        {
            Rectangle r = rectangle;
            return (LineSegmentLineSegmentCollision(a1, a2, r.TopLeft, r.TopRight)
                    || LineSegmentLineSegmentCollision(a1, a2, r.TopRight, r.BottomRight)
                    || LineSegmentLineSegmentCollision(a1, a2, r.BottomRight, r.BottomLeft)
                    || LineSegmentLineSegmentCollision(a1, a2, r.BottomLeft, r.TopLeft));
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

        //Stolen wholesale from this location
        ///http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        private static List<Point> FindLineCircleIntersections(Circle circle, Point p1, Point p2)
        {
            Point cp = circle.CentrePoint;
            double radius = circle.Radius;
            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;

            double A = deltaX * deltaX + deltaY * deltaY;
            double B = 2 * (deltaX * (p1.X - cp.X) + deltaY * (p1.Y - cp.Y));
            double C = (p1.X - cp.X) * (p1.X - cp.X)
                        + (p1.Y - cp.Y) * (p1.Y - cp.Y)
                        - radius * radius;

            double determinant = B * B - 4 * A * C;
            List<Point> intersections = new List<Point>();
            if((A <= 0.0000001) || (determinant < 0))
            {
                return intersections;
            }
            else if(determinant == 0)
            {
                // One solution.
                double t = -B / (2 * A);
                double iX = p1.X + t * deltaX;
                double iY = p1.Y + t * deltaY;
                intersections.Add(new Point(iX, iY));
                return intersections;
            }
            else
            {
                // Two solutions.
                double tPos = ((-B + Math.Sqrt(determinant)) / (2 * A));
                double tNeg = ((-B - Math.Sqrt(determinant)) / (2 * A));

                double i1X = p1.X + tPos * deltaX;
                double i1Y = p1.Y + tPos * deltaY;
                intersections.Add(new Point(i1X, i1Y));

                double i2X = p1.X + tNeg * deltaX;
                double i2Y = p1.Y + tNeg * deltaY;
                intersections.Add(new Point(i2X, i2Y));

                return intersections;
            }
            throw new Exception("Impossible to get here.");
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

        private static bool IsPointWithinSector(Point targetPoint, Sector sector)
        {
            if(!PointCircleCollision(targetPoint, new Circle(sector.CentrePoint, sector.Radius)))
                return false;

            return IsPointWithinSweep(targetPoint, sector);
        }

        private static bool IsPointWithinSweep(Point targetPoint, Sector sector)
        {
            double angleBetweenPoints = ExtraMath.AngleBetweenPoints(targetPoint, sector.CentrePoint);
            Angle abp = new Angle(angleBetweenPoints, true);

            Angle minimum = new Angle(0);
            abp -= sector.AbsoluteOrientation;
            Angle maximum = sector.SweepAngle;

            return abp.Degrees < maximum.Degrees;
        }

        private static bool IsPointWithinRectangle(Point p, Rectangle rect)
        {

            //  Globals which should be set before calling this function:
            //
            //  int    polyCorners  =  how many corners the polygon has (no repeats)
            //  float  polyX[]      =  horizontal coordinates of corners
            //  float  polyY[]      =  vertical coordinates of corners
            //  float  x, y         =  point to be tested
            //
            //  (Globals are used in this example for purposes of speed.  Change as
            //  desired.)
            //
            //  The function will return YES if the point x,y is inside the polygon, or
            //  NO if it is not.  If the point is exactly on the edge of the polygon,
            //  then the function may return YES or NO. 
            //    DNB This is fine for us, because if the point is ON the polygon, 
            //        Then the CircleLineSegment collisions will catch it.
            //
            //  Note that division by zero is avoided because the division is protected
            //  by the "if" clause which surrounds it.
            //http://www.alienryderflex.com/polygon/

            //int i, j = polyCorners - 1;
            //    bool oddNodes = NO;
            //    for(i = 0; i < polyCorners; i++)
            //    {
            //        if((polyY[i] < y && polyY[j] >= y
            //        || polyY[j] < y && polyY[i] >= y)
            //        && (polyX[i] <= x || polyX[j] <= x))
            //        {
            //            if(polyX[i] + (y - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]) < x)
            //            {
            //                oddNodes = !oddNodes;
            //            }
            //        }
            //        j = i;
            //    }
            //    return oddNodes;

            bool tltr = LinePointSubCollision(p, rect.TopLeft, rect.TopRight);
            bool trbr = LinePointSubCollision(p, rect.TopRight, rect.BottomRight);
            bool brbl = LinePointSubCollision(p, rect.BottomRight, rect.BottomLeft);
            bool bltl = LinePointSubCollision(p, rect.BottomLeft, rect.TopLeft);

            return tltr ^= trbr ^= brbl ^= bltl;
        }

        private static bool LinePointSubCollision(Point p, Point a1, Point a2)
        {
            //Check if the point's y value falls between then other two.
            bool yCollision = (a1.Y < p.Y && a2.Y >= p.Y)
                               || (a2.Y < p.Y && a1.Y >= p.Y);
            //Check if the X value is to the left. (we're only checking one direction)
            bool xCollision = (a1.X <= p.X || a2.X <= p.X);
            if(yCollision && xCollision)
            {
                if(a1.X + (p.Y - a1.Y) / (a2.Y - a1.Y) * (a2.X - a1.X) < p.X)
                {
                    return true;
                }
                //else
            }
            //else
            return false;
        }

        private static bool LineSegmentLineSegmentCollision(Point a1, Point a2, Point b1, Point b2)
        {
            //http://devmag.org.za/2009/04/17/basic-collision-detection-in-2d-part-2/
            double deltaAX = a2.X - a1.X;
            double deltaAY = a2.Y - a1.Y;

            double deltaBX = b2.X - b1.X;
            double deltaBY = b2.Y - b1.Y;

            double deltaABY = a1.Y - b1.Y;
            double deltaABX = a1.X - b1.X;

            double denom = (deltaBY * deltaAX) - (deltaBX * deltaAY);
            if(denom == 0)
                return false; //parallel

            double ua = ((deltaBX * deltaABY) - (deltaBY * deltaABX)) / denom;

            double ub = ((deltaAX * deltaABY) - (deltaAY * deltaABX)) / denom;

            if(ua < 0
                || ua > 1
                || ub < 0
                || ub > 1)
            {
                return false;
            }
            return true;
            //		return LineA1 + ua * (LineA2 – LineA1) //intersection point
        }


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


        //http://devmag.org.za/2009/04/17/basic-collision-detection-in-2d-part-2/
        //        LineLineCollision
        //            CircleLineCollision

    }
}