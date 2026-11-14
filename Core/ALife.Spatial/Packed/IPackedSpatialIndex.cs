namespace ALife.Spatial.Packed;

/// <summary>
/// A spatial index that's rebuilt from scratch every frame instead of updated element by element. Each build sorts the
/// boxes into flat arrays, so searching them streams through memory, and nothing is kept between frames to tidy up. It finds
/// every intersecting pair in one pass, in parallel if you like, which is what lets it keep up with hundreds of thousands of
/// moving elements where an <see cref="ISpatialIndex"/> can't.
/// </summary>
/// <remarks>
/// Elements are the positions of their boxes in the span given to <see cref="Build"/>: there are no handles or IDs, so keep
/// your elements in an array and build from their boxes in the same order. <see cref="Build"/> isn't thread-safe; once it's
/// done, <see cref="Query"/>, <see cref="ForEachPair{TVisitor}"/> and <see cref="ForEachNeighbor{TVisitor}"/> only read, so they
/// can run on several threads at once.
/// </remarks>
public interface IPackedSpatialIndex
{
    /// <summary>The area the index covers. Elements outside it still work, but are all filed at its edges.</summary>
    Aabb Bounds { get; }

    /// <summary>How many elements the last <see cref="Build"/> was given, empty boxes included.</summary>
    int Count { get; }

    /// <summary>Replaces the contents with these boxes. Element <c>i</c> is <c>boxes[i]</c>; an empty box is kept but never found.</summary>
    /// <param name="boxes">The elements' boxes. They're copied, so the span can change afterwards.</param>
    void Build(ReadOnlySpan<Aabb> boxes);

    /// <summary>An element's box, as given to <see cref="Build"/>.</summary>
    /// <param name="element">The element.</param>
    /// <returns>The box.</returns>
    Aabb GetBounds(int element);

    /// <summary>Adds the elements whose boxes intersect <paramref name="area"/> to <paramref name="results"/>, each once.</summary>
    /// <param name="area">The area to search.</param>
    /// <param name="results">The list to add to. It isn't cleared first.</param>
    /// <param name="omitElement">An element to leave out, or -1.</param>
    void Query(Aabb area, List<int> results, int omitElement = -1);

    /// <summary>Calls <paramref name="visitor"/> once for each pair of elements whose boxes intersect, on this thread.</summary>
    /// <param name="visitor">The visitor. It's passed by reference, so it can count or collect as it goes.</param>
    /// <typeparam name="TVisitor">The visitor's type; a struct, ideally.</typeparam>
    void ForEachPair<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor;

    /// <summary>
    /// Calls <paramref name="visitor"/> with <c>(a, b)</c> for every element <c>a</c> and each other element <c>b</c> whose box
    /// intersects it, spread over the thread pool. Each pair comes up twice, once from each side.
    /// </summary>
    /// <remarks>
    /// Calls with the same <c>a</c> never run at the same time, so a visitor can safely update element <c>a</c>'s own state,
    /// as long as it only reads the others' (keep what you're writing separate from what you're reading, or results depend
    /// on timing). Each worker gets its own copy of a struct visitor, so changes to its fields are lost; write through arrays
    /// it refers to. Returns when every call has finished.
    /// </remarks>
    /// <param name="visitor">The visitor.</param>
    /// <typeparam name="TVisitor">The visitor's type; a struct, ideally.</typeparam>
    void ForEachNeighbor<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor;

    /// <summary>Calls <paramref name="visitor"/> for each node or cell, e.g. to draw the structure.</summary>
    /// <param name="visitor">The callback.</param>
    void VisitNodes(SpatialNodeVisitor visitor);
}
