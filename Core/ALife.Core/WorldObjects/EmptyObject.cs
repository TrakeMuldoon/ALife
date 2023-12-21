﻿using ALife.Core.Geometry.Shapes;
using System;

namespace ALife.Core.WorldObjects
{
    public class EmptyObject : WorldObject
    {
        public EmptyObject(Geometry.Shapes.Point centrePoint, float startRadius, string collisionLevel) : this(centrePoint, startRadius, collisionLevel, String.Empty)
        {
        }
        public EmptyObject(Geometry.Shapes.Point centrePoint, float startRadius, string collisionLevel, string name)
            : base(centrePoint, new Circle(centrePoint, startRadius), "Empty", name, collisionLevel, System.Drawing.Color.Gray)
        {
        }
        public EmptyObject(IShape shape, string collisionLevel)
            : base("Empty", string.Empty, collisionLevel)
        {
            Shape = shape;
        }

        public void SetShape(IShape newShape)
        {
            Shape = newShape;
        }

        public override void Die()
        {
            this.Alive = false;
        }

        public override void ExecuteAliveTurn()
        {
            //Do Nothing
        }

        public override void ExecuteDeadTurn()
        {
            Planet.World.RemoveWorldObject(this);
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Clone()
        {
            throw new NotImplementedException();
        }
    }
}
