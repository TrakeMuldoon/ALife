using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    //TODO: World Objects are current hardcodded to be "Circle". 
    public abstract class WorldObject : IHasShape
    {
        //private Point centre;
        //public Point CentrePoint
        //{
        //    get { return centre; }

        //    set
        //    {
        //        //TODO: This stops agents from fallling off the world. But they don't know they're doing or not doing that.
        //        double diffX = value.X - centre.X;
        //        double diffY = value.Y - centre.Y;

        //        BoundingBox bb = Shape.BoundingBox;
        //        double halfBBX = (bb.MaxX - bb.MinX) / 2;
        //        double halfBBY = (bb.MaxY - bb.MinY) / 2;
        //        centre.X = Math.Clamp(value.X, halfBBX, Planet.World.WorldWidth - halfBBX);
        //        centre.Y = Math.Clamp(value.Y, halfBBY, Planet.World.WorldHeight - halfBBY);
        //    }
        //}

        public IShape Shape
        {
            get;
            private set;
        }

        public readonly String GenusLabel;
        public readonly String IndividualLabel;

        public int ExecutionOrder;
        protected int numChildren = 0;

        //TODO: Merge PropertyInput and StatisticInput into a single "Properties" cabinet
        public Dictionary<String, PropertyInput> Properties = new Dictionary<string, PropertyInput>();
        public Dictionary<String, StatisticInput> Statistics = new Dictionary<string, StatisticInput>();

        public readonly string CollisionLevel;
        public bool Alive;

        protected WorldObject(Point centrePoint, IShape shape, string genusLabel, string individualLabel, string collisionLevel, Color color)
        {
            Shape = shape;
            Shape.Color = color;
            Shape.CentrePoint = centrePoint;

            GenusLabel = genusLabel;
            IndividualLabel = individualLabel;
            CollisionLevel = collisionLevel;
            Alive = true;
        }

        /* METHODS */
        public virtual void ExecuteTurn()
        {
            if(Alive)
            {
                ExecuteAliveTurn();
            }
            else
            {
                ExecuteDeadTurn();
            }
        }

        public abstract void ExecuteAliveTurn();
        public abstract void ExecuteDeadTurn();

        public abstract void Die();

        public abstract WorldObject Reproduce();

        public abstract WorldObject Clone();

        public virtual void TrashItem()
        {
            Planet.World.RemoveWorldObject(this);
        }
    }
}
