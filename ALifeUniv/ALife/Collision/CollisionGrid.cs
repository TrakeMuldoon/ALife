using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    class CollisionGrid : ICollisionMap
    {
        public readonly int Height;
        public readonly int Width;

        //TODO: Load from Default Configuration
        private int GridSize = 25;
        private List<WorldObject>[,] objectGrid;
        private List<WorldObject> trackedObjects;
        private Dictionary<WorldObject, List<Point>> agentLocationTracker = new Dictionary<WorldObject, List<Point>>();

        public CollisionGrid(int gridHeight, int gridWidth)
        {
            if(gridHeight < GridSize)
            {
                GridSize = gridHeight;
            }
            if(gridWidth < GridSize)
            {
                GridSize = gridWidth;
            }

            Height = gridHeight;
            Width = gridWidth;

            double numXBoxes = (Width + 1) / (double)GridSize;
            double numYBoxes = (Height + 1) / (double)GridSize;
            int numXBoxesCeil = (int)Math.Ceiling(numXBoxes);
            int numYBoxesCeil = (int)Math.Ceiling(numYBoxes);
            int numXBoxesFloor = (int)Math.Floor(numXBoxes);
            int numYBoxesFloor = (int)Math.Floor(numYBoxes);
            

            trackedObjects = new List<WorldObject>();

            objectGrid = new List<WorldObject>[numXBoxesCeil, numYBoxesCeil];
            for(int x = 0; x < numXBoxesFloor; x++)
            {
                for(int y = 0; y < numYBoxesFloor; y++)
                {
                    objectGrid[x, y] = new List<WorldObject>();
                }
            }

            if(numXBoxesCeil != numXBoxesFloor)
            {
                for(int i = 0; i < numYBoxesFloor; i++)
                {
                    objectGrid[numXBoxesFloor, i] = objectGrid[numXBoxesFloor - 1, i];
                }
            }
            if(numYBoxesCeil != numYBoxesFloor)
            {
                for(int i = 0; i < numXBoxesCeil; i++)
                {
                    objectGrid[i, numYBoxesFloor] = objectGrid[i, numYBoxesFloor - 1];
                }
            }
        }


        public bool Insert(WorldObject newObject)
        {
            //figure out xMin and xMax bucket
            int xMaxBucket = (int)(newObject.CentrePoint.X + newObject.Radius) / GridSize;
            int xMinBucket = (int)(newObject.CentrePoint.X - newObject.Radius) / GridSize;
            //figure out yMin and yMax bucket
            int yMaxBucket = (int)(newObject.CentrePoint.Y + newObject.Radius) / GridSize;
            int yMinBucket = (int)(newObject.CentrePoint.Y - newObject.Radius) / GridSize;

            //This creates a list of grid buckets that the agent falls within
            List<Point> myCoords = new List<Point>();
            for (int x = xMinBucket; x <= xMaxBucket; x++)
            {
                for(int y = yMinBucket; y <= yMaxBucket; y++)
                {
                    myCoords.Add(new Point(x,y));
                }
            }

            //insert into all applicable buckets
            foreach(Point gc in myCoords)
            {
                objectGrid[(int)gc.X,(int)gc.Y].Add(newObject);
            }

            //Insert agent into the bucket tracker,
            //Used to remove the agent
            agentLocationTracker.Add(newObject, myCoords);

            //Add agent to all tracked objects in this layer.
            //Used for counting and display logic
            trackedObjects.Add(newObject);

            //TODO: If there is ever a meaningful change this coudl fail, return false
            return true;
        }

        public void MoveObject(WorldObject moveMe)
        {
            RemoveObject(moveMe);
            Insert(moveMe);
            //TODO: handle boolean return value
        }

        public List<WorldObject> QueryForBoundingBoxCollisions(WorldObject queryObject)
        {
            return QueryForBoundingBoxCollisions(queryObject.BoundingBox, queryObject);
        }

        public List<WorldObject> QueryForBoundingBoxCollisions(BoundingBox queryBox)
        {
             //figure out xMin and xMax bucket
            int xMaxBucket = (int)(queryBox.MaxX) / GridSize;
            int xMinBucket = (int)(queryBox.MinX) / GridSize;
            //figure out yMin and yMax bucket
            int yMaxBucket = (int)(queryBox.MaxY) / GridSize;
            int yMinBucket = (int)(queryBox.MinY) / GridSize;

            //This creates a list of grid buckets that the bounding box falls within
            HashSet<WorldObject> potentialCollisions = new HashSet<WorldObject>();
            for (int x = xMinBucket; x <= xMaxBucket; x++)
            {
                for (int y = yMinBucket; y <= yMaxBucket; y++)
                {
                    foreach(WorldObject wo in objectGrid[x,y])
                    {
                        potentialCollisions.Add(wo);
                    }
                }
            }

            List<WorldObject> boundingCollisions = new List<WorldObject>();
            foreach(WorldObject wo in potentialCollisions)
            {
                if(wo.BoundingBox.IsCollision(queryBox))
                {
                    boundingCollisions.Add(wo);
                }
            }

            return boundingCollisions;
        }

        public List<WorldObject> QueryForBoundingBoxCollisions(BoundingBox queryBox, WorldObject self)
        {
            List<WorldObject> tempList = QueryForBoundingBoxCollisions(queryBox);
            tempList.Remove(self);
            return tempList;
        }

        public void RemoveObject(WorldObject killMe)
        {
            trackedObjects.Remove(killMe);
            List<Point> myCoords = agentLocationTracker[killMe];
            foreach(Point coord in myCoords)
            {
                objectGrid[(int)coord.X, (int)coord.Y].Remove(killMe);
            }
            agentLocationTracker.Remove(killMe);
        }

        public IEnumerator<WorldObject> GetEnumerator()
        {
            return trackedObjects.GetEnumerator();
        }

        public IEnumerable<WorldObject> EnumerateItems()
        {
            int i = 0;
            while(i < trackedObjects.Count)
            {
                WorldObject ret = trackedObjects[0];

                try { ret = trackedObjects[i++]; }
                catch(ArgumentOutOfRangeException aore)
                {
                    /* Swallowed, it is possible that while enumerating items, the list is modified. We'll return the first item in the list */
                }
                yield return ret;
            }
        }
    }
}
