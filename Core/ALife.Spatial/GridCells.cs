namespace ALife.Spatial;

/// <summary>Cell arithmetic shared by the grids.</summary>
internal static class GridCells
{
    /// <summary>The cell a coordinate falls in, clamped to the grid; NaN and out-of-range values land in the edge cells.</summary>
    /// <param name="offset">The coordinate minus the grid's left (or top).</param>
    /// <param name="inverseCellSize">One over the cell width (or height).</param>
    /// <param name="cellCount">The number of columns (or rows).</param>
    public static int Index(float offset, float inverseCellSize, int cellCount)
    {
        float cell = offset * inverseCellSize;
        if (!(cell >= 0))
        {
            return 0;
        }

        return cell >= cellCount ? cellCount - 1 : (int)cell;
    }

    /// <summary>A range of cells, inclusive. It's empty when <see cref="X1"/> is past <see cref="X2"/> or <see cref="Y1"/> past <see cref="Y2"/>.</summary>
    internal readonly record struct Range(int X1, int Y1, int X2, int Y2)
    {
        public static Range None { get; } = new(0, 0, -1, -1);

        public bool Contains(int x, int y) => x >= X1 && x <= X2 && y >= Y1 && y <= Y2;
    }
}
