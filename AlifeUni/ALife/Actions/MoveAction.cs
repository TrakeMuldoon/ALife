using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    class MoveAction : Action
    {
        private double Speed = Settings.AgentDefaultSpeed;

        public MoveAction(Agent myself) : base(myself)
        {
        
        }

        protected override void TakeAction(double IntensityPercent)
        {
            Vector2 origin = self.CentrePoint;

            Vector2 destination = new Vector2();
            destination.X = (float)(origin.X + (Speed / 100) * IntensityPercent * Math.Cos(self.OrientationInRads));
            destination.Y = (float)(origin.Y + (Speed / 100) * IntensityPercent * Math.Sin(self.OrientationInRads));
            self.CentrePoint = destination;
        }
    }
}
