using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Inputs
{
    public abstract class SenseInput<T> : Input<T>, IShape
    {
        public abstract IShape GetShape();
        public abstract String GetCollisionLevel();
        public abstract void Detect();

        public abstract BoundingBox GetBoundingBox();

        public abstract void DrawOnCanvas();

    }
}
