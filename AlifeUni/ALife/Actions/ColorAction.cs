using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace AlifeUniversal.ALife
{
    class ColorAction : Action
    {
        public ColorAction(Agent myself) : base(myself)
        {
        }

        protected override void TakeAction(double IntensityPercent)
        {
            double R = IntensityPercent;
            double G = IntensityPercent * 100;
            double B = IntensityPercent * 10000;

            R = R - (int)R;
            G = G - (int)G;
            B = B - (int)B;

            byte rByte = (byte)(R * 255);
            byte gByte = (byte)(G * 255);
            byte bByte = (byte)(B * 255);

            self.color = Color.FromArgb(255, rByte, gByte, bByte);
        }
    }
}
