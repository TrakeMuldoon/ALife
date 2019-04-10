using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace AlifeUniversal.ALife
{
    public abstract class WorldObject
    {
        public Point CentrePoint;

        public String GenusLabel;
        public String IndividualLabel;

        private double radius;
        public double Radius
        {
            get
            {
                return radius;
            }

            set
            {
                double rad = value;
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

        public Dictionary<String, PropertyInput> Properties = new Dictionary<string, PropertyInput>();

        public readonly String CollisionLevel;

        public bool Alive;

        public Color color; 

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
            Environment.World.RemoveWorldObject(this);
        }
    }
}
