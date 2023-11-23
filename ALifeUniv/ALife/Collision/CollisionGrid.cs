using ALifeUni.ALife.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    public class CollisionGrid<T> : ICollisionMap<T> where T : IHasShape
    {
        public readonly int Height;
        public readonly int Width;
        public readonly string GridName;

        //TODO: Load from Default Configuration
        private int GridXMax;
        private int GridYMax;
        private int GridSize = 25;
        private List<T>[,] objectGrid;
        private List<T> trackedObjects;
        private Dictionary<T, List<Point>> agentLocationTracker = new Dictionary<T, List<Point>>();

        public CollisionGrid(int gridHeight, int gridWidth, string gridName)
        {
            GridName = gridName;
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
            GridXMax = numXBoxesCeil;
            GridYMax = numYBoxesCeil;

            trackedObjects = new List<T>();

            objectGrid = new List<T>[numXBoxesCeil, numYBoxesCeil];
            for(int x = 0; x < numXBoxesFloor; x++)
            {
                for(int y = 0; y < numYBoxesFloor; y++)
                {
                    objectGrid[x, y] = new List<T>();
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


        public bool Insert(T newObject)
        {
            //figure out xMin and xMax bucket
            BoundingBox bb = newObject.Shape.BoundingBox;

            int xMaxBucket = (int)(bb.MaxX) / GridSize;
            int xMinBucket = (int)(bb.MinX) / GridSize;
            //figure out yMin and yMax bucket
            int yMaxBucket = (int)(bb.MaxY) / GridSize;
            int yMinBucket = (int)(bb.MinY) / GridSize;

            //This creates a list of grid buckets that the agent falls within
            List<Point> myCoords = new List<Point>();
            for(int x = xMinBucket; x <= xMaxBucket; x++)
            {
                for(int y = yMinBucket; y <= yMaxBucket; y++)
                {
                    myCoords.Add(new Point(x, y));
                }
            }

            //insert into all applicable buckets
            foreach(Point gc in myCoords)
            {
                if(gc.X < 0
                   || gc.Y < 0
                   || gc.X >= GridXMax
                   || gc.Y >= GridYMax)
                {
                    continue;
                }

                objectGrid[(int)gc.X, (int)gc.Y].Add(newObject);
            }

            //Insert agent into the bucket tracker,
            //Used to remove the agent
            agentLocationTracker.Add(newObject, myCoords);

            //Add agent to all tracked objects in this layer.
            //Used for counting and display logic
            trackedObjects.Add(newObject);

            //TODO: If there is ever a meaningful chance this could fail, return false
            return true;
        }

        public void RemoveObject(T killMe)
        {
            trackedObjects.Remove(killMe);
            List<Point> myCoords = agentLocationTracker[killMe];
            foreach(Point coord in myCoords)
            {
                //In case some objects go out of bounds.
                if(coord.X < 0
                   || coord.Y < 0
                   || coord.X >= GridXMax
                   || coord.Y >= GridYMax)
                {
                    continue;
                }
                objectGrid[(int)coord.X, (int)coord.Y].Remove(killMe);
            }
            agentLocationTracker.Remove(killMe);
        }

        public void MoveObject(T moveMe)
        {
            RemoveObject(moveMe);
            Insert(moveMe);
            //TODO: handle boolean return value
        }

        public List<T> QueryForBoundingBoxCollisions(T queryObject)
        {
            return QueryForBoundingBoxCollisions(queryObject.Shape.BoundingBox, queryObject);
        }

        public List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox)
        {
            //figure out xMin and xMax bucket
            int xMaxBucket = (int)(queryBox.MaxX) / GridSize;
            int xMinBucket = (int)(queryBox.MinX) / GridSize;
            //figure out yMin and yMax bucket
            int yMaxBucket = (int)(queryBox.MaxY) / GridSize;
            int yMinBucket = (int)(queryBox.MinY) / GridSize;

            //Clamp Them
            xMaxBucket = Math.Clamp(xMaxBucket, 0, objectGrid.GetLength(0) - 1);
            xMinBucket = Math.Clamp(xMinBucket, 0, objectGrid.GetLength(0) - 1);
            yMaxBucket = Math.Clamp(yMaxBucket, 0, objectGrid.GetLength(1) - 1);
            yMinBucket = Math.Clamp(yMinBucket, 0, objectGrid.GetLength(1) - 1);

            //This creates a list of grid buckets that the bounding box falls within
            HashSet<T> potentialCollisions = new HashSet<T>();
            for(int x = xMinBucket; x <= xMaxBucket; x++)
            {
                for(int y = yMinBucket; y <= yMaxBucket; y++)
                {
                    foreach(T wo in objectGrid[x, y])
                    {
                        potentialCollisions.Add(wo);
                    }
                }
            }

            List<T> boundingCollisions = new List<T>();
            foreach(T wo in potentialCollisions)
            {
                if(wo.Shape.BoundingBox.IsCollision(queryBox))
                {
                    boundingCollisions.Add(wo);
                }
            }

            return boundingCollisions;
        }

        public List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox, T self)
        {
            List<T> tempList = QueryForBoundingBoxCollisions(queryBox);
            tempList.Remove(self);
            return tempList;
        }

        public List<T> DetectCollisions(T myBody)
        {
            List<T> collisions = QueryForBoundingBoxCollisions(myBody.Shape.BoundingBox, myBody);
            List<IHasShape> colShapes = CollisionDetector.FineGrainedCollisionDetection(collisions.Cast<IHasShape>(), myBody.Shape);
            collisions = colShapes.Cast<T>().ToList();
            return collisions;
        }

        public List<T> DetectCollisions(IHasShape detectionArea, T myBody)
        {
            List<T> collisions = QueryForBoundingBoxCollisions(detectionArea.Shape.BoundingBox, myBody);
            List<IHasShape> colShapes = CollisionDetector.FineGrainedCollisionDetection(collisions.Cast<IHasShape>(), detectionArea.Shape);
            collisions = colShapes.Cast<T>().ToList();
            return collisions;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return trackedObjects.GetEnumerator();
        }

        public IEnumerable<T> EnumerateItems()
        {
            int i = 0;
            while(i < trackedObjects.Count)
            {
                T ret = trackedObjects[0];

                try { ret = trackedObjects[i++]; }
                catch(ArgumentOutOfRangeException)
                {
                    //TODO Find out if this causes bugs, where some itemse are getting hit up twice.
                    /* Swallowed, it is possible that while enumerating items, the list is modified. We'll return the first item in the list */
                }
                yield return ret;
            }
        }

    }
}