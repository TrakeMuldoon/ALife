using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public interface IShape
    {
        Point GetCentrePoint();
        Angle GetOrientation();
        BoundingBox GetBoundingBox();

        Type GetShapeType();

        void Reset();
    }
}
