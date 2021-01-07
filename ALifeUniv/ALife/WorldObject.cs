using ALifeUni.ALife.Shapes;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public abstract class WorldObject : IHasShape
    {
        public virtual IShape Shape
        {
            get;
            protected set;
        }

        public readonly String GenusLabel;
        public readonly String IndividualLabel;

        public int ExecutionOrder;
        public int NumChildren
        {
            get;
            protected set;
        }

        //TODO: Merge PropertyInput and StatisticInput into a single "Properties" cabinet
        public Dictionary<String, PropertyInput> Properties = new Dictionary<string, PropertyInput>();
        public Dictionary<String, StatisticInput> Statistics = new Dictionary<string, StatisticInput>();

        public string CollisionLevel
        {
            get;
            protected set;
        }
        public bool Alive;

        protected WorldObject(Point centrePoint, IShape shape, string genusLabel, string individualLabel, string collisionLevel, Color color)
        {
            NumChildren = 0;
            Shape = shape;
            Shape.Color = color;
            Shape.CentrePoint = centrePoint;

            GenusLabel = genusLabel;
            IndividualLabel = individualLabel;
            CollisionLevel = collisionLevel;
            Alive = true;
        }

        protected WorldObject(String genusLabel, string individualLabel, string collisionLevel)
        {
            GenusLabel = genusLabel;
            IndividualLabel = individualLabel;
            CollisionLevel = collisionLevel;
            NumChildren = 0;
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
