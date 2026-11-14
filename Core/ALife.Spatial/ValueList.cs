namespace ALife.Spatial;

/// <summary>A growable array of structs with access by reference; also a stack. It's kept and reused so traversals don't allocate.</summary>
internal sealed class ValueList<T> where T : struct
{
    private T[] _items = new T[16];

    public int Count { get; private set; }

    public ref T this[int index] => ref _items[index];

    public int Add(T item)
    {
        if (Count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * 2);
        }

        _items[Count] = item;
        return Count++;
    }

    public T Pop() => _items[--Count];

    public void Clear() => Count = 0;
}
