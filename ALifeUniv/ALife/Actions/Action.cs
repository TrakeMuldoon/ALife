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

        public void AttemptEnact()
        {
            if(activated && AttemptSuccessful())
            {
                Intensity = Intensity > 1.0 ? 1.0 : (Intensity < 0.0 ? 0.0 : Intensity); 
                TakeAction(Intensity);
                //Reset the Intensity;
                Reset();
            }
            //else
            //No need to reset, because nothing has changed. 
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
