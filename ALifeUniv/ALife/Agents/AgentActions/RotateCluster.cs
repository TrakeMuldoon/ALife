using ALifeUni.ALife.Collision;
using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using System.Collections.Generic;

namespace ALifeUni.ALife.Agents.AgentActions
{
    public class RotateCluster : ActionCluster
    {
        const int MAXIMUM_TURN_DEGREES = 70;  //TODO: Abstract this into a config or the scenario

        public RotateCluster(Agent self) : base(self, "Rotate")
        {
            SubActions.Add("TurnLeft", new ActionPart("TurnLeft", Name));
            SubActions.Add("TurnRight", new ActionPart("TurnRight", Name));
            //SubActions.Add("TurnAround", new ActionPart("TurnAround", Name));
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
            //double turnAround = SubActions["TurnAround"].Intensity;

            double netRightTurnPercent = turnRight - turnLeft;

            //if(turnAround > Math.Abs(netRightTurnPercent))
            //{
            //    netTurn = 180;
            //}
            //else
            //{
            //    netTurn = netRightTurnPercent * MAXIMUM_TURN_DEGREES;
            //}

            netTurn = netRightTurnPercent * MAXIMUM_TURN_DEGREES; //TODO: Abstract this into a config or the scenario

            Angle myOrientation = self.Shape.Orientation;
            myOrientation.Degrees += netTurn;


            if(self.Shape is Circle)
            {
                //This if statement is here because circles don't change their collision profile when they rotate,
                return true;
            }

            // But other shapes do. 
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
                myOrientation.Degrees -= netTurn; //cancel the move
                self.CollisionBehvaviour(collisions);
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
                return "Rotated at " + netTurn;
            }
            else
            {
                return "Unturned";
            }
        }
    }
}
