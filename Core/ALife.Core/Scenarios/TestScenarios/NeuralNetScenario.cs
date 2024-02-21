using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.EvoNumbers;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System.Collections.Generic;

namespace ALife.Core.Scenarios.TestScenarios
{
    [ScenarioRegistration("Test - Neural Net", description: "Lorum Ipsum", debugModeOnly: true)]
    public class NeuralNetScenario : IScenario
    {
        /******************/
        /*   AGENT STUFF  */
        /******************/

        public virtual Agent CreateAgentOne(string genusName, Zone parentZone, Zone targetZone, Colour color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.HomeZone = parentZone;
            agent.TargetZone = targetZone;

            Point centrePoint = parentZone.Distributor.NextObjectCentre(10, 10);

            IShape myShape = new Circle(centrePoint, 5);
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Colour = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "Eye1"
                                , new ReadOnlyEvoNumber(0, 20, -360, 360)  //Orientation Around Parent
                                , new ReadOnlyEvoNumber(0, 30, -360, 360)  //Relative Orientation
                                , new ReadOnlyEvoNumber(80, 3, 40, 120)    //Radius
                                , new ReadOnlyEvoNumber(25, 1, 15, 40)),   //Sweep
                new ProximityCluster(agent, "Proximity1"
                                , new ReadOnlyEvoNumber(20, 4, 10, 40))    //Radius
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>();

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new ColorCluster(agent),
                new MoveCluster(agent, ActionCluster.NullInteraction),
                new RotateCluster(agent, ActionCluster.NullInteraction)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 7, 9 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public virtual void AgentEndOfTurnTriggers(Agent me)
        {
            //Do Nothing
        }

        /******************/
        /*  PLANET STUFF  */
        /******************/

        public virtual int WorldWidth { get { return 800; } }

        public virtual int WorldHeight { get { return 800; } }

        public bool FixedWidthHeight => true;

        public virtual void PlanetSetup()
        {
            Zone nullZone = new Zone("Null", "random", Colour.Black, new Point(0, 0), 1000, 1000);
            Planet.World.AddZone(nullZone);

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = CreateAgentOne("Agent", nullZone, null, Colour.Blue, 0);
            }
        }

        public void GlobalEndOfTurnActions()
        {
            //Do Nothing
        }
    }
}
