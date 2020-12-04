namespace ALifeUni.ALife.UtilityClasses
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
    }
}
