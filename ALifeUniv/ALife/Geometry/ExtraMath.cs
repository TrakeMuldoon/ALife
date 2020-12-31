using System;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public static class ExtraMath
    {
        public static int MultiMin(params int[] values)
        {
            if(values == null)
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
            if(values == null)
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
            if(values == null)
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
            if(values == null)
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
            if(values == null)
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
            if(values == null)
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

        public static double DistanceBetweenTwoPoints(Point a, Point b)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double delX = (double)(a.X - b.X);
            double delY = (double)(a.Y - b.Y);

            return Math.Sqrt((delX * delX) + (delY * delY));
        }

        public static double AngleBetweenPoints(Point target, Point source)
        {
            double deltaX = target.X - source.X;
            double deltaY = target.Y - source.Y;

            double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
            return angleBetweenPoints;
        }
    }
}
