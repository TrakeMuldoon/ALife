using System.Runtime.CompilerServices;

namespace ALife.Core.Geometry.Shapes
{
    public struct BoundingBox
    {
        public double MaxX;
        public double MaxY;
        public double MinX;
        public double MinY;

        public double XLength
        {
            get
            {
                return MaxX - MinX;
            }
        }
        public double YHeight
        {
            get
            {
                return MaxY - MinY;
            }
        }

        public BoundingBox(double minX, double minY, double maxX, double maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public bool IsCollision(BoundingBox interloper)
        {
            if(MinX < interloper.MaxX
                && MaxX > interloper.MinX
                && MinY < interloper.MaxY
                && MaxY > interloper.MinY)
            {
                return true;
            }
            else //explicit else
            {
                return false;
            }
        }

        /// <summary>
        /// Transforms the bounding box by adjusting its coordinates based on the provided point.
        /// Updates the bounding box dimensions to encompass the given point if necessary
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransformByPoint(Point point)
        {
            TransformByXCoord(point.X);
            TransformByYCoord(point.Y);
        }

        /// <summary>
        /// Adjusts the bounding box's minimum or maximum Y coordinate to encompass the specified Y value.
        /// If the provided Y value is less than the current minimum Y, updates the minimum Y coordinate.
        /// If the provided Y value is greater than the current maximum Y, updates the maximum Y coordinate.
        /// </summary>
        /// <param name="y">The Y coordinate to evaluate and potentially use to expand the bounding box.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransformByYCoord(double y)
        {
            if(y < MinY)
            {
                MinY = y;
            }
            else if(y > MaxY)
            {
                MaxY = y;
            }
        }

        /// <summary>
        /// Adjusts the bounding box's minimum or maximum X coordinate to encompass the specified X value.
        /// If the provided X value is less than the current minimum X, updates the minimum X coordinate.
        /// If the provided X value is greater than the current maximum X, updates the maximum X coordinate.
        /// </summary>
        /// <param name="x">The X coordinate to evaluate and potentially use to expand the bounding box.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransformByXCoord(double x)
        {
            if(x < MinX)
            {
                MinX = x;
            }
            else if(x > MaxX)
            {
                MaxX = x;
            }
        }
    }
}
