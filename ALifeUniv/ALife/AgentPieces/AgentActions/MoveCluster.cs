using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class MoveCluster : ActionCluster
    {
        private double Speed = Settings.AgentDefaultSpeed;

        public MoveCluster(Agent self) : base(self, "Move")
        {
            SubActions.Add("GoForward", new ActionPart("GoForward", Name));
            SubActions.Add("StopForward", new ActionPart("StopForward", Name));
            SubActions.Add("GoBackward", new ActionPart("GoBackward", Name));
            SubActions.Add("StopBackward", new ActionPart("StopBackward", Name));
            SubActions.Add("GoLeft",new ActionPart("GoLeft", Name));
            SubActions.Add("StopLeft",new ActionPart("StopLeft", Name));
            SubActions.Add("GoRight",new ActionPart("GoRight", Name));
            SubActions.Add("StopRight",new ActionPart("StopRight", Name));
        }
        protected override bool ValidatePreconditions()
        {
            //TODO: Draw this from Config
            return true;
        }
        protected override bool SubActionsEngaged()
        {
            foreach(ActionPart ap in SubActions.Values)
            {
                if(ap.Intensity != 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool AttemptEnact()
        {
            double fwd = GetDirectionValue("Forward");
            double back = GetDirectionValue("Backward");
            double left = GetDirectionValue("Left");
            double right = GetDirectionValue("Right");

            //figure out vector
            double forwardVector = fwd - back;
            double rightVector = right - left;
            bool moveResult = Move(forwardVector, rightVector);
            return moveResult;
        }

        private double GetDirectionValue(string direction)
        {
            string go = "Go" + direction;
            string stop = "Stop" + direction;

            double dirValue = SubActions[go].Intensity - SubActions[stop].Intensity;
            dirValue = Math.Clamp(dirValue, 0.0, 1.0);
            return dirValue;
        }

        double forwardDist = -999;
        double rightDist = -999;

        private bool Move(double forwardMagnitude, double rightMagnitude)
        {
            Point origin = new Point(self.CentrePoint.X, self.CentrePoint.Y);
            
            //Move forward, then move right from that point
            forwardDist = Speed * forwardMagnitude;
            double tempX = (forwardDist * Math.Cos(self.Orientation.Radians)) + origin.X;
            double tempY = (forwardDist * Math.Sin(self.Orientation.Radians)) + origin.Y;

            rightDist = Speed * rightMagnitude;
            double newX = (forwardDist * Math.Cos(self.Orientation.Radians + (Math.PI / 2))) + tempX;
            double newY = (forwardDist * Math.Sin(self.Orientation.Radians + (Math.PI / 2))) + tempY;

            Point destination = new Point(newX, newY);
            self.CentrePoint = destination;

            ICollisionMap collider = Planet.World.CollisionLevels[self.CollisionLevel];
            List<WorldObject> collisions = collider.QueryForBoundingBoxCollisions(self.BoundingBox, self);
            collisions = CollisionDetector.FineGrainedCollisionDetection(collisions, self);

            //If there are no collisions, we propogate the move.
            if(collisions.Count == 0)
            {
                collider.MoveObject(self);
                return true;
            }
            else
            {
                CollisionBehvaviour(collisions);
                return false;
            }
        }

        private void CollisionBehvaviour(List<WorldObject> collisions)
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


        protected override void FailureResults()
        {
            //TODO: Draw this from Config
        }

        protected override void SuccessResults()
        {
            //TODO: Draw this from Config
        }

        public override string LastTurnString()
        {
            if(ActivatedLastTurn)
            {
                return "Moved (" + forwardDist + "," + rightDist + ")";
            }
            else
            {
                return "No Move";
            }
        }
    }
}
