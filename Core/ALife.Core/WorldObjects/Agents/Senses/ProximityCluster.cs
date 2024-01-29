using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects.Agents.Senses.GenericInputs;
using System.Collections.Generic;
using ALife.Core.GeometryOld;
using ALife.Core.GeometryOld.Shapes;
using ALife.Core.GeometryOld.Shapes.ChildShapes;

namespace ALife.Core.WorldObjects.Agents.Senses
{
    public class ProximityCluster : SenseCluster
    {
        private IEvoNumber evoRadius;
        private ChildCircle myShape;
        public override IShape Shape
        {
            get
            {
                return myShape;
            }
        }

        public ProximityCluster(WorldObject parent, string name, IEvoNumber radius)
            : base(parent, name)
        {
            evoRadius = radius;

            myShape = new ChildCircle(parent.Shape, new Angle(0), 0, (float)radius.OriginalValue);

            SubInputs.Add(new AnyInput(name + ".SomethingClose"));
        }
        public ProximityCluster(WorldObject parent, string name, IEvoNumber radius, Colour newColor)
            : this(parent, name, radius)
        {
            myShape.Colour = newColor;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, evoRadius.Evolve(Planet.World.NumberGen), (Colour)myShape.Colour.Clone());
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            return new ProximityCluster(newParent, Name, evoRadius.Evolve(Planet.World.NumberGen), (Colour)myShape.Colour.Clone());
        }

        public Dictionary<string, string> ExportEvoNumbersAsCode()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            PopulateCodeDictionary(properties, "Radius", evoRadius);

            return properties;
        }

        private void PopulateCodeDictionary(Dictionary<string, string> properties, string name, IEvoNumber evo)
        {
            properties.Add(name, $", new ReadOnlyEvoNumber(startValue: {evo.OriginalValue},\tevoDeltaMax: {evo.ValueDeltaMaximumValue},\thardMin: {evo.ValueAbsoluteMinimum},\thardMax: {evo.ValueAbsoluteMaximum})\t//{name}");
        }
    }
}
