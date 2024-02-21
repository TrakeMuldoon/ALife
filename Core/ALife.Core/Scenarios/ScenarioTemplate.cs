using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents;
using System;

namespace ALife.Core.Scenarios
{
    /*
        [ScenarioRegistration("[ScenarioName]",
    description:
        @"
    [Scenario description]

    Failure cases:
    [What kills an agent]

    Success Cases:
    [What causes an agent to reproduce]
    ")]
    */
    //[SuggestedSeed([SeedNumber], "Seed Description")]
    public class ScenarioTemplate : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        //TODO: Fully Comment This
        public virtual Agent CreateAgentOne(string genusName, Zone parentZone, Zone targetZone, Colour colour, double startOrientation)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Agent End Of Turn Triggers is a function which will be executed at the end of every Scenario Agent's turn.
        /// Any manual statistic updating, state checking, win conditions or other activities should be done here.
        /// </summary>
        /// <param name="me"></param>
        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            //Default, nothing happens
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        /// <summary>
        /// The width of the Simulation space.
        /// </summary>
        public virtual int WorldWidth => throw new NotImplementedException();

        /// <summary>
        /// The height of the simulation space.
        /// </summary>
        public virtual int WorldHeight => throw new NotImplementedException();

        /// <summary>
        /// This property is to indicate if the Scenario makes sense at arbitrary widths and heights.
        /// For instance, most of the maze scenarios are fixed width/height, because the mazes are fixed position.
        /// The Garden scenarios tend to not be fixed width/height, as the size of the garden does not impact the scenario.
        /// </summary>
        public virtual bool FixedWidthHeight
        {
            get { return false; }
        }

        //TODO: Fully Comment This
        public virtual void PlanetSetup()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function is executed after:
        /// 1. Every Active WorldObject has taken a turn
        /// 2. all the new WorldObjects have been added to the Active list.
        /// 3. all the discarded WorldObjects have been removed from the ActiveList.
        /// This function is executed, and then a new WorldTurn begins.
        /// </summary>
        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
