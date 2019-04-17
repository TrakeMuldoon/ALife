using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    class RotateAction : Action
    {
        public RotateAction(Agent myself) : base(myself)
        {
        }

        protected override void TakeAction(double IntensityPercent)
        {
            double rotationInRads = (double)(2 * Math.PI * IntensityPercent) / 100;
            rotationInRads -= 60;
            rotationInRads = Math.Abs(rotationInRads);

            self.OrientationInRads += rotationInRads;
        }
    }
}
