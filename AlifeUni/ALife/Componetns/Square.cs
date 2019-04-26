using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUni.ALife.Componetns
{
    public struct Square
    {
        public float MaxX;
        public float MaxY;
        public float MinX;
        public float MinY;

        public bool IsCollision(Square interloper)
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
