using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class AgentShadow : WorldObject
    {
        public AgentShadow(Point centrePoint, float startRadius, string genusLabel, string individualLabel, string collisionLevel, Color color) : base(centrePoint, startRadius, genusLabel, individualLabel, collisionLevel, color)
        {
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteAliveTurn()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Reproduce(bool exactCopy)
        {
            throw new NotImplementedException();
        }
    }
}
