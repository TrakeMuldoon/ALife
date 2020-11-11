using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public abstract class Action
    {
        protected Agent self;
        public Action(Agent myself)
        {
            self = myself;
        }

        public abstract string Name
        {
            get;
        }

        private double intensity;
        public double Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                activated = true;
                intensity = value;
            }
        }
        public double IntensityLastTurn
        {
            get;
            private set;
        }

        private bool activated;
        public bool ActivatedLastTurn
        {
            get;
            private set;
        }

        const double IntensityMax = 1.0;
        const double IntensityMin = 0.0;
        public void AttemptEnact()
        {
            if(activated && AttemptSuccessful())
            {
                Intensity = Math.Clamp(Intensity, IntensityMin, IntensityMax);
                TakeAction(Intensity);
            }
            //Reset the Intensity;
            Reset();
        }

        protected abstract bool AttemptSuccessful();

        public virtual void Reset()
        {
            IntensityLastTurn = intensity;
            intensity = 0;
            ActivatedLastTurn = activated;
            activated = false;
        }

        protected abstract void TakeAction(double IntensityPercent);
    }
}
