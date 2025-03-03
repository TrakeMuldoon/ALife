namespace ALife.Core.Interfaces
{
    /// <summary>
    /// An interface for objects that can be deep-cloned.
    /// NOTE: We could use System.ICloneable, but that also has no guarantee of deep cloning...and this way it is more
    ///       heavily implied.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public interface IDeepCloneable<out T>
    {
        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        T Clone();
    }
}