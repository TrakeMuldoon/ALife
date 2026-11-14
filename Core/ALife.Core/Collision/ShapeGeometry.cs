using ALife.Core.Geometry.Shapes;
using ALife.Spatial;
using ALife.Spatial.Collisions;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SpatialSector = ALife.Spatial.Shapes.Sector;

namespace ALife.Core.Collision
{
    /// <summary>
    /// An ALife shape's outline as the narrow phase sees it: a circle, a four-cornered polygon, or a sector, in floats relative
    /// to an origin near it, so worlds far from zero don't lose precision. It's worked out from the shape's centre, orientation
    /// and size as they are now, not from the corners and box the shape caches until <see cref="IShape.Reset"/>.
    /// </summary>
    internal readonly struct ShapeGeometry
    {
        private readonly GeometryKind _kind;
        private readonly Vector2 _center;
        private readonly float _radius;
        private readonly Corners _corners;
        private readonly SpatialSector _sector;

        private ShapeGeometry(GeometryKind kind, Vector2 center, float radius, Corners corners, SpatialSector sector)
        {
            _kind = kind;
            _center = center;
            _radius = radius;
            _corners = corners;
            _sector = sector;
        }

        /// <summary>The smallest box around the outline, relative to the origin it was made with.</summary>
        public Aabb Bounds
        {
            get
            {
                switch(_kind)
                {
                    case GeometryKind.Circle:
                        return Aabb.FromCenter(_center.X, _center.Y, _radius, _radius);
                    case GeometryKind.Sector:
                        return _sector.GetBounds();
                    default:
                        ReadOnlySpan<Vector2> corners = _corners;
                        Aabb bounds = Aabb.Empty;
                        foreach(Vector2 corner in corners)
                        {
                            bounds = bounds.Union(new Aabb(corner.X, corner.Y, corner.X, corner.Y));
                        }

                        return bounds;
                }
            }
        }

        /// <summary>Works out a shape's outline as it is now.</summary>
        /// <param name="shape">The shape: any circle, rectangle, axis-aligned rectangle or sector, child shapes included.</param>
        /// <param name="origin">The point the outline is relative to; the same for every outline compared with it.</param>
        /// <returns>The outline.</returns>
        public static ShapeGeometry From(IShape shape, Point origin)
        {
            switch(shape.GetShapeEnum())
            {
                case ShapesEnum.Circle:
                {
                    Circle circle = (Circle)shape;
                    return new ShapeGeometry(GeometryKind.Circle, Relative(circle.CentrePoint, origin), circle.Radius, default, default);
                }
                case ShapesEnum.Rectangle:
                {
                    Rectangle rectangle = (Rectangle)shape;
                    Point centre = rectangle.CentrePoint;
                    double radians = rectangle.Orientation.Radians;
                    double halfLength = rectangle.FBLength / 2, halfWidth = rectangle.RLWidth / 2;

                    // Forward is along the orientation, and the sides are a quarter turn either way of it.
                    double forwardX = Math.Cos(radians) * halfLength, forwardY = Math.Sin(radians) * halfLength;
                    double sideX = -Math.Sin(radians) * halfWidth, sideY = Math.Cos(radians) * halfWidth;
                    double x = centre.X - origin.X, y = centre.Y - origin.Y;
                    Corners corners = default;
                    corners[0] = new Vector2((float)(x + forwardX - sideX), (float)(y + forwardY - sideY));
                    corners[1] = new Vector2((float)(x + forwardX + sideX), (float)(y + forwardY + sideY));
                    corners[2] = new Vector2((float)(x - forwardX + sideX), (float)(y - forwardY + sideY));
                    corners[3] = new Vector2((float)(x - forwardX - sideX), (float)(y - forwardY - sideY));
                    return new ShapeGeometry(GeometryKind.Polygon, default, 0, corners, default);
                }
                case ShapesEnum.AARectangle:
                {
                    AARectangle box = (AARectangle)shape;
                    Vector2 topLeft = Relative(box.TopLeft, origin);
                    float width = (float)box.XWidth, height = (float)box.YHeight;
                    Corners corners = default;
                    corners[0] = topLeft;
                    corners[1] = new Vector2(topLeft.X + width, topLeft.Y);
                    corners[2] = new Vector2(topLeft.X + width, topLeft.Y + height);
                    corners[3] = new Vector2(topLeft.X, topLeft.Y + height);
                    return new ShapeGeometry(GeometryKind.Polygon, default, 0, corners, default);
                }
                case ShapesEnum.Sector:
                {
                    Sector sector = (Sector)shape;
                    SpatialSector spatial = new SpatialSector(
                        Relative(sector.CentrePoint, origin), (float)sector.Orientation.Radians, (float)sector.SweepAngle.Radians, sector.Radius);
                    return new ShapeGeometry(GeometryKind.Sector, default, 0, default, spatial);
                }
                default:
                    throw new NotSupportedException("No collision outline for shape " + shape.GetShapeEnum());
            }
        }

        /// <summary>Whether this outline and another, made with the same origin, overlap or touch.</summary>
        /// <param name="other">The other outline.</param>
        /// <returns>True if they overlap or touch.</returns>
        public bool Overlaps(in ShapeGeometry other)
        {
            // Order the pair so the first is the earlier kind, which halves the cases.
            if(_kind > other._kind)
            {
                return other.Overlaps(this);
            }

            ReadOnlySpan<Vector2> corners = _corners;
            ReadOnlySpan<Vector2> otherCorners = other._corners;
            switch(_kind, other._kind)
            {
                case (GeometryKind.Circle, GeometryKind.Circle):
                    return Overlap.Circles(_center, _radius, other._center, other._radius);
                case (GeometryKind.Circle, GeometryKind.Polygon):
                    return Overlap.CirclePolygon(_center, _radius, otherCorners);
                case (GeometryKind.Circle, GeometryKind.Sector):
                    return Overlap.SectorCircle(other._sector, _center, _radius);
                case (GeometryKind.Polygon, GeometryKind.Polygon):
                    return Overlap.Polygons(corners, otherCorners);
                case (GeometryKind.Polygon, GeometryKind.Sector):
                    return Overlap.SectorPolygon(other._sector, corners);
                default:
                    return Overlap.Sectors(_sector, other._sector);
            }
        }

        private static Vector2 Relative(Point point, Point origin)
        {
            return new Vector2((float)(point.X - origin.X), (float)(point.Y - origin.Y));
        }

        private enum GeometryKind
        {
            Circle,
            Polygon,
            Sector
        }

        [InlineArray(4)]
        private struct Corners
        {
            private Vector2 _element;
        }
    }
}
