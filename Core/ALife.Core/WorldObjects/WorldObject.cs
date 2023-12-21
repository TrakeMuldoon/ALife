using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects.Agents.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using Point = ALife.Core.Geometry.Shapes.Point;

namespace ALife.Core.WorldObjects
{
    /// <summary>
    /// The World Object class is the main class for anything which will exist in the world and has some sort of shape which will go on a collision grid.
    /// It is an abstract class because an "empty world" object cannot do anything in the world.
    /// All objects can be considered to have two (sort of three) states: Alive, Dead, and Removed
    /// Alive - Alive objects will execute their "Alive" turn behaviours every turn.
    /// Dead - Dead objects will execute their "Dead" turn behaviours every turn. Based on which state the object was in at the start of its turn.
    /// Removed - Once a "Dead" or alive object no longer has any actions to take, it will be "Removed" from the active grid, and will take no more turns.
    ///           The (C#) object may remain in memory and be tracked for other activities, but the object will not return to the collision grids. (sort of)
    /// </summary>
    public abstract class WorldObject : IHasShape
    {
        /// <summary>
        /// The Shape of the Object
        /// The shape contains the coordinates and most of the information about where the object is in the world.
        /// </summary>
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

        /// <summary>
        /// How to execute a turn for an object which is considered "alive". 
        /// </summary>
        public abstract void ExecuteAliveTurn();

        /// <summary>
        /// How to execute a turn for an object which is considered "dead". 
        /// </summary>
        public abstract void ExecuteDeadTurn();

        /// <summary>
        /// Execute any required steps to move the object from "Alive" -> "Dead" (or "Removed" if the object has no meaningful "Dead" state")
        /// </summary>
        public abstract void Die();

        /// <summary>
        /// Reproduce is the keyword for any object recreating an EVOLVED copy of itself, with some parameters or properties being modified. 
        /// The mechanism of evolution is up to the implementer
        /// </summary>
        /// <returns>An Evolved Object</returns>
        public abstract WorldObject Reproduce();

        /// <summary>
        /// Clone is the keyword for any object recreating an IDENTICAL copy of itself. The relevant behaviours and properties should be unchanged.
        /// </summary>
        /// <returns>A copy</returns>
        public abstract WorldObject Clone();

        /// <summary>
        /// This is how an object moves into the "Removed" state
        /// </summary>
        public virtual void TrashItem()
        {
            Planet.World.RemoveWorldObject(this);
        }
    }
}
