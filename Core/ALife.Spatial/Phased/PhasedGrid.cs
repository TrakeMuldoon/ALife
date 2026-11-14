using System.Numerics;
using ALife.Spatial.Packed;

namespace ALife.Spatial.Phased;

/// <summary>
/// A grid rebuilt every frame, optionally in layers, whose parallel pair search reports each pair once and lets the visitor
/// write to both elements. The top layer is split into tiles, and the tiles are searched in four phases, like the squares of
/// a checkerboard in two colors each way, so tiles searched at the same time never share an element.
/// </summary>
/// <remarks>
/// <para>
/// With one layer it's a uniform grid, like <see cref="PackedGrid"/>; with more it's a stack of ever-finer grids, like
/// <see cref="PackedLayeredGrid"/>. Size the top cells a little bigger than your biggest common element and, for mixed
/// sizes, add layers until the finest cells are a little bigger than your smallest. Bigger elements go in a side tree
/// (<see cref="OversizedCount"/>), and their pairs are searched on one thread after the phases.
/// </para>
/// <para>
/// <see cref="Build"/> sorts on up to <see cref="Threads"/> threads, and <see cref="Order"/> gives the order it sorted the
/// elements into, so you can keep your own arrays in roughly that order and turn the pair search's reads into near-sequential ones.
/// </para>
/// </remarks>
public sealed class PhasedGrid : IPackedSpatialIndex
{
    /// <summary>The most layers a grid can have.</summary>
    public const int MaxLayers = GridLayers.MaxLayers;

    // Builds split the boxes into chunks of at least this many, so small builds stay on one thread.
    private const int MinChunk = 8192;

    // When there are a lot of slots for the elements, the build sorts by slot in two steps: first into buckets by the slots'
    // top this many bits, so each chunk only counts a couple of thousand buckets however many cells there are, then within
    // each bucket by the rest.
    private const int BucketBits = 11;

    // Aim for this many tiles per phase per thread, so a slow tile doesn't hold up the others.
    private const int TilesPerThread = 16;

    private readonly GridLayers _layers;
    private readonly PackedBoxes _sorted = new();
    private readonly PackedQuadTree _large;

    // Every layer's cells numbered one after another, top layer first, then a slot for oversized elements and one for empty
    // boxes. Each slot's run starts at _starts[slot], and ends where the next starts.
    private readonly int[] _layerFirstSlot;
    private readonly int _oversizedSlot, _emptySlot;
    private readonly int[] _starts;
    private readonly float[] _reachX, _reachY;
    private readonly int _bucketLowBits;
    private readonly int[] _bucketStarts;

    private Aabb[] _boxes = [];
    private int[] _slots = [];

    // The elements sorted into buckets, each as its slot and number packed into one value, slot in the high half.
    private long[] _entries = [];

    // Each chunk's count of each bucket, then where its next element in that bucket goes.
    private int[] _chunkCounts = [];

    // For each chunk, a count of each slot in the bucket it's sorting, then where its next element in that slot goes.
    private int[] _slotCounts = [];
    private float[] _chunkReach = [];
    private int[] _oversized = [];
    private Aabb[] _largeBoxes = [];
    private int _chunks;

    // How many slots a bucket covers in this build, as a power of two, and how many buckets that makes.
    private int _lowBits, _buckets;
    private int _threads;
    private int _tileSize;

    /// <summary>Creates an empty grid.</summary>
    /// <param name="bounds">The area to cover. Element centers outside it are filed in its edge cells.</param>
    /// <param name="columns">The number of cell columns in the top layer. Each layer below has twice as many.</param>
    /// <param name="rows">The number of cell rows in the top layer. Each layer below has twice as many.</param>
    /// <param name="layers">How many layers, from 1 (a plain grid) to <see cref="MaxLayers"/>.</param>
    /// <param name="threads">How many threads to use; see <see cref="Threads"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A count is below 1, <paramref name="layers"/> is above <see cref="MaxLayers"/>, or the finest layer would have more
    /// than 2^24 cells.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="bounds"/> has no area.</exception>
    public PhasedGrid(Aabb bounds, int columns, int rows, int layers = 1, int threads = 4)
    {
        _layers = new GridLayers(bounds, columns, rows, layers);
        _large = new PackedQuadTree(bounds);
        _layerFirstSlot = new int[layers + 1];
        for (int layer = 0; layer < layers; layer++)
        {
            _layerFirstSlot[layer + 1] = _layerFirstSlot[layer] + _layers.Columns[layer] * _layers.Rows[layer];
        }

        _oversizedSlot = _layerFirstSlot[layers];
        _emptySlot = _oversizedSlot + 1;
        _starts = new int[_emptySlot + 2];
        _reachX = new float[layers];
        _reachY = new float[layers];

        // When sorting in two steps, a bucket is a run of 2^_bucketLowBits slots, so there are at most 2^BucketBits buckets.
        int slotBits = 32 - BitOperations.LeadingZeroCount((uint)_emptySlot);
        _bucketLowBits = Math.Max(0, slotBits - BucketBits);
        _bucketStarts = new int[(_emptySlot >> _bucketLowBits) + 2];
        Threads = threads;
    }

