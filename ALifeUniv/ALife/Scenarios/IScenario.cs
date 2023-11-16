using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public interface IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        string Name
        {
            get;
        }

        /******************/
        /*   AGENT STUFF  */
        /******************/

        Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation);


        /*** Something that an agent does, or soemthing affecting their internals ***/
        void AgentUpkeep(Agent me);

        /*** Something that is happening TO an agent, based on external stimulus ***/
        void EndOfTurnTriggers(Agent me);

        void CollisionBehaviour(Agent me, List<WorldObject> collisions);

        /******************/
        /*  PLANET STUFF  */
        /******************/

        int WorldWidth
        {
            get;
        }
        int WorldHeight
        {
            get;
        }
        /* Some Scenarios have fixed maps, like the mazes, you cannot set an arbitrary world width and height for them */
        bool FixedWidthHeight
        {
            get;
        }

        void PlanetSetup();

        void GlobalEndOfTurnActions();


        /* This is called when the scenario is reset, any variables being tracked inside the Scenario itself must be reset */
        void Reset();
    }
}
