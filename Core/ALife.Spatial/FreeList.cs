using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ALife.Spatial;

/// <summary>
/// An array whose slots can be erased from anywhere in constant time without moving the rest, so indices stay valid; later
/// inserts reuse erased slots. An erased slot's first four bytes hold the next free index, as the C version's union does.
/// </summary>
internal sealed class FreeList<T> where T : unmanaged
{
    private T[] _items = new T[16];
    private int _firstFree = -1;

    public FreeList()
    {
        Debug.Assert(Unsafe.SizeOf<T>() >= sizeof(int), "An erased slot must have room for the next free index.");
    }

    /// <summary>How many slots are in use.</summary>
    public int Count { get; private set; }

    /// <summary>One past the highest index ever handed out: every index in use is below it.</summary>
    public int Range { get; private set; }

    public ref T this[int index] => ref _items[index];

    public int Insert(T item)
    {
        int index;
        if (_firstFree != -1)
        {
            index = _firstFree;
            _firstFree = Unsafe.As<T, int>(ref _items[index]);
        }
        else
        {
            if (Range == _items.Length)
            {
                Array.Resize(ref _items, _items.Length * 2);
            }

            index = Range++;
        }

        _items[index] = item;
        Count++;
        return index;
    }

    public void Erase(int index)
    {
        Unsafe.As<T, int>(ref _items[index]) = _firstFree;
        _firstFree = index;
        Count--;
    }

    public void Clear()
    {
        Range = 0;
        Count = 0;
        _firstFree = -1;
    }
}
