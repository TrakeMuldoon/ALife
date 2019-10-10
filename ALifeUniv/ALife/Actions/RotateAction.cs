using ALifeUni.ALife.UtilityClasses;
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
            double turn = Settings.AgentMaximumTurnDegrees * IntensityPercent;

            Angle myOrientation = self.Orientation;
            myOrientation.Degrees += turn;
        }
    }
}
