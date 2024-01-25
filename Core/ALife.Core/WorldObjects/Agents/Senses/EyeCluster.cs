using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects.Agents.Senses.Eyes;
using ALife.Core.WorldObjects.Agents.Senses.GenericInputs;
using System;
using System.Collections.Generic;

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
        private IEvoNumber EvoOrientationAroundParent;
        private IEvoNumber EvoRelativeOrientation;
        private IEvoNumber EvoRadius;
        private IEvoNumber EvoSweep;

        public EyeCluster(WorldObject parent, String name, bool withColour
                          , IEvoNumber eOrientationAroundParent, IEvoNumber eRelativeOrientation, IEvoNumber eRadius, IEvoNumber eSweep)
            : base(parent, name)
        {
            IncludeColor = withColour;
            EvoOrientationAroundParent = eOrientationAroundParent;
            EvoRelativeOrientation = eRelativeOrientation;
            EvoRadius = eRadius;
            EvoSweep = eSweep;

            Angle orientationAroundParent = new Angle(eOrientationAroundParent.OriginalValue);
            Angle relativeOrientation = new Angle(eRelativeOrientation.OriginalValue);
            float radius = (float)eRadius.OriginalValue;
            Angle sweep = new Angle(eSweep.OriginalValue);
            myShape = new ChildSector(parent.Shape, orientationAroundParent
                                      , 5.0 //TODO: HUUUUUUGE BUG. See explanation below
                                      , relativeOrientation, radius, sweep);
            SubInputs.Add(new AnyInput(name + ".SeeSomething"));
            //SubInputs.Add(new CountInput(name + ".HowMany"));
            //SubInputs.Add(new EyeIdentifierInput(name + ".WhoISee"));
            if(IncludeColor)
            {
                SubInputs.Add(new ColorBoolInput(name + ".IsRed", (WorldObject wo) => wo.Shape.Colour.R));
                SubInputs.Add(new ColorBoolInput(name + ".IsBlue", (WorldObject wo) => wo.Shape.Colour.B));
                SubInputs.Add(new ColorBoolInput(name + ".IsGreen", (WorldObject wo) => wo.Shape.Colour.G));
                //SubInputs.Add(new ColorInput(name + ".HowRed", (WorldObject wo) => wo.Shape.Colour.R));
                //SubInputs.Add(new ColorInput(name + ".HowBlue", (WorldObject wo) => wo.Shape.Colour.B));
                //SubInputs.Add(new ColorInput(name + ".HowGreen", (WorldObject wo) => wo.Shape.Colour.G));
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
                  , IEvoNumber eOrientationAroundParent, IEvoNumber eRelativeOrientation, IEvoNumber eRadius, IEvoNumber eSweep)
            : this(parent, name, false
                  , eOrientationAroundParent, eRelativeOrientation, eRadius, eSweep)
        { }


        public EyeCluster(WorldObject parent, String name, bool includeColor
                          , IEvoNumber eOrientationAroundParent, IEvoNumber eRelativeOrientation, IEvoNumber eRadius, IEvoNumber eSweep
                          , Colour myColor, Colour myDebugColor)
            : this(parent, name, includeColor
                  , eOrientationAroundParent, eRelativeOrientation, eRadius, eSweep)
        {
            this.myShape.DebugColour = myDebugColor;
            this.myShape.Colour = myColor;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            EyeCluster newEC = new EyeCluster(newParent, Name, IncludeColor
                                              , EvoOrientationAroundParent.Clone(), EvoRelativeOrientation.Clone(), EvoRadius.Clone(), EvoSweep.Clone()
                                              , (Colour)myShape.Colour.Clone(), (Colour)myShape.DebugColour.Clone());
            return newEC;
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            EyeCluster newEC = new EyeCluster(newParent, Name, IncludeColor
                                              , EvoOrientationAroundParent.Evolve(Planet.World.NumberGen), EvoRelativeOrientation.Evolve(Planet.World.NumberGen), EvoRadius.Evolve(Planet.World.NumberGen), EvoSweep.Evolve(Planet.World.NumberGen)
                                              , (Colour)myShape.Colour.Clone(), (Colour)myShape.DebugColour.Clone());
            return newEC;
        }

        public Dictionary<string, string> ExportEvoNumbersAsCode()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            PopulateCodeDictionary(properties, "OrientationAroundParent", EvoOrientationAroundParent);
            PopulateCodeDictionary(properties, "RelativeOrientation", EvoRelativeOrientation);
            PopulateCodeDictionary(properties, "Radius", EvoRadius);
            PopulateCodeDictionary(properties, "Sweep", EvoSweep);

            return properties;
        }

        private void PopulateCodeDictionary(Dictionary<string, string> properties, string name, IEvoNumber evo)
        {
            properties.Add(name, $", new ReadOnlyEvoNumber(startValue: {evo.OriginalValue},\tevoDeltaMax: {evo.ValueDeltaMaximumValue},\thardMin: {evo.ValueAbsoluteMinimum},\thardMax: {evo.ValueAbsoluteMaximum})\t//{name}");
        }
    }
}
