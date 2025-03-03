namespace ALife.Core.Utility.Geometry;

/// <summary>
/// THe remaining math functions (related to geometry) from the old ExtraMath class.
/// </summary>
public class GeometryMath
{
    public static double AngleBetweenPoints(Point target, Point source)
    {
        double deltaX = target.X - source.X;
        double deltaY = target.Y - source.Y;

        double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
        return angleBetweenPoints;
    }

    public static double DistanceBetweenTwoPoints(Point a, Point b)
    {
        return Math.Sqrt(SquaredDistanceBetweenTwoPoints(a, b));
    }

    public static double SquaredDistanceBetweenTwoPoints(Point a, Point b)
    {
        //pythagorean theorem c^2 = a^2 + b^2
        //thus c = square root(a^2 + b^2)
        double delX = a.X - b.X;
        double delY = a.Y - b.Y;

        return (delX * delX) + (delY * delY);
    }

    public static Point TranslateByVector(Point startPoint, double radians, double distance)
    {
        double newX = (distance * Math.Cos(radians)) + startPoint.X;
        double newY = (distance * Math.Sin(radians)) + startPoint.Y;

        return new Point(newX, newY);
    }

    public static Point TranslateByVector(Point startPoint, Angle angle, double distance)
    {
        return TranslateByVector(startPoint, angle.Radians, distance);
    }
}
