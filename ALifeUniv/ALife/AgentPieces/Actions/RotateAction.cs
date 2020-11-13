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
        public override string Name
        {
            get
            {
                return "Rotate";
            }
        }
        protected override bool AttemptSuccessful()
        {
            return true;
        }

        protected override void TakeAction(double IntensityPercent)
        {
            //TODO: Linked to Settings, should be linked to config.
            double turn = (Settings.AgentMaximumTurnDegrees * IntensityPercent) - (Settings.AgentMaximumTurnDegrees/2);

            Angle myOrientation = self.Orientation;
            myOrientation.Degrees += turn;
        }
    }
}
