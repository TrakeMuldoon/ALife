using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public abstract class WorldObject
    {
        private Coordinate centre;
        public Coordinate CentrePoint
        {
            get { return centre; }

            set
            {
                float properX = value.X;
                float properY = value.Y;

                if (value.X + Radius > Planet.World.WorldWidth)
                {
                    properX = Planet.World.WorldWidth - Radius;
                }
                if (value.X - Radius < 0)
                {
                    properX = Radius;
                }
                if (value.Y + Radius > Planet.World.WorldHeight)
                {
                    properY = Planet.World.WorldHeight - Radius;
                }
                if (value.Y - Radius < 0)
                {
                    properY = Radius;
                }
                centre = new Coordinate(properX, properY);
            }
        }

        public readonly String GenusLabel;
        public readonly String IndividualLabel;

        private float radius;
        public float Radius
        {
            get
            {
                return radius;
            }

            set
            {
                float rad = value;
                if(rad > Settings.ObjectRadiusMax)
                {
                    rad = Settings.ObjectRadiusMax;
                }
                else if (rad < Settings.ObjectRadiusMin)
                {
                    rad = Settings.ObjectRadiusMin;
                }
                radius = rad;
            }
        }

        public BoundingBox Boundary
        {
            get
            {
                return new BoundingBox(CentrePoint.X - Radius, CentrePoint.Y - Radius, CentrePoint.X + Radius, CentrePoint.Y + Radius);
            }
        }

        public Dictionary<String, PropertyInput> Properties = new Dictionary<string, PropertyInput>();

        public readonly string CollisionLevel;

        public bool Alive;

        public Color Color; 

        protected WorldObject(Coordinate centrePoint, float startRadius, string genusLabel, string individualLabel, string collisionLevel, Color color)
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
            if (!Alive)
            {
                ExecuteDeadTurn();
            }
            else
            {
                ExecuteAliveTurn();
            }
        }

        public abstract void ExecuteAliveTurn();
        public abstract void ExecuteDeadTurn();

        public virtual void TrashItem()
        {
            Planet.World.RemoveWorldObject(this);
        }
    }
}
