using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class Zone : AARectangle
    {
        public String Name
        {
            get;
            private set;
        }

        public Zone(String name, Point topLeft, double xWidth, double yHeight, Color color) : base(topLeft, xWidth, yHeight, color)
        {
            Color lowAlpha = Color;
            lowAlpha.A = 50;
            Color = lowAlpha;

            Name = name;
        }
    }
}
