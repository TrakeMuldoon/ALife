namespace ALife.Spatial;

/// <summary>
/// A spatial index over boxes for broad-phase collision detection: it finds the elements whose boxes may intersect an area,
/// and moving an element is cheap. Elements are referred to by the handle <see cref="Insert"/> returns.
/// </summary>
/// <remarks>
/// Not thread-safe, not even for concurrent queries: queries reuse scratch buffers so they don't allocate. A handle stays the
/// same while the element is moved, and is reused after <see cref="Remove"/>; using a removed handle is undefined.
/// </remarks>
public interface ISpatialIndex
{
    /// <summary>The area the index covers. Elements outside it still work, but are all filed at its edges.</summary>
    Aabb Bounds { get; }

    /// <summary>How many elements are in the index.</summary>
    int Count { get; }

    /// <summary>Adds an element.</summary>
    /// <param name="id">Your ID for the element, e.g. an index into your own array. It isn't interpreted and needn't be unique.</param>
    /// <param name="bounds">The element's box.</param>
    /// <returns>The element's handle, for <see cref="Move"/>, <see cref="Remove"/> and the results of <see cref="Query"/>.</returns>
    int Insert(int id, Aabb bounds);

    /// <summary>Removes an element. Empty nodes are only tidied up by <see cref="Cleanup"/>.</summary>
    /// <param name="element">The element's handle.</param>
    void Remove(int element);

    /// <summary>Changes an element's box, keeping its handle.</summary>
    /// <param name="element">The element's handle.</param>
    /// <param name="bounds">The new box.</param>
    void Move(int element, Aabb bounds);

    /// <summary>The ID an element was inserted with.</summary>
    /// <param name="element">The element's handle.</param>
    /// <returns>The ID.</returns>
    int GetId(int element);

    /// <summary>The element's box as the index stores it, which may be rounded or, for <see cref="UniformGrid"/>, the uniform size.</summary>
    /// <param name="element">The element's handle.</param>
    /// <returns>The box.</returns>
    Aabb GetBounds(int element);

    /// <summary>Adds the handles of the elements whose stored boxes intersect <paramref name="area"/> to <paramref name="results"/>, each once.</summary>
    /// <param name="area">The area to search.</param>
    /// <param name="results">The list to add to. It isn't cleared first.</param>
    /// <param name="omitElement">An element to leave out, usually the one you're finding collisions for, or -1.</param>
    void Query(Aabb area, List<int> results, int omitElement = -1);

    /// <summary>
    /// Tidies up after removals and moves: frees empty nodes and, for loose structures, shrinks boxes to fit their elements
    /// again. Call it once a frame, after moving everything.
    /// </summary>
    void Cleanup();

    /// <summary>Removes every element and resets the structure.</summary>
    void Clear();

    /// <summary>Calls <paramref name="visitor"/> for each node or cell, e.g. to draw the structure. The visitor mustn't change the index.</summary>
    /// <param name="visitor">The callback.</param>
    void VisitNodes(SpatialNodeVisitor visitor);
}
