namespace ALife.Core.Shapes
{
    /// <summary>
    /// An interface defining that an object has a shape associated with it
    /// </summary>
    public interface IHasShape
    {
        /// <summary>
        /// Gets the shape associated with the object.
        /// </summary>
        /// <value>The shape.</value>
        Shape Shape
        {
            get;
        }
    }
}
