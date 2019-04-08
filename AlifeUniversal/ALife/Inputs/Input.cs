using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    public abstract class Input
    {
        private double myValue;
        public double Value
        {
            get {
                return myValue;
            }
            set {
                mostRecentValue = myValue;
                myValue = value;
            }
        }

        private double mostRecentValue;
        public double MostRecentValue
        {
            get
            {
                return mostRecentValue;
            }
        }

        public double Delta
        {
            get
            {
                return myValue - mostRecentValue;
            }
        }

        public double Name;
    }
}
