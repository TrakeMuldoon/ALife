using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ALife.Core.Scenarios
{
    public interface IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation);

        /*** Something that is happening TO an agent, based on external stimulus ***/
        void AgentEndOfTurnTriggers(Agent me);

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
