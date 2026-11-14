namespace ALife.Spatial.Tests;

[TestClass]
public sealed class BuildingBlockTests
{
    [TestMethod]
    public void Aabb_edges_are_inclusive()
    {
        Aabb box = new(0, 0, 10, 10);

        Assert.IsTrue(box.Intersects(new Aabb(10, 10, 20, 20)));
        Assert.IsFalse(box.Intersects(new Aabb(10.5f, 0, 20, 10)));
        Assert.IsTrue(box.Contains(10, 0));
        Assert.IsFalse(box.Contains(-0.1f, 5));
    }

    [TestMethod]
    public void Aabb_empty_is_the_identity_for_union_and_meets_nothing()
    {
        Aabb box = new(1, 2, 3, 4);

        Assert.IsTrue(Aabb.Empty.IsEmpty);
        Assert.AreEqual(box, Aabb.Empty.Union(box));
        Assert.AreEqual(box, box.Union(Aabb.Empty));
        Assert.IsFalse(Aabb.Empty.Intersects(box));
        Assert.IsFalse(box.Intersects(Aabb.Empty));
    }

    [TestMethod]
    public void Aabb_measures_and_builds()
    {
        Aabb box = Aabb.FromCenter(5, 10, 2, 3);

        Assert.AreEqual(new Aabb(3, 7, 7, 13), box);
        Assert.AreEqual(4, box.Width);
        Assert.AreEqual(6, box.Height);
        Assert.AreEqual(5, box.CenterX);
        Assert.AreEqual(10, box.CenterY);
        Assert.AreEqual(new Aabb(2, 5, 8, 15), box.Inflate(1, 2));
    }

    [TestMethod]
    public void FreeList_reuses_erased_slots_last_in_first_out()
    {
        FreeList<long> list = new();
        int a = list.Insert(1), b = list.Insert(2), c = list.Insert(3);

        list.Erase(a);
        list.Erase(c);

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(2, list[b]);
        Assert.AreEqual(c, list.Insert(30));
        Assert.AreEqual(a, list.Insert(10));
        Assert.AreEqual(3, list.Range);
        Assert.AreEqual(10, list[a]);
        Assert.AreEqual(30, list[c]);
    }

    [TestMethod]
    public void FreeList_grows_past_its_first_block()
    {
        FreeList<int> list = new();
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(i, list.Insert(i * 2));
        }

        Assert.AreEqual(100, list.Count);
        Assert.AreEqual(198, list[99]);

        list.Clear();
        Assert.AreEqual(0, list.Count);
        Assert.AreEqual(0, list.Insert(5));
    }
}
