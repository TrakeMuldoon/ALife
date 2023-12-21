using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Brains;

namespace ALife.Core.WorldObjects.Agents.Brains
{
    public interface IBrain
    {
        void ExecuteTurn();
        IBrain Clone(Agent self);
        IBrain Reproduce(Agent self);
    }
}
