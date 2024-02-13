using System;
using ALife.Core.Shapes;

namespace ALife.Core.CollisionDetection
{
    /// <summary>
    /// A class to marshal collision detections
    /// Note: this is a singleton because we only need to define collision marshalling rules once
    /// </summary>
    public sealed class CollisionMarshaller
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static readonly Lazy<CollisionMarshaller> _lazy = new Lazy<CollisionMarshaller>(() => new CollisionMarshaller());

        /// <summary>
        /// Prevents a default instance of the <see cref="CollisionMarshaller"/> class from being created.
        /// </summary>
        private CollisionMarshaller()
        {
        }

        /// <summary>
        /// Gets the marshal.
        /// </summary>
        /// <value>The marshal.</value>
        public static CollisionMarshaller Marshal => _lazy.Value;

        /// <summary>
        /// Checks the collision.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsCollision(Shape obj1, Shape obj2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether [is collision objects] [the specified obj1].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns><c>true</c> if [is collision objects] [the specified obj1]; otherwise, <c>false</c>.</returns>
        public bool IsCollision<T, S>(T obj1, S obj2) where T : IHasShape where S : IHasShape
        {
            return IsCollision(obj1.Shape, obj2.Shape);
        }
    }
}
