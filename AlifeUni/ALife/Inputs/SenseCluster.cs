using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public abstract class SenseInput : Input
    {
        public readonly CollisionDetector myField;
        public abstract double Detect();
    }
}
