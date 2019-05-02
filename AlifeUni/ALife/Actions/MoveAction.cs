using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    class MoveAction : Action
    {
        private double Speed = Settings.AgentDefaultSpeed;

        public MoveAction(Agent myself) : base(myself)
        {
        
        }

        protected override void TakeAction(double IntensityPercent)
        {
            Coordinate origin = self.CentrePoint;

            float newX = (float)(Speed / 100 * IntensityPercent * Math.Cos(self.OrientationInRads)) + origin.X;
            float newY = (float)(Speed / 100 * IntensityPercent * Math.Sin(self.OrientationInRads)) + origin.Y;

            //Gravity!
            newY = newY + 5;

            Coordinate destination = new Coordinate(newX, newY);
            BoundingBox destBoundingBox = new BoundingBox(destination.X - self.Radius, destination.Y - self.Radius, destination.X + self.Radius, destination.Y + self.Radius);

            ICollisionMap collider = Planet.World.CollisionLevels[self.CollisionLevel];
            List<WorldObject> collisions = collider.QueryForBoundingBoxCollisions(destBoundingBox, self);

            if (collisions.Count == 0)
            {
                self.CentrePoint = destination;
                collider.MoveObject(self);
            }
            else
            {
                self.Color = Colors.Red;
            }
        }
    }
}
