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

        public override string Name
        {
            get
            {
                return "Move";
            }
        }

        protected override bool AttemptSuccessful()
        {
            return true;
        }

        protected override void TakeAction(double IntensityPercent)
        {
            Point origin = new Point(self.CentrePoint.X, self.CentrePoint.Y);
            double magnitude = Speed * IntensityPercent;

            double newX = (magnitude * Math.Cos(self.Orientation.Radians)) + origin.X;
            double newY = (magnitude * Math.Sin(self.Orientation.Radians)) + origin.Y;
            
            //Gravity!
            //newY = newY + 3;

            Point destination = new Point(newX, newY);
            self.CentrePoint = destination;

            ICollisionMap collider = Planet.World.CollisionLevels[self.CollisionLevel];
            List<WorldObject> collisions = collider.QueryForBoundingBoxCollisions(self.BoundingBox, self);
            collisions = CollisionDetector.FineGrainedCollisionDetection(collisions, self);

            //If there are no collisions, we propogate the move.
            if(collisions.Count == 0)
            {
                collider.MoveObject(self);
            }
            else
            {
                //TODO: Somehow abstract out "Collision behaviour"

                //Collision means death right now
                foreach(WorldObject wo in collisions)
                {
                    wo.Die();
                }
                //self must die, but also stop the movement from taking place. 
                //Also, we change colour of self, to see who bumped into whom.
                self.Die();
                self.DebugColor = Colors.Red;
            }
        }
    }
}
