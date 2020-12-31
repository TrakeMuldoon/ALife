using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using System;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class EyeCluster : SenseCluster
    {
        ChildSector myShape;
        public override IShape Shape
        {
            get { return myShape; }
        }

        [Obsolete("EyeClusterDefault is deprecated, please use EyeCluster with EvoNumbers instead.")]
        public EyeCluster(WorldObject parent, String name) : this(parent, name
                                                                    , new ROEvoNumber(startValue: 5,   evoDeltaMax: 0, hardMin: 5,   hardMax: 5)
                                                                    , new ROEvoNumber(startValue: 355, evoDeltaMax: 0, hardMin: 355, hardMax: 355)
                                                                    , new ROEvoNumber(startValue: 80,  evoDeltaMax: 0, hardMin: 80,  hardMax: 80)
                                                                    , new ROEvoNumber(startValue: 25,  evoDeltaMax: 0, hardMin: 25,  hardMax: 25))
        {
        }

        private EvoNumber EvoOrientationAroundParent;
        private EvoNumber EvoRelativeOrientation;
        private EvoNumber EvoRadius;
        private EvoNumber EvoSweep;

        public EyeCluster(WorldObject parent, String name
                          , EvoNumber eOrientationAroundParent, EvoNumber eRelativeOrientation, EvoNumber eRadius, EvoNumber eSweep)
            : base(parent, name)
        {
            EvoOrientationAroundParent = eOrientationAroundParent;
            EvoRelativeOrientation = eRelativeOrientation;
            EvoRadius = eRadius;
            EvoSweep = eSweep;

            Angle orientationAroundParent = new Angle(eOrientationAroundParent.StartValue);
            Angle relativeOrientation = new Angle(eRelativeOrientation.StartValue);
            float radius = (float)eRadius.StartValue;
            Angle sweep = new Angle(eSweep.StartValue);
            myShape = new ChildSector(parent.Shape, orientationAroundParent
                                      , 5.0 //TODO: HUUUUUUGE BUG. Eyes are hardcoded to be 5 units from centre
                                      , relativeOrientation, radius, sweep);
            SubInputs.Add(new AnyInput(name + ".SeeSomething"));
            //SubInputs.Add(new CountInput(name + ".HowMany"));
            //SubInputs.Add(new EyeIdentifierInput(name + ".WhoISee"));
            //SubInputs.Add(new ColorBoolInput(name + ".IsRed", (WorldObject wo) => wo.Shape.Color.R));
            //SubInputs.Add(new ColorBoolInput(name + ".IsBlue", (WorldObject wo) => wo.Shape.Color.B));
            //SubInputs.Add(new ColorBoolInput(name + ".IsGreen", (WorldObject wo) => wo.Shape.Color.G));
            //SubInputs.Add(new ColorInput(name + ".HowRed", (WorldObject wo) => wo.Shape.Color.R));
            //SubInputs.Add(new ColorInput(name + ".HowBlue", (WorldObject wo) => wo.Shape.Color.B));
            //SubInputs.Add(new ColorInput(name + ".HowGreen", (WorldObject wo) => wo.Shape.Color.G));
        }

        public EyeCluster(WorldObject parent, String name
                          , EvoNumber eOrientationAroundParent, EvoNumber eRelativeOrientation, EvoNumber eRadius, EvoNumber eSweep
                          , Color myColor, Color myDebugColor)
            : this(parent, name
                  , eOrientationAroundParent, eRelativeOrientation, eRadius, eSweep)
        {
            this.myShape.DebugColor = myDebugColor;
            this.myShape.Color = myColor;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            EyeCluster newEC = new EyeCluster(newParent, Name,
                                              EvoOrientationAroundParent.Clone(), EvoRelativeOrientation.Clone(), EvoRadius.Clone(), EvoSweep.Clone()
                                              , myShape.Color.Clone(), myShape.DebugColor.Clone());
            return newEC;
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            EyeCluster newEC = new EyeCluster(newParent, Name,
                                              EvoOrientationAroundParent.Evolve(), EvoRelativeOrientation.Evolve(), EvoRadius.Evolve(), EvoSweep.Evolve()
                                              , myShape.Color.Clone(), myShape.DebugColor.Clone());
            return newEC;
        }
    }
}
