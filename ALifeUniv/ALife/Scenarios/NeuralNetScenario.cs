using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public class NeuralNetScenario : BaseScenario
    {
        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.Zone = parentZone;
            agent.TargetZone = targetZone;

            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10); 

            IShape myShape = new Circle(centrePoint, 5);                        
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "Eye1"
                                , new ROEvoNumber(0, 20, -360, 360)  //Orientation Around Parent
                                , new ROEvoNumber(0, 30, -360, 360)  //Relative Orientation
                                , new ROEvoNumber(80, 3, 40, 120)    //Radius
                                , new ROEvoNumber(25, 1, 15, 40)),   //Sweep
                new ProximityCluster(agent, "Proximity1"
                                , new ROEvoNumber(20, 4, 10, 40))    //Radius
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>();

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new ColorCluster(agent),
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new NeuralNetworkBrain(agent, new List<int> { 8, 8 });

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public override void AgentUpkeep(Agent me)
        {
        }

        public override void EndOfTurnTriggers(Agent me)
        {
        }

        public override void GlobalEndOfTurnActions()
        {
        }

        public override string Name
        {
            get { return "Neural Net Test"; }
        }
        public override void PlanetSetup()
        {
            Zone nullZone = new Zone("Null", "random", Colors.Black, new Point(0, 0), 1000, 1000);
            Planet.World.AddZone(nullZone);

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", nullZone, null, Colors.Blue, 0);
            }
        }

        public override void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {

        }
    }
}
