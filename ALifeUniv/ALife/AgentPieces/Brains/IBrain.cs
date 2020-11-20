using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains
{
    public interface IBrain
    {
        void ExecuteTurn();
        IBrain Clone(Agent self);
        IBrain Reproduce(Agent self);
    }
}
