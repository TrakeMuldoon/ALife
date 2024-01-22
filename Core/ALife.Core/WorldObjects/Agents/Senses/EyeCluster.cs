using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility;
using ALife.Core.WorldObjects.Agents.Senses.Eyes;
using ALife.Core.WorldObjects.Agents.Senses.GenericInputs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ALife.Core.WorldObjects.Agents.Senses
{
    public class EyeCluster : SenseCluster
    {
        ChildSector myShape;
        public override IShape Shape
        {
            get { return myShape; }
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

            //Re: "TODO: HUUGE BUUG." The code there is setting the distance for the root Geometry.Shapes.Point of the childSector (the vision radius of the eye) 
            //to 5.0 away from the parent center Geometry.Shapes.Point. This is a bug, because if the parent radius is modified to be larger than the that distancce, then the eye will be inside it.
            //The reason we can't set it to be "parent.Shape.Radius" is that the parent is not guaranteed to be a circle.
            //Technically, there are two "solves" for this. 
            // 1. Do some sort of math to determine based on a 360 degree orientation, the distance to the shape perimeter at that point. 
            //      This has some significant drawbacks, the shapes must be concave. Otherwise, there will be angles where the function would 
            //      Have multiple results. 
            // 2. Just assume that eyes don't need to be directly on the perimeter of the agent, which implies a more 3D model. This still has 
            //      Some of the same difficulties as the other solution, because we still need to ensure that the eyes are not external to the agent.
            // 3. Just let the eyes be anywhere. 
            //      Simple. No math problems here. The eyes will be 5 units from the agent centre, at any orientation.
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
