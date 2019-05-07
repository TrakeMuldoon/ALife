using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Sector : IShape
    {
        public Point CentrePoint;
        public float Radius;
        public Angle SweepInDegrees;
        public Angle OrientationInDegrees;

        public void DrawOnCanvas()
        {
            throw new NotImplementedException();
        }

        public BoundingBox GetBoundingBox()
        {
            throw new NotImplementedException();
        }
    }
}
