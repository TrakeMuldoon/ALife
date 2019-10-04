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
            Point origin = self.CentrePoint;
            double magnitude = Speed * IntensityPercent;

            double newX = (magnitude * Math.Cos(self.Orientation.Radians)) + origin.X;
            double newY = (magnitude * Math.Sin(self.Orientation.Radians)) + origin.Y;
            
            //Gravity!
            //newY = newY + 5;

            Point destination = new Point(newX, newY);
            //Note the agent has a bounding box, but it needs to be recalculated because this is their THEORETICAL bounding box, after they've moved.
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
