using ALife.Core.Collision;
using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects.Agents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ALife.Core.WorldObjects.Prebuilt
{
    class FallingRock : WorldObject
    {

        private Zone start;
        private Zone finish;

        public FallingRock(Geometry.Shapes.Point centrePoint, IShape shape, Colour color, Zone startZone, Zone targetZone)
            : base(centrePoint, shape, "Rock", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical, color)
        {
            Shape.Orientation = new Angle(90);
            start = startZone;
            finish = targetZone;
        }

        public FallingRock(Geometry.Shapes.Point centrePoint, IShape shape, Colour color)
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
            Geometry.Shapes.Point origin = new Geometry.Shapes.Point(Shape.CentrePoint.X, Shape.CentrePoint.Y);
            Angle turnRotation = new Angle(2);

            Geometry.Shapes.Point newCentre = GeometryMath.TranslateByVector(Shape.CentrePoint, Shape.Orientation, 10);
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

            /* This code made the falling rock zoom across the map and then reappear at the starting Geometry.Shapes.Point
             * Could be used like a raindrop animation, if there were many of them */

            //List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(Shape.BoundingBox);
            //Zone z = inZones.Where((zone) => zone.Name == finish.Name).FirstOrDefault();
            //if(z != null)
            //{
            //    Geometry.Shapes.Point myPoint = start.Distributor.NextAgentCentre(Shape.BoundingBox.XLength, Shape.BoundingBox.YHeight);
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
