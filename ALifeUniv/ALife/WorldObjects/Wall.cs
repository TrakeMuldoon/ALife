using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Objects
{
    public class Wall : WorldObject
    {
        public Wall(Point centrePoint, double Length, Angle orientation, string individualLabel)
            : base("Wall", individualLabel, ReferenceValues.CollisionLevelPhysical)
        {
            Shape = new Rectangle(Length, 5, Colors.DarkKhaki);
            Shape.CentrePoint = centrePoint;
            Shape.Orientation = orientation;
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteAliveTurn()
        {
            //Do nothing. It's a wall.
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
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
