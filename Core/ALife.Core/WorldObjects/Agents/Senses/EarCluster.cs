using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects.Agents.Senses.Ears;
using ALife.Core.WorldObjects.Agents.Senses.GenericInputs;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses
{
    public class EarCluster : SenseCluster
    {
        ChildCircle myShape;
        public override IShape Shape
        {
            get { return myShape; }
        }

        private IEvoNumber EvoOrientationAroundParent;
        private IEvoNumber EvoRadius;

        public EarCluster(WorldObject parent, String name
                          , IEvoNumber eOrientationAroundParent, IEvoNumber eRadius)
            : base(parent, name, ReferenceValues.CollisionLevelSound)
        {
            EvoOrientationAroundParent = eOrientationAroundParent;
            EvoRadius = eRadius;

            Angle orientationAroundParent = new Angle(eOrientationAroundParent.OriginalValue);
            float radius = (float)eRadius.OriginalValue;
            myShape = new ChildCircle(parent.Shape, orientationAroundParent
                                      , 5.0 //TODO: Similar to EyeCluster Issue. This is a hack.
                                      , radius);
            SubInputs.Add(new AnyInput(name + ".HearSomething"));
            SubInputs.Add(new SoundVolume(name + ".HowLoud", myShape));
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            EarCluster newEC = new EarCluster(newParent, Name
                                              , (EvoNumber)EvoOrientationAroundParent.Clone(), (EvoNumber)EvoRadius.Clone());
            return newEC;
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            EarCluster newEC = new EarCluster(newParent, Name
                                              , (EvoNumber)EvoOrientationAroundParent.Evolve(Planet.World.NumberGen), (EvoNumber)EvoRadius.Evolve(Planet.World.NumberGen));
            return newEC;
        }

        public Dictionary<string, string> ExportEvoNumbersAsCode()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            PopulateCodeDictionary(properties, "OrientationAroundParent", EvoOrientationAroundParent);
            PopulateCodeDictionary(properties, "Radius", EvoRadius);

            return properties;
        }

        private void PopulateCodeDictionary(Dictionary<string, string> properties, string name, IEvoNumber evo)
        {
            properties.Add(name, $", new ReadOnlyEvoNumber(startValue: {evo.OriginalValue},\tevoDeltaMax: {evo.ValueDeltaMaximumValue},\thardMin: {evo.ValueAbsoluteMinimum},\thardMax: {evo.ValueAbsoluteMaximum})\t//{name}");
        }
    }
}
