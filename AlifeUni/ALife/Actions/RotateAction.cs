using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    class RotateAction : Action
    {
        public RotateAction(Agent myself) : base(myself)
        {
        }

        protected override void TakeAction(double IntensityPercent)
        {
            double rotationInRads = (double)(2 * Math.PI * IntensityPercent);
            rotationInRads -= 1;
            rotationInRads = Math.Abs(rotationInRads);

            self.OrientationInRads += rotationInRads;
        }
    }
}
