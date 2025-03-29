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
        const int GRID_HEIGHT_IN_CELLS = 28;
        const int CELL_WIDTH = 35;
        const int CELL_HEIGHT = 35;
        const int CELLS_TOTAL = 3000;

        public readonly List<BioCell> agents = new List<BioCell>();
        public readonly List<Color> colours = new List<Color>();
        public AAGrid worldGrid = new AAGrid(CELL_WIDTH, CELL_HEIGHT, GRID_HEIGHT_IN_CELLS, GRID_WIDTH_IN_CELLS);
        public AAGrid futureGrid = new AAGrid(CELL_WIDTH, CELL_HEIGHT, GRID_HEIGHT_IN_CELLS, GRID_WIDTH_IN_CELLS);
        int randomSeed;

        public Simulation(int randomSeed, int numColours)
        {
            this.randomSeed = randomSeed;
            StaticRandom.Initialize(randomSeed);

            for(int i = 0; i < numColours; i++)
            {
                colours.Add(Color.FromArgb(StaticRandom.NextColourShortInt()
                                          , StaticRandom.NextColourShortInt()
                                          , StaticRandom.NextColourShortInt()));
            }

            for(int i = 0; i < numColours; i++)
            {
                List<SenseSet> senses = new List<SenseSet>()
                {
                    new SenseSet(50.0, colours),
                    new SenseSet(8.0, colours)
                    //new SenseSet(2.0, colours)
                };
                

                for(int cellIndex = 0; cellIndex < CELLS_TOTAL / numColours; cellIndex++)
                {
                    BioCell bc = new BioCell(colours[i], worldGrid, senses);
                    agents.Add(bc);
                }
            }
        }

        internal void RunForegroundStep()
        {
            Parallel.ForEach(agents, (ag) => { ag.SetTargetDirection(); });

            futureGrid.Clear();

            foreach(BioCell bc in agents)
            {
                futureGrid.InsertCellAt(bc,  bc.targetLocation);
            }

            Parallel.ForEach(agents, (ag) => { ag.CheckTargetCollision(futureGrid); });

            worldGrid.Clear();
            foreach(BioCell bc in agents)
            {
                bc.ExecuteTargetDirection();
            }
            

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

        public static int NextColourShortInt()
        {
            return myRandom.Next(226) + 30;
        }

        public static int NextInt(int maxValue)
        {
            return myRandom.Next(maxValue);
        }
    }

    internal class DoublePoint
    {
        public double X;
        public double Y;
        public DoublePoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    

    internal class BioCell
    {
        private static int global_cell_number = 0;

        public DoublePoint Location;
        public DoublePoint targetLocation;
        public int Radius;
        public Color myColour;
        List<SenseSet> senses;
        AAGrid theWorld;
        public readonly int NUMBER;
        public Boolean collision = false;
        private const double WALK_MAX = 12;
        private readonly double WALK_MAX_2 = WALK_MAX * WALK_MAX;

        public BioCell(Color newColour, AAGrid theWorld, List<SenseSet> senses)
        {
            this.NUMBER = global_cell_number++;
            this.senses = senses;
            Radius = 2;
            myColour = newColour;
            this.theWorld = theWorld;

            Location = new DoublePoint(StaticRandom.NextInt(theWorld.cellWidth * theWorld.gridWidthInCells - 1)
                                , StaticRandom.NextInt(theWorld.cellHeight * theWorld.gridHeightInCells - 1)
                                );

            theWorld.InsertCellAt(this, Location);
        }

        public void SetTargetDirection()
        {
            double xDiff = StaticRandom.BetweenNegOneAndOne() - StaticRandom.BetweenNegOneAndOne();
            double yDiff = StaticRandom.BetweenNegOneAndOne();

            foreach(SenseSet sense in senses)
            {
                List<BioCell> hits = theWorld.GetBCsInRange(Location, sense.Radius);
                foreach(BioCell target in hits)
                {
                    double multiplier = sense.Reactions[target.myColour];

                    double distSquared = GetDistSquared(this.Location, target.Location);
                    if(distSquared > sense.RadiusSquared)
                    {
                        //Too far away
                        continue;
                    }

                    double proximity = (sense.RadiusSquared - distSquared) / (sense.RadiusSquared);

                    double push = proximity * multiplier;

                    xDiff += (Location.X - target.Location.X) * push;
                    yDiff += (Location.Y - target.Location.Y) * push;
                }
            }

            double walkDistSq = (xDiff * xDiff) + (yDiff * yDiff);
            if (walkDistSq > WALK_MAX_2)
            {
                double ratio = walkDistSq / WALK_MAX_2;

                xDiff = xDiff * WALK_MAX_2 / walkDistSq;
                yDiff = yDiff * WALK_MAX_2 / walkDistSq;
            }

            double targetX = ClampValue(0, theWorld.worldWidth - 1, Location.X + xDiff);
            double targetY = ClampValue(0, theWorld.worldHeight - 1, Location.Y + yDiff);

            targetLocation = new DoublePoint(targetX, targetY);
        }

        private double ClampValue(double min, double max, double value)
        {
            if(value >= min && value <= max)
            {
                return value;
            }
            if(value < min)
            {
                return min;
            }
            return max; //this is the only option left
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


        private static double GetDistSquared(DoublePoint origin, DoublePoint target)
        {
            //c^2 = a^2 + b^2
            double distSquared = Math.Pow(origin.X - target.X, 2) 
                                 + Math.Pow(origin.Y - target.Y, 2);
            return distSquared;
        }

        internal void ExecuteTargetDirection()
        {
            //theWorld.RemoveCellFrom(this, this.Location);
            //check if collision, move only halfway.
            if(collision)
            {
                double deltaXShrink = (this.Location.X - this.targetLocation.X) / 10;
                double deltaYShrink = (this.Location.Y - this.targetLocation.Y) / 10;

                double newX = this.Location.X - deltaXShrink;
                double newY = this.Location.Y - deltaYShrink;
                DoublePoint smallMove = new DoublePoint(newX, newY);
                this.targetLocation = smallMove;
            }    
            theWorld.InsertCellAt(this, this.targetLocation);
            this.Location = this.targetLocation;
        }

        internal void CheckTargetCollision(AAGrid futureGrid)
        {
            collision = false;
            List<BioCell> hits = futureGrid.GetBCsInRange(targetLocation, Radius);
            foreach(BioCell target in hits)
            {
                if(this == target)
                {
                    //me!
                    continue;
                }
                double distSquared = GetDistSquared(this.targetLocation, target.targetLocation);
                if(distSquared > Radius * Radius)
                {
                    //Too far away
                    continue;
                }

                collision = true;
                return;
            }
            return;
        }
    }

    internal class SenseSet
    {
        public readonly double Radius;
        public readonly double RadiusSquared;
        public readonly Dictionary<Color, double> Reactions = new Dictionary<Color, double>();

        public SenseSet(double radius, List<Color> colors)
        {
            Radius = radius;
            RadiusSquared = radius * radius;

            foreach(Color c in colors)
            {
                Reactions.Add(c, StaticRandom.BetweenNegOneAndOne());
            }
        }
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

        internal void Clear()
        {
            Parallel.For(0, gridHeightInCells * gridWidthInCells
                , (index) =>
                {
                    int x = index % gridWidthInCells;
                    int y = index / gridWidthInCells;
                    gridCells[x, y].Clear();
                }
            );
        }

        internal List<BioCell> GetBCsInRange(DoublePoint location, double radius)
        {
            double left = location.X - radius;
            double right = location.X + radius;
            double top = location.Y - radius;
            double bottom = location.Y + radius;

            int firstBucketX = (int)(left / cellWidth);
            int lastBucketX = (int)(right / cellWidth);
            int firstBucketY = (int)(top / cellHeight);
            int lastBucketY = (int)(bottom / cellHeight);

            firstBucketX = firstBucketX < 0 ? 0 : firstBucketX;
            firstBucketY = firstBucketY < 0 ? 0 : firstBucketY;

            HashSet<BioCell> cellList = new HashSet<BioCell>();

            for(int x = firstBucketX; x <= lastBucketX && x < gridWidthInCells; x++)
            {
                for(int y = firstBucketY; y <= lastBucketY && y < gridHeightInCells; y++)
                {
                    foreach(BioCell bc in gridCells[x,y])
                    {
                        cellList.Add(bc);
                    }
                }
            }
            return cellList.ToList();
        }

        internal void InsertCellAt(BioCell cell, DoublePoint location)
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

        internal void RemoveCellFrom(BioCell cell, DoublePoint location)
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
                    gridCells[x, y].Remove(cell);
                }
            }
        }
    }
}


