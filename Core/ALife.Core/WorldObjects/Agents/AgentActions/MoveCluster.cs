using ALife.Core.Collision;
using ALife.Core.Geometry.New;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Maths;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.AgentActions
{
    public class MoveCluster : ActionCluster
    {
        private double Speed = 5; //TODO Abstract this into Scenario

        public MoveCluster(Agent self, InteractionFunction collisionBehaviour) : base(self, "Move", collisionBehaviour)
        {
            SubActions.Add("GoForward", new ActionPart("GoForward", Name));
            SubActions.Add("StopForward", new ActionPart("StopForward", Name));
            SubActions.Add("GoBackward", new ActionPart("GoBackward", Name));
            SubActions.Add("StopBackward", new ActionPart("StopBackward", Name));
            SubActions.Add("GoLeft", new ActionPart("GoLeft", Name));
            SubActions.Add("StopLeft", new ActionPart("StopLeft", Name));
            SubActions.Add("GoRight", new ActionPart("GoRight", Name));
            SubActions.Add("StopRight", new ActionPart("StopRight", Name));
        }

        public override ActionCluster CloneAction(Agent newParent)
        {
            return new MoveCluster(newParent, Interaction);
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
            dirValue = ExtraMath.Clamp(dirValue, 0.0, 1.0);
            return dirValue;
        }

        double forwardDist = -999;
        double rightDist = -999;

        private bool Move(double forwardMagnitude, double rightMagnitude)
        {
            IShape theShape = self.Shape;
            Geometry.Shapes.Point origin = new Geometry.Shapes.Point(theShape.CentrePoint.X, theShape.CentrePoint.Y);

            //Move forward, then move right from that Geometry.Shapes.Point
            forwardDist = Speed * forwardMagnitude;
            rightDist = Speed * rightMagnitude;

            Geometry.Shapes.Point tempPoint = GeometryMath.TranslateByVector(origin, theShape.Orientation, forwardDist);
            Geometry.Shapes.Point finalPoint = GeometryMath.TranslateByVector(tempPoint, theShape.Orientation.Radians + (Math.PI / 2), rightDist);

            double halfXLength = theShape.BoundingBox.XLength / 2;
            double halfYHeight = theShape.BoundingBox.YHeight / 2;
            finalPoint.X = ExtraMath.Clamp(finalPoint.X, halfXLength, Planet.World.WorldWidth - halfXLength);
            finalPoint.Y = ExtraMath.Clamp(finalPoint.Y, halfYHeight, Planet.World.WorldHeight - halfYHeight);

            theShape.CentrePoint = finalPoint;

            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[self.CollisionLevel];
            List<WorldObject> collisions = collider.DetectCollisions(self);

            //If there are no collisions, we propogate the move.
            if(collisions.Count == 0)
            {
                collider.MoveObject(self);
                return true;
            }
            else
            {
                theShape.CentrePoint = origin; //cancel the move
                Interaction(self, collisions);
                return false;
            }
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
