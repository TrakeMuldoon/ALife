namespace ALife.Spatial;

/// <summary>
/// The geometry shared by the layered grids: layer 0 is the top grid, with the biggest cells, and each layer below splits
/// every cell of the one above into four. An element goes in the finest layer whose cells it fits in, by its center.
/// </summary>
internal sealed class GridLayers
{
    /// <summary>The most layers a grid can have.</summary>
    public const int MaxLayers = 12;

    /// <summary>The most cells the finest layer can have.</summary>
    public const int MaxCells = 1 << 24;

    // Elements must be a hair smaller than a cell to go in its layer, so float rounding can never put two overlapping
    // centers two cells apart.
    private const float FitFraction = 0.999f;

    private readonly float[] _inverseCellWidth, _inverseCellHeight;

    public GridLayers(Aabb bounds, int columns, int rows, int layers)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columns, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rows, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(layers, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(layers, MaxLayers);
        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentException("The bounds must have an area.", nameof(bounds));
        }

        if (((long)columns << (layers - 1)) * ((long)rows << (layers - 1)) > MaxCells)
        {
            throw new ArgumentOutOfRangeException(nameof(layers), layers, $"The finest layer would have more than {MaxCells:N0} cells.");
        }

        Bounds = bounds;
        Count = layers;
        Columns = new int[layers];
        Rows = new int[layers];
        CellWidth = new float[layers];
        CellHeight = new float[layers];
        _inverseCellWidth = new float[layers];
        _inverseCellHeight = new float[layers];
        for (int layer = 0; layer < layers; layer++)
        {
            Columns[layer] = columns << layer;
            Rows[layer] = rows << layer;
            CellWidth[layer] = bounds.Width / Columns[layer];
            CellHeight[layer] = bounds.Height / Rows[layer];
            _inverseCellWidth[layer] = Columns[layer] / bounds.Width;
            _inverseCellHeight[layer] = Rows[layer] / bounds.Height;
        }
    }

    public Aabb Bounds { get; }

    /// <summary>The number of layers.</summary>
    public int Count { get; }

    public int[] Columns { get; }

    public int[] Rows { get; }

    public float[] CellWidth { get; }

    public float[] CellHeight { get; }

    /// <summary>The number of cells in the top layer, which is what the work is split by.</summary>
    public int TopCells => Columns[0] * Rows[0];

    /// <summary>Whether a box is small enough for a layer's cells.</summary>
    public bool Fits(int layer, Aabb box) => box.Width <= CellWidth[layer] * FitFraction && box.Height <= CellHeight[layer] * FitFraction;

    /// <summary>The finest layer a box fits in, or the top layer if it doesn't fit anywhere.</summary>
    public int LayerFor(Aabb box)
    {
        int layer = Count - 1;
        while (layer > 0 && !Fits(layer, box))
        {
            layer--;
        }

        return layer;
    }

    public int Column(int layer, float x) => GridCells.Index(x - Bounds.Left, _inverseCellWidth[layer], Columns[layer]);

    public int Row(int layer, float y) => GridCells.Index(y - Bounds.Top, _inverseCellHeight[layer], Rows[layer]);

    public int CellOf(int layer, float x, float y) => Row(layer, y) * Columns[layer] + Column(layer, x);

    /// <summary>The cells holding the centers of a layer's elements that could reach a box, given how far they reach.</summary>
    public GridCells.Range Around(int layer, Aabb box, float reachX, float reachY) =>
        new(Column(layer, box.Left - reachX), Row(layer, box.Top - reachY), Column(layer, box.Right + reachX), Row(layer, box.Bottom + reachY));

    public Aabb CellBounds(int layer, int x, int y)
    {
        float left = Bounds.Left + x * CellWidth[layer], top = Bounds.Top + y * CellHeight[layer];
        return new Aabb(left, top, left + CellWidth[layer], top + CellHeight[layer]);
    }

    /// <summary>
    /// Calls <paramref name="body"/> for each top cell, on up to <paramref name="threads"/> threads, and returns when they've
    /// all finished. Each layer's cells under top cell <c>(x, y)</c> are columns <c>x &lt;&lt; layer</c> up to
    /// <c>(x + 1) &lt;&lt; layer</c>, and the same for rows.
    /// </summary>
    public void ForEachTopCell(int threads, Action<int> body)
    {
        if (threads == 1)
        {
            for (int cell = 0; cell < TopCells; cell++)
            {
                body(cell);
            }

            return;
        }

        Parallel.For(0, TopCells, new ParallelOptions { MaxDegreeOfParallelism = threads }, body);
    }
}
