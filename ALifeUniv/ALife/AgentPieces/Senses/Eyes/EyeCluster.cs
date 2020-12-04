using ALifeUni.ALife.Inputs.SenseClusters;
using ALifeUni.ALife.UtilityClasses;
using System;

namespace ALifeUni.ALife
{
    public class EyeCluster : SenseCluster
    {
        ChildSector myShape;
        public override IShape Shape
        {
            get { return myShape; }
        }

        public EyeCluster(WorldObject parent, String name) : base(parent, name)
        {
            //TODO: Hardcoded sweep and orientation angles here. They should be linked to parent or to configuration
            //TODO: Hardcoded other angles here too. All bad. BAD
            Angle orientationAroundParent = new Angle(5);
            Angle relativeOrientation = new Angle(355);
            float radius = 80;
            Angle sweep = new Angle(25);
            myShape = new ChildSector(orientationAroundParent, parent.Shape, 5.0 //TODO: HUUUUUUGE BUG. Eyes are hardcoded to be 5 units from centre
                                      , relativeOrientation, radius, sweep);
            SubInputs.Add(new EyeInput(name + ".SeeSomething"));
            SubInputs.Add(new EyeCounterInput(name + ".HowMany"));
            SubInputs.Add(new EyeIdentifierInput(name + ".WhoISee"));
            SubInputs.Add(new ColorBoolInput(name + ".IsRed", (WorldObject wo) => wo.Shape.Color.R));
            SubInputs.Add(new ColorBoolInput(name + ".IsBlue", (WorldObject wo) => wo.Shape.Color.B));
            SubInputs.Add(new ColorBoolInput(name + ".IsGreen", (WorldObject wo) => wo.Shape.Color.G));
            SubInputs.Add(new ColorInput(name + ".HowRed", (WorldObject wo) => wo.Shape.Color.R));
            SubInputs.Add(new ColorInput(name + ".HowBlue", (WorldObject wo) => wo.Shape.Color.B));
            SubInputs.Add(new ColorInput(name + ".HowGreen", (WorldObject wo) => wo.Shape.Color.G));
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            EyeCluster newEC = new EyeCluster(newParent, Name);
            newEC.myShape.OrientationAroundParent = myShape.OrientationAroundParent.Clone();
            newEC.myShape.RelativeOrientation = myShape.RelativeOrientation.Clone();
            newEC.myShape.Radius = myShape.Radius;
            newEC.myShape.SweepAngle = myShape.SweepAngle.Clone();
            //TODO: Fix bug with Colors being passed around and not cloned properly
            newEC.myShape.Color = myShape.Color;
            newEC.myShape.DebugColor = myShape.DebugColor;
            return newEC;
        }
    }
}
