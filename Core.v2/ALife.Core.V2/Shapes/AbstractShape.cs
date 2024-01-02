using System.Collections.Generic;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    public abstract class AbstractShape
    {
        public Point CentrePoint;

        public List<AbstractShape> Children = null;

        public Angle Orientation;

        public AbstractShape Parent = null;

        public RenderComponentInfo[] RenderComponents;

        public abstract AbstractShape Clone();

        public abstract BoundingBox GetBoundingBox(BoundingBoxMode mode = BoundingBoxMode.Relative);

        public abstract void Reset();

        public void UpdateAfterParentChange()
        {
            UpdateSelfAfterParentChange();

            for(int i = 0; i < Children.Length; i++)
            {
                Children[i].UpdateAfterParentChange();
            }
        }

        protected abstract void UpdateSelfAfterParentChange();
    }
}
