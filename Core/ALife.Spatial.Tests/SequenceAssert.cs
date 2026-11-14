namespace ALife.Spatial.Tests;

/// <summary>Compares sequences element by element, as MSTest's <see cref="Assert.AreEqual{T}(T, T)"/> doesn't.</summary>
internal static class SequenceAssert
{
    public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual) =>
        CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray());
}
