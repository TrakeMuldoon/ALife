using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.WorldObjects
{
    class FallingRock : WorldObject
    {
        public FallingRock(Point centrePoint, IShape shape, Color color)
            : base(centrePoint, shape, "Rock", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical, color)
        {
            Shape.Orientation = new Angle(90);
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
            Point newCentre = ExtraMath.TranslateByVector(Shape.CentrePoint, 0, (11 - Shape.BoundingBox.XLength) / 10 * 5);
            Shape.CentrePoint = newCentre;
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[CollisionLevel];
            collider.MoveObject(this);


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