    /// <inheritdoc />
    public Aabb Bounds => _layers.Bounds;

    /// <summary>The number of cell columns in the top layer.</summary>
    public int Columns => _layers.Columns[0];

    /// <summary>The number of cell rows in the top layer.</summary>
    public int Rows => _layers.Rows[0];

    /// <summary>The number of layers.</summary>
    public int Layers => _layers.Count;

    /// <summary>The width of a top cell. Each layer below halves it, and elements have to be a little narrower to go in a layer.</summary>
    public float CellWidth => _layers.CellWidth[0];

    /// <summary>The height of a top cell. Each layer below halves it, and elements have to be a little shorter to go in a layer.</summary>
    public float CellHeight => _layers.CellHeight[0];

    /// <summary>
    /// How many threads <see cref="Build"/>, <see cref="ForEachNeighbor{TVisitor}"/> and
    /// <see cref="ForEachPairInParallel{TVisitor}"/> use at most. 1 runs everything on the calling thread.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is below 1.</exception>
    public int Threads
    {
        get => _threads;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
            _threads = value;

            // Tiles have to be at least two top cells across, so that tiles of the same phase are a whole tile apart.
            _tileSize = Math.Max(2, (int)MathF.Sqrt(_layers.TopCells / (4f * TilesPerThread * value)));
        }
    }

    /// <inheritdoc />
    public int Count { get; private set; }

    /// <summary>How many elements in the last build were too big even for a top cell, and went in the side tree.</summary>
    public int OversizedCount { get; private set; }

    /// <summary>
    /// Every element of the last build, in the order the grid stores them: layer by layer from the top, cell by cell, then
    /// the oversized elements and the empty boxes. Elements in the same cell keep the order they were given in.
    /// </summary>
    /// <remarks>
    /// The pair searches read your data for elements that are near each other in this order. Reordering your arrays to match
    /// (element <c>Order[i]</c> moved to position <c>i</c>) before the next build makes those reads close together in memory.
    /// It's only valid until the next <see cref="Build"/>.
    /// </remarks>
    public ReadOnlySpan<int> Order => _sorted.Elements(Count);

    /// <inheritdoc />
    public void Build(ReadOnlySpan<Aabb> boxes)
    {
        int count = boxes.Length;
        if (count > _boxes.Length)
        {
            _boxes = new Aabb[Math.Max(count, _boxes.Length * 2)];
            _slots = new int[_boxes.Length];
            _entries = new long[_boxes.Length];
        }

        boxes.CopyTo(_boxes);
        _sorted.EnsureCapacity(count);
        Count = count;

        // Each chunk of the boxes works out its slots and how far its boxes reach on its own, so the chunks can run at once.
        // If every chunk can count every slot with no more counters in all than there are elements, the buckets are the
        // slots and that's the whole sort.
        _chunks = Math.Clamp(count / MinChunk, 1, _threads);
        _lowBits = (long)_chunks * (_starts.Length - 1) <= count ? 0 : _bucketLowBits;
        _buckets = (_emptySlot >> _lowBits) + 1;
        if (_chunkCounts.Length < _chunks * _buckets)
        {
            _chunkCounts = new int[_chunks * _buckets];
        }

        if (_slotCounts.Length < _chunks << _lowBits)
        {
            _slotCounts = new int[_chunks << _lowBits];
        }

        if (_chunkReach.Length < _chunks * MaxLayers * 2)
        {
            _chunkReach = new float[_chunks * MaxLayers * 2];
        }

        RunChunks(SlotChunk);
        for (int layer = 0; layer < Layers; layer++)
        {
            _reachX[layer] = 0;
            _reachY[layer] = 0;
            for (int chunk = 0; chunk < _chunks; chunk++)
            {
                _reachX[layer] = MathF.Max(_reachX[layer], _chunkReach[(chunk * MaxLayers + layer) * 2]);
                _reachY[layer] = MathF.Max(_reachY[layer], _chunkReach[(chunk * MaxLayers + layer) * 2 + 1]);
            }
        }

        // Running totals, bucket by bucket and chunk by chunk within a bucket, turn each chunk's counts into where it puts
        // its elements in that bucket. So a cell's elements stay in the order they came, and the sort comes out the same
        // however many chunks there are.
        int[] bucketStarts = _lowBits == 0 ? _starts : _bucketStarts;
        int total = 0;
        for (int bucket = 0; bucket < _buckets; bucket++)
        {
            bucketStarts[bucket] = total;
            for (int chunk = 0; chunk < _chunks; chunk++)
            {
                int index = chunk * _buckets + bucket;
                int chunkCount = _chunkCounts[index];
                _chunkCounts[index] = total;
                total += chunkCount;
            }
        }

        bucketStarts[_buckets] = total;
        RunChunks(BucketChunk);
        if (_lowBits > 0)
        {
            RunChunks(SortBucketsChunk);
            _starts[^1] = count;
        }

        int firstOversized = _starts[_oversizedSlot];
        OversizedCount = _starts[_emptySlot] - firstOversized;
        if (_oversized.Length < OversizedCount)
        {
            _oversized = new int[Math.Max(16, OversizedCount * 2)];
            _largeBoxes = new Aabb[_oversized.Length];
        }

        for (int k = 0; k < OversizedCount; k++)
        {
            _oversized[k] = _sorted.ElementAt(firstOversized + k);
            _largeBoxes[k] = _boxes[_oversized[k]];
        }

        _large.Build(_largeBoxes.AsSpan(0, OversizedCount));
    }

    /// <inheritdoc />
    public Aabb GetBounds(int element) => _boxes[element];

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        if (area.IsEmpty)
        {
            return;
        }

        for (int layer = 0; layer < Layers; layer++)
        {
            if (IsEmpty(layer))
            {
                continue;
            }

            int first = _layerFirstSlot[layer], columns = _layers.Columns[layer];
            GridCells.Range range = _layers.Around(layer, area, _reachX[layer], _reachY[layer]);
            for (int y = range.Y1; y <= range.Y2; y++)
            {
                int row = first + y * columns;
                _sorted.AddRun(results, area, _starts[row + range.X1], _starts[row + range.X2 + 1], omitElement);
            }
        }

        if (OversizedCount > 0)
        {
            LargeCollector collector = new(results, _oversized, omitElement);
            _large.VisitArea(ref collector, -1, area);
        }
    }

    /// <inheritdoc />
    public void ForEachPair<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor
    {
        PairsInTopCells(ref visitor, 0, 0, Columns, Rows);
        OversizedPairs(ref visitor);
    }

    /// <summary>
    /// Calls <c>visitor.Visit(a, b)</c> once for each pair of elements whose boxes intersect, on up to
    /// <see cref="Threads"/> threads, and returns when they're all done. Two calls that share an element never run at the
    /// same time, so the visitor may write to both <c>a</c>'s and <c>b</c>'s own state.
    /// </summary>
    /// <remarks>
    /// The top layer is cut into tiles, and the tiles are searched in four phases; tiles in the same phase are a tile apart,
    /// and an element is only ever paired with elements under its own top cell or the ones around it. Pairs with an
    /// oversized element come last, on the calling thread. Since tiles searched at once share nothing, the result doesn't
    /// depend on how the threads are scheduled, even if the visitor changes the elements it's given. Each worker gets its
    /// own copy of a struct visitor, so write through arrays it holds.
    /// </remarks>
    /// <typeparam name="TVisitor">The visitor; a struct, so the search is compiled for it.</typeparam>
    /// <param name="visitor">Receives the pairs.</param>
    public void ForEachPairInParallel<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor
    {
        int tileSize = _tileSize;
        int tilesX = (Columns + tileSize - 1) / tileSize, tilesY = (Rows + tileSize - 1) / tileSize;
        for (int phase = 0; phase < 4; phase++)
        {
            int offsetX = phase & 1, offsetY = phase >> 1;
            int acrossX = (tilesX - offsetX + 1) / 2, acrossY = (tilesY - offsetY + 1) / 2;
            RunParallel(acrossX * acrossY, tile =>
            {
                TVisitor local = visitor;
                int x = (offsetX + tile % acrossX * 2) * tileSize, y = (offsetY + tile / acrossX * 2) * tileSize;
                PairsInTopCells(ref local, x, y, Math.Min(x + tileSize, Columns), Math.Min(y + tileSize, Rows));
            });
        }

        OversizedPairs(ref visitor);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Each tile of the top layer is one piece of work, for every layer's elements under it, over up to
    /// <see cref="Threads"/> threads; the oversized elements come after. When the visitor can write to both elements of a
    /// pair, <see cref="ForEachPairInParallel{TVisitor}"/> does half the work.
    /// </remarks>
    public void ForEachNeighbor<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor
    {
        int tileSize = _tileSize;
        int tilesX = (Columns + tileSize - 1) / tileSize, tilesY = (Rows + tileSize - 1) / tileSize;
        RunParallel(tilesX * tilesY, tile =>
        {
            TVisitor local = visitor;
            int x = tile % tilesX * tileSize, y = tile / tilesX * tileSize;
            NeighborsInTopCells(ref local, x, y, Math.Min(x + tileSize, Columns), Math.Min(y + tileSize, Rows));
        });

        if (OversizedCount > 0)
        {
            RunParallel(OversizedCount, k =>
            {
                TVisitor local = visitor;
                int element = _oversized[k];
                VisitGridded(ref local, element, _largeBoxes[k]);
                Remap<TVisitor> remapped = new(local, _oversized);
                _large.VisitArea(ref remapped, element, _largeBoxes[k]);
            });
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Reports every top cell, at depth 0, and every cell of the finer layers that holds anything, at its layer's depth, all
    /// as leaves with their element counts. Oversized elements aren't counted.
    /// </remarks>
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        for (int layer = 0; layer < Layers; layer++)
        {
            int first = _layerFirstSlot[layer];
            int columns = _layers.Columns[layer], rows = _layers.Rows[layer];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int slot = first + y * columns + x;
                    int count = _starts[slot + 1] - _starts[slot];
                    if (layer == 0 || count > 0)
                    {
                        visitor(_layers.CellBounds(layer, x, y), layer, true, count);
                    }
                }
            }
        }
    }

    // Works out each box's slot in one chunk, and counts its buckets and how far its boxes reach.
    private void SlotChunk(int chunk)
    {
        int offset = chunk * _buckets;
        Array.Clear(_chunkCounts, offset, _buckets);
        Span<float> reachX = stackalloc float[MaxLayers], reachY = stackalloc float[MaxLayers];
        (int from, int to) = ChunkRange(chunk);
        for (int i = from; i < to; i++)
        {
            Aabb box = _boxes[i];
            int slot;
            if (box.IsEmpty)
            {
                slot = _emptySlot;
            }
            else
            {
                int layer = _layers.LayerFor(box);
                if (layer == 0 && !_layers.Fits(0, box))
                {
                    slot = _oversizedSlot;
                }
                else
                {
                    slot = _layerFirstSlot[layer] + _layers.CellOf(layer, box.CenterX, box.CenterY);
                    reachX[layer] = MathF.Max(reachX[layer], box.Width * 0.5f);
                    reachY[layer] = MathF.Max(reachY[layer], box.Height * 0.5f);
                }
            }

            _slots[i] = slot;
            _chunkCounts[offset + (slot >> _lowBits)]++;
        }

        for (int layer = 0; layer < Layers; layer++)
        {
            _chunkReach[(chunk * MaxLayers + layer) * 2] = reachX[layer];
            _chunkReach[(chunk * MaxLayers + layer) * 2 + 1] = reachY[layer];
        }
    }

    // Puts one chunk's elements in their buckets, where the running totals say. If the buckets are the slots, that's where
    // they stay.
    private void BucketChunk(int chunk)
    {
        int offset = chunk * _buckets;
        (int from, int to) = ChunkRange(chunk);
        if (_lowBits == 0)
        {
            for (int i = from; i < to; i++)
            {
                _sorted.Set(_chunkCounts[offset + _slots[i]]++, i, _boxes[i]);
            }

            return;
        }

        for (int i = from; i < to; i++)
        {
            int slot = _slots[i];
            _entries[_chunkCounts[offset + (slot >> _lowBits)]++] = (long)slot << 32 | (uint)i;
        }
    }

    // Sorts the buckets that start in one chunk's share of the positions (the last chunk also takes any empty ones at the
    // end), each by a count of its slots: the counts' running totals are where the slots' runs start.
    private void SortBucketsChunk(int chunk)
    {
        (int from, int to) = ChunkRange(chunk);
        int firstBucket = FirstBucketFrom(from), endBucket = chunk == _chunks - 1 ? _buckets : FirstBucketFrom(to);
        int bucketSlots = 1 << _lowBits, lowMask = bucketSlots - 1, slots = _starts.Length - 1;
        Span<int> counts = _slotCounts.AsSpan(chunk << _lowBits, bucketSlots);
        for (int bucket = firstBucket; bucket < endBucket; bucket++)
        {
            int start = _bucketStarts[bucket], end = _bucketStarts[bucket + 1];
            counts.Clear();
            for (int p = start; p < end; p++)
            {
                counts[(int)(_entries[p] >> 32) & lowMask]++;
            }

            int firstSlot = bucket << _lowBits, position = start;
            for (int low = 0; low < bucketSlots; low++)
            {
                if (firstSlot + low < slots)
                {
                    _starts[firstSlot + low] = position;
                }

                int slotCount = counts[low];
                counts[low] = position;
                position += slotCount;
            }

            for (int p = start; p < end; p++)
            {
                long entry = _entries[p];
                int element = (int)entry;
                _sorted.Set(counts[(int)(entry >> 32) & lowMask]++, element, _boxes[element]);
            }
        }
    }

    // The first bucket that starts at or after a position.
    private int FirstBucketFrom(int position)
    {
        int low = 0, high = _buckets;
        while (low < high)
        {
            int middle = (low + high) / 2;
            if (_bucketStarts[middle] < position)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        return low;
    }

    private (int From, int To) ChunkRange(int chunk) => ((int)((long)Count * chunk / _chunks), (int)((long)Count * (chunk + 1) / _chunks));

    private void RunChunks(Action<int> body) => RunParallel(_chunks, body);

    private void RunParallel(int count, Action<int> body)
    {
        if (_threads == 1 || count <= 1)
        {
            for (int i = 0; i < count; i++)
            {
                body(i);
            }

            return;
        }

        Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = _threads }, body);
    }

    // The pairs owned by the elements under a block of top cells, in every layer, each found once: in its own layer from the
    // element that comes first, and across layers from the bigger one. The other element is always under one of the top
    // cells around the block.
    private void PairsInTopCells<TVisitor>(ref TVisitor visitor, int topX1, int topY1, int topX2, int topY2) where TVisitor : IPairVisitor
    {
        for (int layer = 0; layer < Layers; layer++)
        {
            if (IsEmpty(layer))
            {
                continue;
            }

            int first = _layerFirstSlot[layer], columns = _layers.Columns[layer];
            float reachX = _reachX[layer], reachY = _reachY[layer];
            int x1 = topX1 << layer, x2 = topX2 << layer;
            for (int y = topY1 << layer; y < topY2 << layer; y++)
            {
                int row = first + y * columns;
                for (int p = _starts[row + x1]; p < _starts[row + x2]; p++)
                {
                    int element = _sorted.ElementAt(p);
                    Aabb box = _sorted.BoxAt(p);

                    // The rest of this row as far as the box reaches, which is one run, then the rows below.
                    GridCells.Range range = _layers.Around(layer, box, reachX, reachY);
                    _sorted.VisitRun(ref visitor, element, box, p + 1, _starts[row + range.X2 + 1], -1);
                    for (int below = y + 1; below <= range.Y2; below++)
                    {
                        int belowRow = first + below * columns;
                        _sorted.VisitRun(ref visitor, element, box, _starts[belowRow + range.X1], _starts[belowRow + range.X2 + 1], -1);
                    }

                    for (int finer = layer + 1; finer < Layers; finer++)
                    {
                        VisitLayer(ref visitor, finer, element, box, -1);
                    }
                }
            }
        }
    }

    // Every element under a block of top cells against everything it touches, both ways round.
    private void NeighborsInTopCells<TVisitor>(ref TVisitor visitor, int topX1, int topY1, int topX2, int topY2) where TVisitor : IPairVisitor
    {
        Remap<TVisitor> remapped = new(visitor, _oversized);
        for (int layer = 0; layer < Layers; layer++)
        {
            if (IsEmpty(layer))
            {
                continue;
            }

            int first = _layerFirstSlot[layer], columns = _layers.Columns[layer];
            int x1 = topX1 << layer, x2 = topX2 << layer;
            for (int y = topY1 << layer; y < topY2 << layer; y++)
            {
                int row = first + y * columns;
                for (int p = _starts[row + x1]; p < _starts[row + x2]; p++)
                {
                    int element = _sorted.ElementAt(p);
                    Aabb box = _sorted.BoxAt(p);
                    for (int other = 0; other < Layers; other++)
                    {
                        VisitLayer(ref visitor, other, element, box, other == layer ? p : -1);
                    }

                    if (OversizedCount > 0)
                    {
                        _large.VisitArea(ref remapped, element, box);
                    }
                }
            }
        }
    }

    // Every pair with an oversized element in it, each once: the oversized element against the grid, then the side tree's own pairs.
    private void OversizedPairs<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor
    {
        if (OversizedCount == 0)
        {
            return;
        }

        for (int k = 0; k < OversizedCount; k++)
        {
            VisitGridded(ref visitor, _oversized[k], _largeBoxes[k]);
        }

        RemapBoth<TVisitor> remapped = new(visitor, _oversized);
        _large.ForEachPair(ref remapped);
        visitor = remapped.Visitor;
    }

    // Visits (element, e) for each element e in the grid, in any layer, whose box intersects the box.
    private void VisitGridded<TVisitor>(ref TVisitor visitor, int element, Aabb box) where TVisitor : IPairVisitor
    {
        for (int layer = 0; layer < Layers; layer++)
        {
            VisitLayer(ref visitor, layer, element, box, -1);
        }
    }

    // Visits (element, e) for each element e in a layer whose box intersects the box, other than the one at position skip.
    private void VisitLayer<TVisitor>(ref TVisitor visitor, int layer, int element, Aabb box, int skip) where TVisitor : IPairVisitor
    {
        if (IsEmpty(layer))
        {
            return;
        }

        int first = _layerFirstSlot[layer], columns = _layers.Columns[layer];
        GridCells.Range range = _layers.Around(layer, box, _reachX[layer], _reachY[layer]);
        for (int y = range.Y1; y <= range.Y2; y++)
        {
            int row = first + y * columns;
            _sorted.VisitRun(ref visitor, element, box, _starts[row + range.X1], _starts[row + range.X2 + 1], skip);
        }
    }

    private bool IsEmpty(int layer) => _starts[_layerFirstSlot[layer]] == _starts[_layerFirstSlot[layer + 1]];

    /// <summary>Passes pairs on with the second element, a position in the side tree, turned back into an element; skips (e, e).</summary>
    private struct Remap<TVisitor>(TVisitor visitor, int[] oversized) : IPairVisitor where TVisitor : IPairVisitor
    {
        private TVisitor _visitor = visitor;

        public void Visit(int a, int b)
        {
            int other = oversized[b];
            if (other != a)
            {
                _visitor.Visit(a, other);
            }
        }
    }

    /// <summary>Passes pairs from the side tree on with both elements turned back into elements.</summary>
    private struct RemapBoth<TVisitor>(TVisitor visitor, int[] oversized) : IPairVisitor where TVisitor : IPairVisitor
    {
        public TVisitor Visitor = visitor;

        public void Visit(int a, int b) => Visitor.Visit(oversized[a], oversized[b]);
    }

    /// <summary>Adds elements from the side tree to a query's results.</summary>
    private readonly struct LargeCollector(List<int> results, int[] oversized, int omitElement) : IPairVisitor
    {
        public void Visit(int a, int b)
        {
            int element = oversized[b];
            if (element != omitElement)
            {
                results.Add(element);
            }
        }
    }
}
