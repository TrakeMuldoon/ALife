namespace ALife.Core.Utility;

/// <summary>
/// Various helpers for lists.
/// </summary>
public static class ListHelpers
{
    /// <summary>
    /// Adds individuals to a starting list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lists">The lists.</param>
    /// <param name="individuals">The individuals.</param>
    /// <returns>The compiled list</returns>
    public static List<T> CompileList<T>(IEnumerable<T>[] lists, params T[] individuals)
    {
        List<T> toReturn = individuals.ToList();
        if(lists != null)
        {
            foreach(IEnumerable<T> list in lists)
            {
                toReturn.AddRange(list);
            }
        }
        return toReturn;
    }

    /// <summary>
    /// Adds individuals to a starting list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="individuals">The individuals.</param>
    /// <returns>The compiled list</returns>
    public static List<T> CompileList<T>(params T[] individuals)
    {
        List<T> toReturn = individuals.ToList();
        return toReturn;
    }
}