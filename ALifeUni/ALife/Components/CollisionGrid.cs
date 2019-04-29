using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{


    class CollisionGrid : ICollisionMap
    {
        struct GridCoord
        {
            public int x;
            public int y;

            public GridCoord(int X, int Y)
            {
                x = X;
                y = Y;
            }
        }

        public readonly int Height;
        public readonly int Width;

        //TODO: Load from Default Configuration
        private int GridSize = 25;
        private List<WorldObject>[,] objectGrid;
        private List<WorldObject> trackedObjects;
        private Dictionary<WorldObject, List<GridCoord>> agentLocationTracker = new Dictionary<WorldObject, List<GridCoord>>();

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

            double numXBoxes = Width / (double)GridSize;
            double numYBoxes = Height / (double)GridSize;
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


        public void Insert(WorldObject newObject)
        {
            //figure out xMin and xMax bucket
            int xMaxBucket = (int)(newObject.CentrePoint.X + newObject.Radius) / GridSize;
            int xMinBucket = (int)(newObject.CentrePoint.X - newObject.Radius) / GridSize;
            //figure out yMin and yMax bucket
            int yMaxBucket = (int)(newObject.CentrePoint.Y + newObject.Radius) / GridSize;
            int yMinBucket = (int)(newObject.CentrePoint.Y - newObject.Radius) / GridSize;

            //This creates a list of grid buckets that the agent falls within
            List<GridCoord> myCoords = new List<GridCoord>();
            for (int x = xMinBucket; x <= xMaxBucket; x++)
            {
                for(int y = yMinBucket; y <= yMaxBucket; y++)
                {
                    myCoords.Add(new GridCoord(x,y));
                }
            }

            //insert into all applicable buckets
            foreach(GridCoord gc in myCoords)
            {
                objectGrid[gc.x,gc.y].Add(newObject);
            }

            //Insert agent into the bucket tracker,
            //Used to be able to remove the agents from all buckets they are in
            if (!agentLocationTracker.ContainsKey(newObject))
            {
                agentLocationTracker.Add(newObject, new List<GridCoord>());
            }

            //Add agent to all tracked objects in this layer.
            //Used for counting and display logic
            trackedObjects.Add(newObject);
        }

        public void MoveObject(WorldObject moveMe)
        {
            //throw new NotImplementedException();
        }

        public List<WorldObject> QueryForCollisions(WorldObject queryObject)
        {
            //throw new NotImplementedException();
        }

        public List<WorldObject> QueryForCollisions(BoundingBox queryBox)
        {
            //throw new NotImplementedException();
        }

        public List<WorldObject> QueryForCollisions(BoundingBox queryBox, WorldObject self)
        {
            //throw new NotImplementedException();
        }

        public void RemoveObject(WorldObject killMe)
        {
            //throw new NotImplementedException();
        }

        public IEnumerator<WorldObject> GetEnumerator()
        {
            return trackedObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return trackedObjects.GetEnumerator();
        }
    }
}
