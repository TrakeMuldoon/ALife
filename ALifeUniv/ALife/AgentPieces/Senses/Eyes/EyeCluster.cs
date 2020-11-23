using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALifeUni.ALife.Inputs.SenseClusters;
using ALifeUni.ALife.UtilityClasses;

namespace ALifeUni.ALife
{
    public class EyeCluster : SenseCluster
    {
        ChildSector myShape;

        public EyeCluster(Agent parent, String name) : base(parent, name)
        {
            //TODO: Hardcoded sweep and orientation angles here. They should be linked to parent or to configuration
            myShape = new ChildSector(new Angle(270), new Angle(260), parent);
            SubInputs.Add(new EyeInput(name + ".SeeSomething"));
            SubInputs.Add(new EyeCounterInput(name + ".HowMany"));
            SubInputs.Add(new EyeIdentifierInput(name + ".WhoISee"));
            SubInputs.Add(new ColorBoolInput(name + ".IsRed", (WorldObject wo) => wo.Color.R));
            SubInputs.Add(new ColorBoolInput(name + ".IsBlue", (WorldObject wo) => wo.Color.B));
            SubInputs.Add(new ColorBoolInput(name + ".IsGreen", (WorldObject wo) => wo.Color.G));
            SubInputs.Add(new ColorInput(name + ".HowRed", (WorldObject wo) => wo.Color.R));
            SubInputs.Add(new ColorInput(name + ".HowBlue", (WorldObject wo) => wo.Color.B));
            SubInputs.Add(new ColorInput(name + ".HowGreen", (WorldObject wo) => wo.Color.G));
        }

        public override IShape GetShape()
        {
            return myShape;
        }
    }
}
