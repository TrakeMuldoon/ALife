using ALife.Core.Collision;
using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects.Agents.Senses;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ALife.Core.WorldObjects.Agents.AgentActions
{
    public class EatCluster : ActionCluster, IHasShape
    {
        public IShape Shape { get; set; }

        public static readonly ReadOnlyEvoNumber DefaultOrientationAroundParent = new ReadOnlyEvoNumber(startValue: -40, evoDeltaMax: 0, hardMin: -360, hardMax: 360);
        public static readonly ReadOnlyEvoNumber DefaultRelativeOrientation = new ReadOnlyEvoNumber(startValue: 0, evoDeltaMax: 0, hardMin: -360, hardMax: 360);
        public static readonly ReadOnlyEvoNumber DefaultRadius = new ReadOnlyEvoNumber(startValue: 10, evoDeltaMax: 0, hardMin: 40, hardMax: 120);
        public static readonly ReadOnlyEvoNumber DefaultSweep = new ReadOnlyEvoNumber(startValue: 80, evoDeltaMax: 0, hardMin: 15, hardMax: 40);

        public EatCluster(Agent self, InteractionFunction eatSomethingBehaviour) : base(self, "Eat", eatSomethingBehaviour)
        {
            SubActions.Add("EatThing", new ActionPart("EatThing", Name));


            Angle orientationAroundParent = new Angle(DefaultOrientationAroundParent.OriginalValue);
            Angle relativeOrientation = new Angle(DefaultRelativeOrientation.OriginalValue);
            float radius = (float)DefaultRadius.OriginalValue;
            Angle sweep = new Angle(DefaultSweep.OriginalValue);

            Shape = new ChildSector(self.Shape
                          , orientationAroundParent
                          , 5.0 
                          , relativeOrientation, radius, sweep);
        }

        public override ActionCluster CloneAction(Agent newParent)
        {
            return new EatCluster(newParent, Interaction);
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
                if(ap.Intensity >= 0.5)
                {
                    return true;
                }
            }
            return false;
        }

        double targets = 0;

        protected override bool AttemptEnact()
        {
            if (SubActions["EatThing"].Intensity >= 0.5)
            {
                Shape.Reset();
                ICollisionMap<WorldObject> CollisionMap = Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical];
                List<WorldObject> collisions = CollisionMap.DetectCollisions(this, self);
                targets = collisions.Count;
                Interaction(self, collisions);
                return true;
            }
            targets = 0;
            return false;
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
                return "Tried to eat from ";
            }
            else
            {
                return "Nothing";
            }
        }
    }
}
