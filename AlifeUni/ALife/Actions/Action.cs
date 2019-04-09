using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    public abstract class Action
    {
        protected Agent self;
        public Action(Agent myself)
        {
            self = myself;
        }

        public virtual void AttemptEnact(double IntensityPercent)
        {
            TakeAction(IntensityPercent);
        }

        protected abstract void TakeAction(double IntensityPercent);
    }
}
