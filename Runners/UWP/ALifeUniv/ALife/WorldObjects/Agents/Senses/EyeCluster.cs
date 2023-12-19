using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents.Senses.Eyes;
using ALifeUni.ALife.WorldObjects.Agents.Senses.Generic;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife.WorldObjects.Agents.Senses
{
    public class EyeCluster : SenseCluster
    {
        ChildSector myShape;
        public override IShape Shape
        {
            get { return myShape; }
        }

        [Obsolete("EyeClusterDefault is deprecated, please use EyeCluster with EvoNumbers instead.")]
        public EyeCluster(WorldObject parent, String name) : this(parent, name, false
                                                                    , new ROEvoNumber(startValue: 5,   evoDeltaMax: 0, hardMin: 5,   hardMax: 5)
                                                                    , new ROEvoNumber(startValue: 355, evoDeltaMax: 0, hardMin: 355, hardMax: 355)
                                                                    , new ROEvoNumber(startValue: 80,  evoDeltaMax: 0, hardMin: 80,  hardMax: 80)
                                                                    , new ROEvoNumber(startValue: 25,  evoDeltaMax: 0, hardMin: 25,  hardMax: 25))
        {
        }

        public bool IncludeColor
        {
            get;
            private set;
        }
        private EvoNumber EvoOrientationAroundParent;
        private EvoNumber EvoRelativeOrientation;
        private EvoNumber EvoRadius;
        private EvoNumber EvoSweep;

        public EyeCluster(WorldObject parent, String name, bool withColour
                          , EvoNumber eOrientationAroundParent, EvoNumber eRelativeOrientation, EvoNumber eRadius, EvoNumber eSweep)
            : base(parent, name)
        {
            IncludeColor = withColour;
            EvoOrientationAroundParent = eOrientationAroundParent;
            EvoRelativeOrientation = eRelativeOrientation;
            EvoRadius = eRadius;
            EvoSweep = eSweep;

            Angle orientationAroundParent = new Angle(eOrientationAroundParent.StartValue);
            Angle relativeOrientation = new Angle(eRelativeOrientation.StartValue);
            float radius = (float)eRadius.StartValue;
            Angle sweep = new Angle(eSweep.StartValue);
            myShape = new ChildSector(parent.Shape, orientationAroundParent
                                      , 5.0 //TODO: HUUUUUUGE BUG. See explanation below
                                      , relativeOrientation, radius, sweep);
            SubInputs.Add(new AnyInput(name + ".SeeSomething"));
            //SubInputs.Add(new CountInput(name + ".HowMany"));
            //SubInputs.Add(new EyeIdentifierInput(name + ".WhoISee"));
            if(IncludeColor)
            {
                SubInputs.Add(new ColorBoolInput(name + ".IsRed", (WorldObject wo) => wo.Shape.Color.R));
                SubInputs.Add(new ColorBoolInput(name + ".IsBlue", (WorldObject wo) => wo.Shape.Color.B));
                SubInputs.Add(new ColorBoolInput(name + ".IsGreen", (WorldObject wo) => wo.Shape.Color.G));
                //SubInputs.Add(new ColorInput(name + ".HowRed", (WorldObject wo) => wo.Shape.Color.R));
                //SubInputs.Add(new ColorInput(name + ".HowBlue", (WorldObject wo) => wo.Shape.Color.B));
                //SubInputs.Add(new ColorInput(name + ".HowGreen", (WorldObject wo) => wo.Shape.Color.G));
            }

            //Re: "TODO: HUUGE BUUG." The code there is setting the distance for the root point of the childSector (the vision radius of the eye) 
            //to 5.0 away from the parent center point. This is a bug, because if the parent radius is modified to be larger than the that distancce, then the eye will be inside it.
            //The reason we can't set it to be "parent.Shape.Radius" is that the parent is not guaranteed to be a circle. 
        }

        public EyeCluster(WorldObject parent, String name
                  , EvoNumber eOrientationAroundParent, EvoNumber eRelativeOrientation, EvoNumber eRadius, EvoNumber eSweep)
            : this(parent, name, false
                  , eOrientationAroundParent, eRelativeOrientation, eRadius, eSweep)
        { }


        public EyeCluster(WorldObject parent, String name, bool includeColor
                          , EvoNumber eOrientationAroundParent, EvoNumber eRelativeOrientation, EvoNumber eRadius, EvoNumber eSweep
                          , Color myColor, Color myDebugColor)
            : this(parent, name, includeColor
                  , eOrientationAroundParent, eRelativeOrientation, eRadius, eSweep)
        {
            this.myShape.DebugColor = myDebugColor;
            this.myShape.Color = myColor;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            EyeCluster newEC = new EyeCluster(newParent, Name, IncludeColor
                                              , EvoOrientationAroundParent.Clone(), EvoRelativeOrientation.Clone(), EvoRadius.Clone(), EvoSweep.Clone()
                                              , myShape.Color.Clone(), myShape.DebugColor.Clone());
            return newEC;
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            EyeCluster newEC = new EyeCluster(newParent, Name, IncludeColor
                                              , EvoOrientationAroundParent.Evolve(), EvoRelativeOrientation.Evolve(), EvoRadius.Evolve(), EvoSweep.Evolve()
                                              , myShape.Color.Clone(), myShape.DebugColor.Clone());
            return newEC;
        }

        public Dictionary<string,string> ExportEvoNumbersAsCode()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            PopulateCodeDictionary(properties, "OrientationAroundParent", EvoOrientationAroundParent);
            PopulateCodeDictionary(properties, "RelativeOrientation", EvoRelativeOrientation);
            PopulateCodeDictionary(properties, "Radius", EvoRadius);
            PopulateCodeDictionary(properties, "Sweep", EvoSweep);

            return properties;
        }

        private void PopulateCodeDictionary(Dictionary<string, string> properties, string name, EvoNumber evo)
        {
            properties.Add(name, $", new ROEvoNumber(startValue: {evo.StartValue},\tevoDeltaMax: {evo.DeltaMax},\thardMin: {evo.ValueHardMin},\thardMax: {evo.ValueHardMax})\t//{name}");
        }
    }
}
