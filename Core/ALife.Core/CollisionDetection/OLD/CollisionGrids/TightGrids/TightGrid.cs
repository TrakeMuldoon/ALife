using ALife.Core.Utility.Collections;
using System.Collections.Generic;

namespace ALife.Core.CollisionDetection.OLD.CollisionGrids.TightGrids
{
    public struct TightGrid
    {
        public int CellCount;
        public FreeList<Cell> Cells;

        public List<int> Heads;
        public Point InverseCellSize;
        public int NumberColumns;

        public int NumberRows;
    }
}
