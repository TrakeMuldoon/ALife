﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class ColorCluster : ActionCluster
    {
        public ColorCluster(Agent self) : base(self, "Color")
        {
            SubActions.Add("SetRed", new ActionPart("SetRed", Name));
            SubActions.Add("SetGreen", new ActionPart("SetGreen", Name));
            SubActions.Add("SetBlue", new ActionPart("SetBlue", Name));
            SubActions.Add("ClearRed", new ActionPart("ClearRed", Name));
            SubActions.Add("ClearGreen", new ActionPart("ClearGreen", Name));
            SubActions.Add("ClearBlue", new ActionPart("ClearBlue", Name));
        }

        protected override bool ValidatePreconditions()
        {
            //TODO: Draw this from Config
            return true;
        }
        protected override bool SubActionsEngaged()
        {
            foreach(ActionPart ap in SubActions.Values)
            {
                if(ap.Intensity != 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool AttemptEnact()
        {
            byte rByte = GetNewColour("Red", self.Color.R);
            byte gByte = GetNewColour("Green", self.Color.G);
            byte bByte = GetNewColour("Blue", self.Color.B);

            self.Color = Color.FromArgb(255, rByte, gByte, bByte);
            return true;
        }

        private byte GetNewColour(string colourName, byte currentByte)
        {
            double setC = SubActions["Set" + colourName].Intensity;
            double clearC = SubActions["Clear" + colourName].Intensity;

            //Clearing a colour always works
            if(clearC != 0.0)
            {
                return 0;
            }
            //If there is a set value
            if(setC != 0.0)
            {
                return (byte)(255 * setC);
            }
            else //original value remains
            {
                return currentByte;
            }
        }

        protected override void FailureResults()
        {
            //TODO: Draw this from Config
        }


        protected override void SuccessResults()
        {
            //TODO: Draw this from Config
        }

        public override string LastTurnString()
        {
            if(ActivatedLastTurn)
            {
                return "Set Colour to " + self.Color.ToString();
            }
            else
            {
                return "Unchanged";
            }
        }
    }
}