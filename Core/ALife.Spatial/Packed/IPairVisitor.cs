namespace ALife.Spatial.Packed;

/// <summary>Receives pairs of elements whose boxes intersect, from an <see cref="IPackedSpatialIndex"/> or a <see cref="LayeredGrid"/>.</summary>
/// <remarks>
/// Implement it on a struct: the index's methods are generic over the visitor, so the JIT compiles a copy of the search for
/// each struct visitor and can inline <see cref="Visit"/> into it. A class works too, through an interface call per pair.
/// </remarks>
public interface IPairVisitor
{
    /// <summary>Called for one pair.</summary>
    /// <param name="a">
    /// One element: its index in the boxes given to <see cref="IPackedSpatialIndex.Build"/>, or its handle in a
    /// <see cref="LayeredGrid"/>.
    /// </param>
    /// <param name="b">The other element.</param>
    void Visit(int a, int b);
}
