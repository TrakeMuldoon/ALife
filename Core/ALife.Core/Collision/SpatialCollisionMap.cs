using ALife.Core.Geometry.Shapes;
using ALife.Spatial;
using System;
using System.Collections.Generic;

namespace ALife.Core.Collision
{
    /// <summary>
    /// A collision map backed by an <see cref="ALife.Spatial.LayeredGrid"/>: small objects go in small cells and big ones, like
    /// walls and zones, in big ones, so neither slows the other down. Queries allocate only the list they return, and removing
    /// an object is O(1). Fine-grained collisions go through <see cref="CollisionDetector"/>, so every ALife shape works.
    /// </summary>
    /// <remarks>
    /// The grid stores each object's box where it is at its <see cref="Insert"/> or <see cref="MoveObject"/>, worked out from
    /// its centre, orientation and size rather than the box ALife's shapes cache until <see cref="IShape.Reset"/>. So an object
    /// has to be passed to <see cref="MoveObject"/> after every move, or queries look for it where it was.
    /// </remarks>
    /// <typeparam name="T">The objects on the map.</typeparam>
    public sealed class SpatialCollisionMap<T> : ICollisionMap<T> where T : class, IHasShape
    {
        /// <summary>The finest cells are about this big.</summary>
        public const int FineCellSize = 25;

        private const int Layers = 4;

        // The handles found by a query, for each thread, since the grid can be queried from several at once.
        [ThreadStatic]
        private static List<int>? _found;

        public readonly int Height;
        public readonly int Width;
        public readonly string GridName;

        private readonly LayeredGrid _grid;

        // Each object's handle and place in _objects, the objects in the order EnumerateItems gives them, and the object and
        // its stored box for each handle.
        private readonly Dictionary<T, Entry> _entries = new Dictionary<T, Entry>();
        private readonly List<T> _objects = new List<T>();
        private readonly List<T?> _byHandle = new List<T?>();
        private readonly List<Aabb> _boxByHandle = new List<Aabb>();

        /// <summary>Creates an empty map covering a world.</summary>
        /// <param name="gridHeight">The world's height.</param>
        /// <param name="gridWidth">The world's width.</param>
        /// <param name="gridName">A name for the map, such as its collision level.</param>
        public SpatialCollisionMap(int gridHeight, int gridWidth, string gridName)
        {
            Height = gridHeight;
            Width = gridWidth;
            GridName = gridName;

            // The top layer's cells are 2^(Layers - 1) times the finest ones.
            int topCellSize = FineCellSize << (Layers - 1);
            int columns = Math.Max(1, (int)Math.Ceiling(gridWidth / (double)topCellSize));
            int rows = Math.Max(1, (int)Math.Ceiling(gridHeight / (double)topCellSize));
            _grid = new LayeredGrid(new Aabb(0, 0, Math.Max(1, gridWidth), Math.Max(1, gridHeight)), columns, rows, Layers, threads: 1);
        }

        /// <summary>How many objects are on the map.</summary>
        public int Count => _objects.Count;

        public bool Insert(T newObject)
        {
            Aabb box = BoxOf(newObject.Shape);
            int handle = _grid.Insert(0, box);
            _entries.Add(newObject, new Entry(handle, _objects.Count));
            _objects.Add(newObject);
            while(_byHandle.Count <= handle)
            {
                _byHandle.Add(null);
                _boxByHandle.Add(Aabb.Empty);
            }

            _byHandle[handle] = newObject;
            _boxByHandle[handle] = box;
            return true;
        }

        public void RemoveObject(T killMe)
        {
            Entry entry = _entries[killMe];
            _entries.Remove(killMe);

            int oversized = _grid.OversizedCount;
            _grid.Remove(entry.Handle);
            _byHandle[entry.Handle] = null;
            if(_grid.OversizedCount < oversized)
            {
                _grid.Cleanup();
            }

            // Fill the gap with the last object, so removing doesn't shift the rest.
            int last = _objects.Count - 1;
            if(entry.Position != last)
            {
                T moved = _objects[last];
                _objects[entry.Position] = moved;
                _entries[moved] = new Entry(_entries[moved].Handle, entry.Position);
            }

            _objects.RemoveAt(last);
        }

