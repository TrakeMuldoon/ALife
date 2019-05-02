using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.UtilityClasses
{
    public struct BoundingBox
    {
        public float MaxX;
        public float MaxY;
        public float MinX;
        public float MinY;

        public BoundingBox(float minX, float minY, float maxX, float maxY)
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
