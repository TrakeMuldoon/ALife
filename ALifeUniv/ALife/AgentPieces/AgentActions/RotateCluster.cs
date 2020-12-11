using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public class RotateCluster : ActionCluster
    {
        public RotateCluster(Agent self) : base(self, "Rotate")
        {
            SubActions.Add("TurnLeft", new ActionPart("TurnLeft", Name));
            SubActions.Add("TurnRight", new ActionPart("TurnRight", Name));
        }

        public override ActionCluster CloneAction(Agent newParent)
        {
            return new RotateCluster(newParent);
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

        double netTurn = -999;

        protected override bool AttemptEnact()
        {

            double turnRight = SubActions["TurnRight"].Intensity;
            double turnLeft = SubActions["TurnLeft"].Intensity;

            double netRightTurnPercent = turnRight - turnLeft;
            netTurn = netRightTurnPercent * Settings.AgentMaximumTurnDegrees;

            Angle myOrientation = self.Shape.Orientation;
            myOrientation.Degrees += netTurn;

            if(!(self.Shape is Circle))
            {
                ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[self.CollisionLevel];
                List<WorldObject> collisions = collider.DetectCollisions(self);

                if(collisions.Count > 0)
                {
                    throw new Exception("Unhandled Collision state (rotation collision)");
                }
            }
            return true;
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
                return "Rotated at " + netTurn;
            }
            else
            {
                return "Unturned";
            }
        }
    }
}
