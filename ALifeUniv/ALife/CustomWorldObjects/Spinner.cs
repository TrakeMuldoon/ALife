using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Geometry;
using System;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.CustomWorldObjects
{
    class Spinner : WorldObject
    {
        public Spinner(Point centrePoint, IShape shape, string genusLabel, string individualLabel, string collisionLevel, Color color)
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
