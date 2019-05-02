///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED
///////////////// NOT COMPILED

using ALifeUni.ALife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    class CollisionQuadTree : ICollisionMap
    {
        readonly int Height;
        readonly int Width;
        readonly BoundingBox location;

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
            BoundingBox itemSquare = new BoundingBox();
            itemSquare.MaxX = item.CentrePoint.X + item.Radius;
            itemSquare.MinX = item.CentrePoint.X - item.Radius;
            itemSquare.MaxY = item.CentrePoint.Y + item.Radius;
            itemSquare.MinY = item.CentrePoint.Y - item.Radius;

            if(location.IsCollision(itemSquare))
            {
                //if I don't have children
                    //Place child in my list
                    //if I have more than 10 objects
                        //split
                //else
                    //place child in my list
                    //instert against all four childs
                    //Place in the dictionary against those lists as well
                    
            }
            else
            {
                //It doesn't collide, doesn't belong here.
                return false;
            }
            return true;
        }

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
