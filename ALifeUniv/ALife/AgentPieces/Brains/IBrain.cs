namespace ALifeUni.ALife.AgentPieces.Brains
{
    public interface IBrain
    {
        void ExecuteTurn();
        IBrain Clone(Agent self);
        IBrain Reproduce(Agent self);
    }
}
