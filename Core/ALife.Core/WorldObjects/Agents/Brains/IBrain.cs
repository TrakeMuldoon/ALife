namespace ALife.Core.WorldObjects.Agents.Brains
{
    public interface IBrain
    {
        void ExecuteTurn();
        IBrain Clone(Agent self);
        IBrain Reproduce(Agent self);

        string ExportNewBrain();

    }
}
