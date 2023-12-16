using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using System.Drawing;

namespace ALife.Core.WorldObjects.Prebuilt
{
    class Spinner : WorldObject
    {
        public Spinner(Geometry.Shapes.Point centrePoint, IShape shape, string genusLabel, string individualLabel, string collisionLevel, Color color)
            : base(centrePoint, shape, genusLabel, individualLabel, collisionLevel, color)
        {
        }

        public override WorldObject Clone()
        {
            throw new NotImplementedException();
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteAliveTurn()
        {
            Shape.Orientation += new Angle(12);
            Shape.Reset();
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException();
        }
    }
}
