using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace RunningDots
{
    internal class Simulation
    {
        const int GRID_WIDTH_IN_CELLS = 40;
        const int GRID_HEIGHT_IN_CELLS = 25;
        const int CELL_WIDTH = 30;
        const int CELL_HEIGHT = 30;
        const int CELLS_TOTAL = 1000;

        public readonly List<BioCell> agents = new List<BioCell>();
        public readonly List<Color> colours = new List<Color>();
        public AAGrid worldGrid = new AAGrid(CELL_WIDTH, CELL_HEIGHT, GRID_HEIGHT_IN_CELLS, GRID_WIDTH_IN_CELLS);
        int randomSeed;

        public Simulation(int randomSeed, int numColours)
        {
            this.randomSeed = randomSeed;
            StaticRandom.Initialize(randomSeed);

            for(int i = 0; i < numColours; i++)
            {
                colours.Add(Color.FromArgb(StaticRandom.NextShortInt()
                                          , StaticRandom.NextShortInt()
                                          , StaticRandom.NextShortInt()));
            }

            for(int i = 0; i < numColours; i++)
            {
                for(int cellIndex = 0; cellIndex < CELLS_TOTAL / numColours; cellIndex++)
                {
                    BioCell bc = new BioCell(colours[i], worldGrid);
                    agents.Add(bc);
                }
            }
        }

        internal void RunForegroundStep()
        {
            Parallel.ForEach(agents, (ag) => { ag.SetTargetDirection(); });

            Parallel.ForEach(agents, (ag) => { ag.ExecuteTargetDirection(); });

        }

        void RumSimulation(BackgroundWorker worker, DoWorkEventArgs e)
        {
            while(!e.Cancel)
            {
                if(worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //DO A SIM STEP

                // Report progress as a percentage of the total task.
                worker.ReportProgress(1, worldGrid);
            }
            return;
        }
    }


    internal static class StaticRandom
    {
        public static Random myRandom = new Random();

        public static void Initialize(int randomSeed)
        {
            myRandom = new Random(randomSeed);
        }

        public static double BetweenNegOneAndOne()
        {
            return (myRandom.NextDouble() * 2) - 1;
        }

        public static int NextShortInt()
        {
            return myRandom.Next(256);
        }

        public static int NextInt(int maxValue)
        {
            return myRandom.Next(maxValue);
        }
    } 

    internal class BioCell
    {
        public Point Location;
        public Point targetLocation;
        public int Radius;
        public Color myColour;
        List<SenseSet> senses;
        AAGrid theWorld;

        public BioCell(Color newColour, AAGrid theWorld)
        {
            senses = new List<SenseSet>();
            Radius = 2;
            myColour = newColour;
            this.theWorld = theWorld;

            Location = new Point(StaticRandom.NextInt(theWorld.cellWidth * theWorld.gridWidthInCells - 1)
                                , StaticRandom.NextInt(theWorld.cellHeight * theWorld.gridHeightInCells - 1)
                                );

            theWorld.InsertCellAt(this, Location);
        }

        public void SetTargetDirection()
        {
            double xDiff = -1;//StaticRandom.BetweenNegOneAndOne();
            double yDiff = -1;//StaticRandom.BetweenNegOneAndOne();

            //foreach(SenseSet sense in senses)
            //{
            //    List<BioCell> hits = theWorld.GetBBCollisions(Location, sense.Radius);
            //    foreach(BioCell target in hits)
            //    {
            //        double multiplier = sense.Reactions[target.myColour];

            //        double distSquared = GetDistSquared(this, target);
            //        if (distSquared > sense.RadiusSquared)
            //        {
            //            //Too far away
            //            continue;
            //        }

            //        double proximity = (sense.RadiusSquared - distSquared) / sense.RadiusSquared;

            //        double push = proximity * multiplier;

            //        xDiff += (Location.X - target.Location.X) * push;
            //        yDiff += (Location.Y - target.Location.Y) * push;
            //    }
            //}

            int targetX = ClampValue(0, theWorld.worldWidth - 1, (int)(Location.X + xDiff));
            int targetY = ClampValue(0, theWorld.worldHeight - 1, (int)(Location.Y + yDiff));

            targetLocation = new Point(targetX, targetY);
        }

        private int ClampValue(int min, int max, int value)
        {
            if (value >= min && value <= max)
            {
                return value;
            }
            if (value < min)
            {
                return min;
            }
            return max; //this is the only option left
        }

        private static double GetDistSquared(BioCell origin, BioCell target)
        {
            //c^2 = a^2 + b^2
            double distSquared = Math.Pow(origin.Location.X + target.Location.X, 2) 
                                 + Math.Pow(origin.Location.Y + target.Location.Y, 2);
            return distSquared;
        }

        internal void ExecuteTargetDirection()
        {
            theWorld.RemoveCellFrom(this, this.Location);
            theWorld.InsertCellAt(this, this.targetLocation);
            this.Location = this.targetLocation;
        }
    }

    internal class SenseSet
    {
        public readonly double Radius;
        public readonly double RadiusSquared;
        public readonly Dictionary<Color, double> Reactions = new Dictionary<Color, double>();
    }

    internal class AAGrid
    {
        public readonly List<BioCell>[,] gridCells;
        public int cellWidth;
        public int cellHeight;
        public int gridHeightInCells;
        public int gridWidthInCells;
        public int worldWidth;
        public int worldHeight;

        public AAGrid(int cellWidth, int cellHeight, int gridHeightInCells, int gridWidthInCells)
        {
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.gridWidthInCells = gridWidthInCells;
            this.gridHeightInCells = gridHeightInCells;
            this.worldHeight = cellHeight * gridHeightInCells;
            this.worldWidth = cellWidth * gridWidthInCells;

            gridCells = new List<BioCell>[gridWidthInCells, gridHeightInCells];

            for(int gridX = 0; gridX < gridWidthInCells; gridX++)
            {
                for(int gridY = 0; gridY < gridHeightInCells; gridY++)
                {
                    gridCells[gridX, gridY] = new List<BioCell>();
                }    
            }
        }

        internal List<BioCell> GetBBCollisions(Point location, double radius)
        {
            double left = location.X - radius;
            double right = location.X + radius;
            double top = location.Y - radius;
            double bottom = location.Y + radius;

            int firstBucketX = (int)(left / cellWidth);
            int lastBucketX = (int)(right / cellWidth);
            int firstBucketY = (int)(top / cellHeight);
            int lastBucketY = (int)(bottom / cellHeight);

            HashSet<BioCell> cellList = new HashSet<BioCell>();

            for(int x = firstBucketX; x <= lastBucketX && x < gridWidthInCells; x++)
            {
                for(int y = firstBucketY; y <= lastBucketY && y < gridHeightInCells; y++)
                {
                    foreach(BioCell bc in gridCells[x,y])
                    {
                        if(!cellList.Contains(bc))
                        {
                            cellList.Add(bc);
                        }
                    }
                }
            }
            return cellList.ToList();
        }

        internal void InsertCellAt(BioCell cell, Point location)
        {

            double left = location.X - cell.Radius;
            double right = location.X + cell.Radius;
            double top = location.Y - cell.Radius;
            double bottom = location.Y + cell.Radius;

            int firstBucketX = (int)(left / cellWidth);
            int lastBucketX = (int)(right / cellWidth);
            int firstBucketY = (int)(top / cellHeight);
            int lastBucketY = (int)(bottom / cellHeight);

            for(int x = firstBucketX; x <= lastBucketX && x < gridWidthInCells; x++)
            {
                for(int y = firstBucketY; y <= lastBucketY && y < gridHeightInCells; y++)
                {
                    gridCells[x, y].Add(cell);
                }
            }
        }

        internal void RemoveCellFrom(BioCell cell, Point location)
        {

            double left = location.X - cell.Radius;
            double right = location.X + cell.Radius;
            double top = location.Y - cell.Radius;
            double bottom = location.Y + cell.Radius;

            int firstBucketX = (int)(left / cellWidth);
            int lastBucketX = (int)(right / cellWidth);
            int firstBucketY = (int)(top / cellHeight);
            int lastBucketY = (int)(bottom / cellHeight);

            for(int x = firstBucketX; x <= lastBucketX && x < gridWidthInCells; x++)
            {
                for(int y = firstBucketY; y <= lastBucketY && y < gridHeightInCells; y++)
                {
                    gridCells[x, y].Add(cell);
                }
            }
        }
    }
}


