using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class AbstractScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public virtual string Name => throw new NotImplementedException();

        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            throw new NotImplementedException();
        }
        public virtual void AgentUpkeep(Agent me)
        {
            //Default, no upkeep
        }

        public virtual void EndOfTurnTriggers(Agent me)
        {
            //Default, nothing happens
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //Default, nothing
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth => throw new NotImplementedException();

        public virtual int WorldHeight => throw new NotImplementedException();

        public virtual bool FixedWidthHeight
        {
            get { return false; }
        }

        public virtual void PlanetSetup()
        {
            throw new NotImplementedException();
        }

        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }

        public virtual void Reset()
        {
            //Default, no special actions
        }
    }
}
