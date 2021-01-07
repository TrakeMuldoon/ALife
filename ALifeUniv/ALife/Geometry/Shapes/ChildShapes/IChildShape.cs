namespace ALifeUni.ALife.Shapes
{
    public interface IChildShape
    {
        IShape CloneChildShape(IShape parent);
    }
}
