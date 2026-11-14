namespace ALife.Spatial;

/// <summary>Called by <see cref="ISpatialIndex.VisitNodes"/> for each node or cell, e.g. to draw the structure.</summary>
/// <param name="bounds">The node's box. For loose structures this is the box around its elements, which can be <see cref="Aabb.Empty"/>.</param>
/// <param name="depth">The node's depth: 0 for a quadtree's root, or for a grid's coarsest cells.</param>
/// <param name="isLeaf">Whether the node holds elements directly rather than other nodes.</param>
/// <param name="count">For a leaf, how many elements it holds. For a loose/tight grid's tight cell, how many loose cells it lists. Otherwise 0.</param>
public delegate void SpatialNodeVisitor(Aabb bounds, int depth, bool isLeaf, int count);
