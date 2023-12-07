using ALifeUni.ALife.Agents;
using ALifeUni.ALife.Collision;
using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Utility.WorldObjects
{
    class FallingRock : WorldObject
    {

        private Zone start;
        private Zone finish;

        public FallingRock(Point centrePoint, IShape shape, Color color, Zone startZone, Zone targetZone)
            : base(centrePoint, shape, "Rock", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical, color)
        {
            Shape.Orientation = new Angle(90);
            start = startZone;
            finish = targetZone;
        }

        public FallingRock(Point centrePoint, IShape shape, Color color)
            : base(centrePoint, shape, "Rock", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical, color)
        {
            Shape.Orientation = new Angle(90);
        }

        public override WorldObject Clone()
        {
            throw new NotImplementedException();
        }

        public override void Die()
        {
            //TODO: Abstract this out
            this.Alive = false;
        }

        public override void ExecuteAliveTurn()
        {
            Point origin = new Point(Shape.CentrePoint.X, Shape.CentrePoint.Y);
            Angle turnRotation = new Angle(2);

            Point newCentre = ExtraMath.TranslateByVector(Shape.CentrePoint, Shape.Orientation, 10);
            Shape.CentrePoint = newCentre;

            Shape.Orientation += turnRotation;

            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[CollisionLevel];
            List<WorldObject> collisions = collider.DetectCollisions(this);
            if(collisions.Where((wo) => wo is FallingRock).Count() > 0)
            {
                Shape.CentrePoint = origin;
                Shape.Orientation -= turnRotation;
                return;
            }
            foreach(WorldObject crushed in collisions)
            {
                crushed.Die();
            }
            collider.MoveObject(this);

            /* This code made the falling rock zoom across the map and then reappear at the starting point
             * Could be used like a raindrop animation, if there were many of them */

            //List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(Shape.BoundingBox);
            //Zone z = inZones.Where((zone) => zone.Name == finish.Name).FirstOrDefault();
            //if(z != null)
            //{
            //    Point myPoint = start.Distributor.NextAgentCentre(Shape.BoundingBox.XLength, Shape.BoundingBox.YHeight);
            //    Shape.CentrePoint = myPoint;
            //    collider.MoveObject(this);
            //}
            Shape.Reset();
        }

        public override void ExecuteDeadTurn()
        {
            Planet.World.RemoveWorldObject(this);
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException();
        }
    }
}
