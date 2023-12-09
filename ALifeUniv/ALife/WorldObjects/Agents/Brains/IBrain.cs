namespace ALifeUni.ALife.Agents.Brains
{
    public interface IBrain
    {
        void ExecuteTurn();
        IBrain Clone(Agent self);
        IBrain Reproduce(Agent self);
    }
}
