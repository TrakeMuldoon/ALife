using ALifeUni.ALife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUni.ALife.Componetns
{
    class CollisionQuadTree
    {
        readonly int Height;
        readonly int Width;
        readonly Square location;

        int minSegmentSize;
        int maxEntities;

        List<WorldObject> insideObjects = new List<WorldObject>();
        CollisionQuadTree[] children;
        Dictionary<WorldObject, List<CollisionQuadTree>> objectsToChildren; 

        public bool isEmpty
        {
            get
            {
                return insideObjects.Count == 0;
            }
       
        }

        bool hasChildren;

        public CollisionQuadTree(int MaxXPosition, int MinXPosition, int MaxYPosition, int MinYPosition)
        {
            location.MaxX = MaxXPosition;
            location.MinX = MinXPosition;
            location.MaxY = MaxYPosition;
            location.MinY = MinYPosition;

            hasChildren = false;
        }

        public bool Insert(WorldObject item)
        {
            Square itemSquare = new Square();
            itemSquare.MaxX = item.CentrePoint.X + item.Radius;
            itemSquare.MinX = item.CentrePoint.X - item.Radius;
            itemSquare.MaxY = item.CentrePoint.Y + item.Radius;
            itemSquare.MinY = item.CentrePoint.Y - item.Radius;

            if(location.IsCollision(itemSquare))
            {
                
            }
            else
            {
                //It doesn't collide, doesn't belong here.
                return false;
            }
        }

        //Insert
        //Query //return List<WorldObject> potential collisions
        //Does cube collide with me
        //Yes.
        //Do I have children?
        //yes.
        //Check children 

        //Else go home, you failed
        //RemoveAll
        //MoveObject

        //Split
        //AbsorbChildren



    }
}
