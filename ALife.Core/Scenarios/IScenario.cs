using System;
using System.Collections.Generic;
using Windows.UI;
using ALife.Core.Scenarios;
using ALife.Core.WorldObjects.Agents;

namespace ALife.Core.Scenarios
{
    public interface IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color colour, double startOrientation);

        /*** Something that is happening TO an agent, based on external stimulus ***/
        void AgentEndOfTurnTriggers(Agent me);

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
    }

    public static class IScenarioHelpers
    {
        /* This is called when the scenario is reset, to get you a fresh scenario */
        public static IScenario FreshInstanceOf(IScenario originalScenario)
        {
            return (IScenario)Activator.CreateInstance(originalScenario.GetType());
        }
    }
}
