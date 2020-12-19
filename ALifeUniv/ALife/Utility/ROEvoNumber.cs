using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Utility
{
    public class ROEvoNumber : EvoNumber
    {
        public override double Value 
        { 
            get => base.Value;
            set => throw new NotImplementedException("Do Not Set ReadOnly EvoNumber Values"); 
        }

        public ROEvoNumber(double startValue, double startValueEvoDeltaMax
                            , double valueMin, double valueMax, double valueHardMin, double valueHardMax, double valueMinMaxEvoMax)
            : base(startValue, startValueEvoDeltaMax
                   , valueMin, valueMax, valueHardMin, valueHardMax, valueMinMaxEvoMax
                   , 0, 0, 0, true)
        {

        }
    }
}
