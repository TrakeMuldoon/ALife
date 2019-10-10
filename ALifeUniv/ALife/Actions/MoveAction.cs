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
            Point origin = new Point(self.CentrePoint.X, self.CentrePoint.Y);
            double magnitude = Speed * IntensityPercent;

            double newX = (magnitude * Math.Cos(self.Orientation.Radians)) + origin.X;
            double newY = (magnitude * Math.Sin(self.Orientation.Radians)) + origin.Y;
            
            //Gravity!
            //newY = newY + 5;

            Point destination = new Point(newX, newY);
            self.CentrePoint = destination;

            ICollisionMap collider = Planet.World.CollisionLevels[self.CollisionLevel];
            List<WorldObject> collisions = collider.QueryForBoundingBoxCollisions(self.BoundingBox, self);

            //If there are no collisions, we propogate the move.
            //Otherwise, we reverse it, and turn red.
            if(collisions.Count == 0)
            {
                collider.MoveObject(self);
            }
            else
            {
                self.CentrePoint = origin;
                self.Color = Colors.Red;
            }
        }
    }
}
