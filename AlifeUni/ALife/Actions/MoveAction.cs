using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace AlifeUniversal.ALife
{
    class MoveAction : Action
    {
        private double Speed = Settings.AgentDefaultSpeed;

        public MoveAction(Agent myself) : base(myself)
        {
        
        }

        protected override void TakeAction(double IntensityPercent)
        {
            Point origin = self.CentrePoint;

            Point destination = new Point();
            destination.X = origin.X + (Speed / 100) * IntensityPercent * Math.Cos(self.OrientationInRads);
            destination.Y = origin.Y + (Speed / 100) * IntensityPercent * Math.Sin(self.OrientationInRads);
            self.CentrePoint = destination;
        }
    }
}
