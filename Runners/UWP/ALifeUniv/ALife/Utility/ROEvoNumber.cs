using System;

namespace ALifeUni.ALife.Utility
{
    public class ROEvoNumber : EvoNumber
    {
        public override double Value
        {
            get => base.Value;
            set => throw new NotImplementedException("Do Not Set ReadOnly EvoNumber Values");
        }

        public ROEvoNumber(double startValue, double evoDeltaMax
                           , double hardMin, double hardMax)
            : base(startValue, evoDeltaMax
                   , hardMin, hardMax, hardMin, hardMax, 0
                   , 0, 0, 0, true)
        {

        }
    }
}
