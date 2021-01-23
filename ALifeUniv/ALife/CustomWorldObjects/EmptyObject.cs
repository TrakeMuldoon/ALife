using ALifeUni.ALife.Shapes;
using System;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.CustomWorldObjects
{
    public class EmptyObject : WorldObject
    {
        public EmptyObject(Point centrePoint, float startRadius, string collisionLevel) : this(centrePoint, startRadius, collisionLevel, String.Empty)
        {
        }
        public EmptyObject(Point centrePoint, float startRadius, string collisionLevel, string name)
            : base(centrePoint, new Circle(centrePoint, startRadius), "Empty", name, collisionLevel, Colors.Gray)
        {
        }
        public EmptyObject(IShape shape, string collisionLevel)
            : base("Empty", string.Empty, collisionLevel)
        {
            Shape = shape;
        }

        public override void Die()
        {
            this.Alive = false;
        }

        public override void ExecuteAliveTurn()
        {
            //Do Nothing
        }

        public override void ExecuteDeadTurn()
        {
            Planet.World.RemoveWorldObject(this);
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Clone()
        {
            throw new NotImplementedException();
        }
    }
}
