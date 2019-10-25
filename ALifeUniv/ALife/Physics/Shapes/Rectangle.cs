using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Rectangle : IShape
    {
        public Point CentrePoint => throw new NotImplementedException();

        public Angle Orientation => throw new NotImplementedException();

        public BoundingBox BoundingBox => throw new NotImplementedException();

        public Color Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ShapesEnum GetShapeEnum()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
