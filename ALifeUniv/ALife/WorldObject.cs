using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    //TODO: World Objects are current hardcodded to be "Circle". 
    public abstract class WorldObject : Circle, IHasShape
    {
        public int ExecutionOrder;
        private Point centre;
        public override Point CentrePoint
        {
            get { return centre; }

            set
            {
                //TODO: This stops agents from fallling off the world. But they don't know they're doing or not doing that.
                double properX = value.X;
                double properY = value.Y;

                if(value.X + Radius > Planet.World.WorldWidth)
                {
                    properX = Planet.World.WorldWidth - Radius;
                }
                if(value.X - Radius < 0)
                {
                    properX = Radius;
                }
                if(value.Y + Radius > Planet.World.WorldHeight)
                {
                    properY = Planet.World.WorldHeight - Radius;
                }
                if(value.Y - Radius < 0)
                {
                    properY = Radius;
                }
                centre.X = properX;
                centre.Y = properY;
            }
        }

        public override Angle Orientation
        {
            get;
            set;
        }

        public readonly String GenusLabel;
        public readonly String IndividualLabel;
        protected int numChildren = 0;

        private float radius;
        public override float Radius
        {
            get
            {
                return radius;
            }

            set
            {
                float rad = value;
                rad = Math.Clamp(rad, Settings.ObjectRadiusMin, Settings.ObjectRadiusMax);
                radius = rad;
            }
        }

        //TODO: Merge PropertyInput and StatisticInput into a single "Properties" cabinet
        public Dictionary<String, PropertyInput> Properties = new Dictionary<string, PropertyInput>();
        public Dictionary<String, StatisticInput> Statistics = new Dictionary<string, StatisticInput>();

        public readonly string CollisionLevel;

        public bool Alive;

        protected WorldObject(Point centrePoint, float startRadius, string genusLabel, string individualLabel, string collisionLevel, Color color)
            : base(centrePoint, startRadius)
        {
            CentrePoint = centrePoint;
            Radius = startRadius;
            GenusLabel = genusLabel;
            IndividualLabel = individualLabel;
            CollisionLevel = collisionLevel;
            Color = color;
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

        public IShape GetShape()
        {
            return this;
        }
    }
}
