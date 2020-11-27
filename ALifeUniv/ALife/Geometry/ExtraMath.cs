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

        //TODO: Make this take an "Angle" and not "Radians". I think it will be cleaner elsewhere
        public static Point TranslateByVector(Point startPoint, double radians, double distance)
        {
            double newX = (distance * Math.Cos(radians)) + startPoint.X;
            double newY = (distance * Math.Sin(radians)) + startPoint.Y;

            return new Point(newX, newY);
        }
    }
}
