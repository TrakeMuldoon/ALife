using ALife.Core.Geometry;

namespace ALife.Core.Utility
{
    public static class ExtraMath
    {
        public static int MultiMin(params int[] values)
        {
            if(values == null || values.Length == 0
                )
            {
                throw new ArgumentNullException();
            }
            int min = values[0];
            for(int i = 1; i < values.Length; i++)
            {
                min = Math.Min(min, values[i]);
            }
            return min;
        }

        public static double MultiMin(params double[] values)
        {
            if(values == null || values.Length == 0)
            {
                throw new ArgumentNullException();
            }
            double min = values[0];
            for(int i = 1; i < values.Length; i++)
            {
                min = Math.Min(min, values[i]);
            }
            return min;
        }

        public static float MultiMin(params float[] values)
        {
            if(values == null || values.Length == 0)
            {
                throw new ArgumentNullException();
            }
            float min = values[0];
            for(int i = 1; i < values.Length; i++)
            {
                min = Math.Min(min, values[i]);
            }
            return min;
        }

        public static int MultiMax(params int[] values)
        {
            if(values == null || values.Length == 0)
            {
                throw new ArgumentNullException();
            }
            int min = values[0];
            for(int i = 1; i < values.Length; i++)
            {
                min = Math.Max(min, values[i]);
            }
            return min;
        }

        public static double MultiMax(params double[] values)
        {
            if(values == null || values.Length == 0)
            {
                throw new ArgumentNullException();
            }
            double min = values[0];
            for(int i = 1; i < values.Length; i++)
            {
                min = Math.Max(min, values[i]);
            }
            return min;
        }

        public static float MultiMax(params float[] values)
        {
            if(values == null || values.Length == 0)
            {
                throw new ArgumentNullException();
            }
            float min = values[0];
            for(int i = 1; i < values.Length; i++)
            {
                min = Math.Max(min, values[i]);
            }
            return min;
        }

        public static Geometry.Shapes.Point TranslateByVector(Geometry.Shapes.Point startPoint, double radians, double distance)
        {
            double newX = (distance * Math.Cos(radians)) + startPoint.X;
            double newY = (distance * Math.Sin(radians)) + startPoint.Y;

            return new Geometry.Shapes.Point(newX, newY);
        }

        public static Geometry.Shapes.Point TranslateByVector(Geometry.Shapes.Point startPoint, Angle angle, double distance)
        {
            return TranslateByVector(startPoint, angle.Radians, distance);
        }

        public static double DistanceBetweenTwoPoints(Geometry.Shapes.Point a, Geometry.Shapes.Point b)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double delX = (double)(a.X - b.X);
            double delY = (double)(a.Y - b.Y);

            return Math.Sqrt((delX * delX) + (delY * delY));
        }

        public static double AngleBetweenPoints(Geometry.Shapes.Point target, Geometry.Shapes.Point source)
        {
            double deltaX = target.X - source.X;
            double deltaY = target.Y - source.Y;

            double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
            return angleBetweenPoints;
        }
    }
}
