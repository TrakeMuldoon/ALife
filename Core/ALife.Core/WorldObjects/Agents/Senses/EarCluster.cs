using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility;
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

        private EvoNumber EvoOrientationAroundParent;
        private EvoNumber EvoRadius;

        public EarCluster(WorldObject parent, String name
                          , EvoNumber eOrientationAroundParent, EvoNumber eRadius)
            : base(parent, name)
        {
            EvoOrientationAroundParent = eOrientationAroundParent;
            EvoRadius = eRadius;

            Angle orientationAroundParent = new Angle(eOrientationAroundParent.StartValue);
            float radius = (float)eRadius.StartValue;
            myShape = new ChildCircle(parent.Shape, orientationAroundParent
                                      , 5.0 //TODO: Similar to EyeCluster Issue. This is a hack.
                                      , radius);
            SubInputs.Add(new AnyInput(name + ".HearSomething"));
            //SubInputs.Add(new )
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            EarCluster newEC = new EarCluster(newParent, Name
                                              , EvoOrientationAroundParent.Clone(), EvoRadius.Clone());
            return newEC;
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            EarCluster newEC = new EarCluster(newParent, Name
                                              , EvoOrientationAroundParent.Evolve(), EvoRadius.Evolve());
            return newEC;
        }

        public Dictionary<string, string> ExportEvoNumbersAsCode()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            PopulateCodeDictionary(properties, "OrientationAroundParent", EvoOrientationAroundParent);
            PopulateCodeDictionary(properties, "Radius", EvoRadius);

            return properties;
        }

        private void PopulateCodeDictionary(Dictionary<string, string> properties, string name, EvoNumber evo)
        {
            properties.Add(name, $", new ROEvoNumber(startValue: {evo.StartValue},\tevoDeltaMax: {evo.DeltaMax},\thardMin: {evo.ValueHardMin},\thardMax: {evo.ValueHardMax})\t//{name}");
        }
    }
}
