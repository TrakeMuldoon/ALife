using ALife.Core.Utility.Collections;

namespace ALife.Core.CollisionDetection.CollisionGrids.LooseGrids
{
    public struct GridQuery4
    {
        public SmallList<int> Elements;

        public LooseGridQuery4(int capacity = 4)
        {
            Elements = new SmallList<int>(capacity);
        }
    }
}
