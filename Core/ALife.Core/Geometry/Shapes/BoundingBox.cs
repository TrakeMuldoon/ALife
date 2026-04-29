namespace ALife.Core.Geometry.Shapes
{
    public struct BoundingBox
    {
        private double x;

        private double y;

        private double maxX;

        private double maxY;

        private double width;

        private double height;

        public double X
        {
            get => x;
            set
            {
                x = value;
            }
        }
        
        
        public BoundingBox(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public bool IsCollision(BoundingBox interloper)
        {
            if (X < interloper.X + interloper.Width
                && X + Width > interloper.X
                && Y < interloper.Y + interloper.Height
                && Y + Height > interloper.Y)
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