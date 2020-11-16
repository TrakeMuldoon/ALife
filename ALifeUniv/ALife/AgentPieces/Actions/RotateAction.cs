﻿using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    class RotateAction : AgentAction
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
            //TODO: This has a value of "zero" equalling HARD LEFT. Not working as intended.
            double turn = (Settings.AgentMaximumTurnDegrees * IntensityPercent) - (Settings.AgentMaximumTurnDegrees/2);

            Angle myOrientation = self.Orientation;
            myOrientation.Degrees += turn;
        }
    }
}
