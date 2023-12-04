using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class ManaScenario : IScenario
    {
        /******************/
        /* SCENARIO STUFF */
        /******************/

        public virtual string Name => "Mana from the sky";

        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color colour, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical
            , parentZone
            , targetZone);

            int agentRadius = 5;
            agent.ApplyCircleShapeToAgent(parentZone.Distributor, colour, agentRadius, startOrientation);

            List<SenseCluster> agentSenses = ListExtensions.CompileList<SenseCluster>(null,
                new EyeCluster(agent, "EyeLeft", true
                    , new ROEvoNumber(startValue: -30, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: 10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)                         //Relative Orientation
                    , new ROEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),                          //Sweep
                new EyeCluster(agent, "EyeRight", true
                    , new ROEvoNumber(startValue: 30, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                    , new ROEvoNumber(startValue: -10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)                        //Relative Orientation
                    , new ROEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15, hardMax: 40))                           //Sweep
            );

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new BehaviourBrain(agent, "IF Age.Value GreaterThan [10] THEN Move.GoForward AT [0.2]", "*", "*", "*", "*");
            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 15, 12 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }
        public virtual void AgentUpkeep(Agent me)
        {
            //TODO: Fully Comment This
            //Default, no upkeep
        }

        public virtual void EndOfTurnTriggers(Agent me)
        {
            //TODO: Fully Comment This
            //Default, nothing happens
        }

        public virtual void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //TODO: Fully Comment This
            //Default, nothing
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        //TODO: Fully Comment This
        public virtual int WorldWidth => throw new NotImplementedException();

        //TODO: Fully Comment This
        public virtual int WorldHeight => throw new NotImplementedException();

        //TODO: Fully Comment This
        public virtual bool FixedWidthHeight
        {
            get { return false; }
        }

        //TODO: Fully Comment This
        public virtual void PlanetSetup()
        {
            throw new NotImplementedException();
        }

        //TODO: Fully Comment This
        public virtual void GlobalEndOfTurnActions()
        {
            //Default, no special actions
        }
    }
}
