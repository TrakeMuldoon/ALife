using ALife.Core.CollisionDetection.Geometry;
using System.Collections.Generic;

namespace ALife.Core.CollisionDetection.CollisionGrids.LooseGrids
{
    public struct LooseGrid
    {
        public int CellCount;
        public int NumberColumns;
        public int NumberRows;
        List<Cell> Cells;
        Point InverseCellSize;

        public LooseGrid(double cellWidth, double cellHeight, Point size)
        {
            InverseCellSize = new Point(1.0 / cellWidth, 1.0 / cellHeight);
            NumberColumns = (int)(size.X * InverseCellSize.X);
            NumberRows = (int)(size.Y * InverseCellSize.Y);
            CellCount = NumberColumns * NumberRows;
            Cells = new List<Cell>(CellCount);
            for(int i = 0; i < CellCount; i++)
            {
                Cells.Add(new Cell());
            }
        }
    }
}
