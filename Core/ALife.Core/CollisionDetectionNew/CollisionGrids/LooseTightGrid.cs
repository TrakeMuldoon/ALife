﻿using ALife.Core.CollisionDetection.CollisionGrids.LooseGrids;
using ALife.Core.CollisionDetection.CollisionGrids.TightGrids;
using ALife.Core.CollisionDetection.Geometry;
using ALife.Core.Utility.Collections;

namespace ALife.Core.CollisionDetection.CollisionGrids
{
    public class LooseTightGrid
    {
        public Point Coordinates;
        public int ElementCount;
        public FreeList<Element> Elements;
        public LooseGrid Loose;
        public Point Size;
        public TightGrid Tight;

        public LooseTightGrid(double looseCellWidth, double looseCellHeight, double tightCellWidth, double tightCellHeight, Point size)
        {
            Size = size;
            Coordinates = new Point(0, 0);
            Loose = new LooseGrid(looseCellWidth, looseCellHeight, size);
            Tight = new TightGrid(tightCellWidth, tightCellHeight, size);
            Elements = new FreeList<Element>();
            ElementCount = 0;
        }
    }
}