        public void MoveObject(T moveMe)
        {
            int handle = _entries[moveMe].Handle;
            Aabb box = BoxOf(moveMe.Shape);
            _grid.Move(handle, box);
            _boxByHandle[handle] = box;
        }

        public List<T> QueryForBoundingBoxCollisions(T queryObject)
        {
            return Query(BoxOf(queryObject.Shape), HandleOf(queryObject));
        }

        public List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox)
        {
            return Query(ToAabb(queryBox), -1);
        }

        public List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox, T self)
        {
            return Query(ToAabb(queryBox), HandleOf(self));
        }

        public List<T> DetectCollisions(T self)
        {
            return DetectCollisions(self, self);
        }

        public List<T> DetectCollisions(IHasShape detector, T self)
        {
            // Outlines are relative to the detector, so they keep their precision anywhere in the world.
            Point origin = detector.Shape.CentrePoint;
            ShapeGeometry mine = ShapeGeometry.From(detector.Shape, origin);
            Aabb relative = mine.Bounds;
            float originX = (float)origin.X, originY = (float)origin.Y;
            Aabb queryBox = new Aabb(relative.Left + originX, relative.Top + originY, relative.Right + originX, relative.Bottom + originY);

            List<int> found = Find(queryBox, HandleOf(self));
            List<T> results = new List<T>();
            foreach(int handle in found)
            {
                T? candidate = _byHandle[handle];
                if(candidate != null && Overlaps(_boxByHandle[handle], queryBox) && mine.Overlaps(ShapeGeometry.From(candidate.Shape, origin)))
                {
                    results.Add(candidate);
                }
            }

            return results;
        }

        public IEnumerable<T> EnumerateItems()
        {
            // By index, and forgiving: renderers enumerate while the simulation adds and removes.
            int i = 0;
            while(i < _objects.Count)
            {
                T item;
                try
                {
                    item = _objects[i++];
                }
                catch(ArgumentOutOfRangeException)
                {
                    yield break;
                }

                yield return item;
            }
        }

        private List<T> Query(Aabb queryBox, int omit)
        {
            List<int> found = Find(queryBox, omit);
            List<T> results = new List<T>(found.Count);
            foreach(int handle in found)
            {
                T? candidate = _byHandle[handle];
                if(candidate != null && Overlaps(_boxByHandle[handle], queryBox))
                {
                    results.Add(candidate);
                }
            }

            return results;
        }

        // The handles of the objects whose stored boxes overlap or touch a box, in this thread's list.
        private List<int> Find(Aabb queryBox, int omit)
        {
            List<int> found = _found ??= new List<int>();
            found.Clear();
            _grid.Query(queryBox, found, omit);
            return found;
        }

        private int HandleOf(T item)
        {
            return _entries.TryGetValue(item, out Entry entry) ? entry.Handle : -1;
        }

        // As BoundingBox.IsCollision, only boxes that overlap count, and not ones that just touch.
        private static bool Overlaps(Aabb a, Aabb b)
        {
            return a.Left < b.Right && b.Left < a.Right && a.Top < b.Bottom && b.Top < a.Bottom;
        }

        // A shape's box where it is now, rather than the one it caches.
        private static Aabb BoxOf(IShape shape)
        {
            return ShapeGeometry.From(shape, default).Bounds;
        }

        private static Aabb ToAabb(BoundingBox box)
        {
            return new Aabb((float)box.MinX, (float)box.MinY, (float)box.MaxX, (float)box.MaxY);
        }

        private readonly record struct Entry(int Handle, int Position);
    }
}
