using ALifeUni.ALife.UtilityClasses;
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

        public override ActionCluster CloneAction(Agent newParent)
        {
            return new ColorCluster(newParent);
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
            IShape shape = self.Shape;
            byte rByte = GetNewColour("Red", shape.Color.R);
            byte gByte = GetNewColour("Green", shape.Color.G);
            byte bByte = GetNewColour("Blue", shape.Color.B);

            shape.Color = Color.FromArgb(255, rByte, gByte, bByte);
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
                return "Set Colour to " + self.Shape.Color.ToString();
            }
            else
            {
                return "Unchanged";
            }
        }
    }
}
