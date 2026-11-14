using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace ALife.Spatial.Packed;

/// <summary>
/// Boxes in the order a packed index sorted them, one array per edge so runs of them can be tested four at a time, with the
/// element each came from.
/// </summary>
internal sealed class PackedBoxes
{
    private float[] _left = [], _top = [], _right = [], _bottom = [];
    private int[] _elements = [];

    public int Capacity => _elements.Length;

    /// <summary>The element whose box is at a position.</summary>
    public int ElementAt(int position) => _elements[position];

    /// <summary>The elements at the first <paramref name="count"/> positions, in order.</summary>
    public ReadOnlySpan<int> Elements(int count) => _elements.AsSpan(0, count);

    public void EnsureCapacity(int count)
    {
        if (count > _elements.Length)
        {
            int capacity = Math.Max(count, _elements.Length * 2);
            _left = new float[capacity];
            _top = new float[capacity];
            _right = new float[capacity];
            _bottom = new float[capacity];
            _elements = new int[capacity];
        }
    }

    public void Set(int position, int element, Aabb box)
    {
        _left[position] = box.Left;
        _top[position] = box.Top;
        _right[position] = box.Right;
        _bottom[position] = box.Bottom;
        _elements[position] = element;
    }

    public Aabb BoxAt(int position) => new(_left[position], _top[position], _right[position], _bottom[position]);

    /// <summary>
    /// Calls the visitor with (<paramref name="element"/>, e) for each element e at positions <paramref name="from"/> to
    /// <paramref name="to"/> (exclusive), other than position <paramref name="skip"/>, whose box intersects <paramref name="box"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void VisitRun<TVisitor>(ref TVisitor visitor, int element, Aabb box, int from, int to, int skip)
        where TVisitor : IPairVisitor
    {
        // The callers keep runs inside the arrays, so the bounds checks can go.
        ref float left = ref MemoryMarshal.GetArrayDataReference(_left);
        ref float top = ref MemoryMarshal.GetArrayDataReference(_top);
        ref float right = ref MemoryMarshal.GetArrayDataReference(_right);
        ref float bottom = ref MemoryMarshal.GetArrayDataReference(_bottom);
        ref int elements = ref MemoryMarshal.GetArrayDataReference(_elements);
        int q = from;
        if (Vector128.IsHardwareAccelerated)
        {
            Vector128<float> boxLeft = Vector128.Create(box.Left), boxTop = Vector128.Create(box.Top);
            Vector128<float> boxRight = Vector128.Create(box.Right), boxBottom = Vector128.Create(box.Bottom);
            for (; q <= to - 4; q += 4)
            {
                Vector128<float> hits =
                    Vector128.LessThanOrEqual(Vector128.LoadUnsafe(ref left, (nuint)q), boxRight)
                    & Vector128.GreaterThanOrEqual(Vector128.LoadUnsafe(ref right, (nuint)q), boxLeft)
                    & Vector128.LessThanOrEqual(Vector128.LoadUnsafe(ref top, (nuint)q), boxBottom)
                    & Vector128.GreaterThanOrEqual(Vector128.LoadUnsafe(ref bottom, (nuint)q), boxTop);
                for (uint bits = hits.ExtractMostSignificantBits(); bits != 0; bits &= bits - 1)
                {
                    int hit = q + BitOperations.TrailingZeroCount(bits);
                    if (hit != skip)
                    {
                        visitor.Visit(element, Unsafe.Add(ref elements, hit));
                    }
                }
            }
        }

        for (; q < to; q++)
        {
            if (Unsafe.Add(ref left, q) <= box.Right && box.Left <= Unsafe.Add(ref right, q)
                && Unsafe.Add(ref top, q) <= box.Bottom && box.Top <= Unsafe.Add(ref bottom, q) && q != skip)
            {
                visitor.Visit(element, Unsafe.Add(ref elements, q));
            }
        }
    }

    /// <summary>Adds the elements at positions <paramref name="from"/> to <paramref name="to"/> (exclusive) whose boxes intersect the area.</summary>
    public void AddRun(List<int> results, Aabb area, int from, int to, int omitElement)
    {
        Collector collector = new(results, omitElement);
        VisitRun(ref collector, -1, area, from, to, -1);
    }

    /// <summary>Adds each element it's given to a list, unless it's the one to leave out.</summary>
    private readonly struct Collector(List<int> results, int omitElement) : IPairVisitor
    {
        public void Visit(int a, int b)
        {
            if (b != omitElement)
            {
                results.Add(b);
            }
        }
    }
}
