using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.AgentPieces
{
    public static class AgentFactory
    {
        public static Agent CreateAgent(String genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.Zone = parentZone;
            agent.TargetZone = targetZone;

            //TODO: FIX SO THAT THE SHAPE IS A PARAMETER
            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10); //TODO: HARDCODED AGENT RADIUS

            IShape myShape = new Circle(centrePoint, 5);                        //TODO: HARDCODED AGENT RADIUS
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(agent, "Eye1"),
                new ProximityCluster(agent, "Proximity1")
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue),
                new StatisticInput("ZoneEscapeTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                //new ColorCluster(agent),
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            CompleteAgentInitialization(agent, agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(agent, "IF Age.Value GreaterThan [10] THEN Move.GoForward AT [0.2]", "*", "*", "*", "*");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public static Agent CloneAgent(Agent toClone)
        {
            Agent newClone = new Agent(toClone.GenusLabel
                                       , AgentIDGenerator.GetNextChildId(toClone.IndividualLabel, toClone.NumChildren)
                                       , toClone.CollisionLevel);
            newClone.Zone = toClone.Zone;
            newClone.TargetZone = toClone.TargetZone;

            IShape clonedShape = toClone.Shape.CloneShape();
            newClone.StartOrientation = toClone.StartOrientation;
            clonedShape.Orientation.Degrees = toClone.StartOrientation;
            newClone.SetShape(clonedShape);

            Point newCentrePoint = toClone.Zone.Distributor.NextAgentCentre(clonedShape.BoundingBox.XLength, clonedShape.BoundingBox.YHeight);
            clonedShape.CentrePoint = newCentrePoint;

            List<SenseCluster> clonedSenses = new List<SenseCluster>();
            toClone.Senses.ForEach((sc) => clonedSenses.Add(sc.CloneSense(newClone)));

            List<PropertyInput> clonedProperties = new List<PropertyInput>();
            foreach(PropertyInput pi in toClone.Properties.Values) clonedProperties.Add(pi.ClonePropertyInput());

            List<StatisticInput> clonedStatistics = new List<StatisticInput>();
            foreach(StatisticInput si in toClone.Statistics.Values) clonedStatistics.Add(si.CloneStatisticInput());

            List<ActionCluster> clonedActions = new List<ActionCluster>();
            foreach(ActionCluster ac in toClone.Actions.Values) clonedActions.Add(ac.CloneAction(newClone));

            CompleteAgentInitialization(newClone, clonedSenses, clonedProperties, clonedStatistics, clonedActions);

            IBrain newBrain = toClone.myBrain.Clone(newClone);

            newClone.CompleteInitialization(toClone, toClone.Generation + 1, newBrain);

            return newClone;
        }

        public static Agent ReproduceFromAgent(Agent newParent)
        {
            Agent newChild = new Agent(newParent.GenusLabel
                           , AgentIDGenerator.GetNextChildId(newParent.IndividualLabel, newParent.NumChildren)
                           , newParent.CollisionLevel);

            newChild.Zone = newParent.Zone;
            newChild.TargetZone = newParent.TargetZone;

            IShape evolvedShape = newParent.Shape.CloneShape();
            newChild.StartOrientation = newParent.StartOrientation;
            evolvedShape.Orientation.Degrees = newParent.StartOrientation;
            newChild.SetShape(evolvedShape);

            Point newCentrePoint = newParent.Zone.Distributor.NextAgentCentre(evolvedShape.BoundingBox.XLength, evolvedShape.BoundingBox.YHeight);
            evolvedShape.CentrePoint = newCentrePoint;

            List<SenseCluster> evolvedSenses = new List<SenseCluster>();
            newParent.Senses.ForEach((sc) => evolvedSenses.Add(sc.CloneSense(newChild))); //TODO: EVOLVE THIS

            List<PropertyInput> evolvedProperties = new List<PropertyInput>();
            foreach(PropertyInput pi in newParent.Properties.Values) evolvedProperties.Add(pi.ClonePropertyInput()); //TODO: EVOLVE THIS

            List<StatisticInput> evolvedStatistics = new List<StatisticInput>();
            foreach(StatisticInput si in newParent.Statistics.Values) evolvedStatistics.Add(si.CloneStatisticInput()); //TODO: EVOLVE THIS

            List<ActionCluster> evolvedActions = new List<ActionCluster>();
            foreach(ActionCluster ac in newParent.Actions.Values) evolvedActions.Add(ac.CloneAction(newChild)); //TODO: EVOLVE THIS

            CompleteAgentInitialization(newChild, evolvedSenses, evolvedProperties, evolvedStatistics, evolvedActions);

            IBrain newBrain = newParent.myBrain.Reproduce(newChild);

            newChild.CompleteInitialization(newParent, newParent.Generation + 1, newBrain);

            return newChild;
        }

        private static void CompleteAgentInitialization(Agent newAgent
                                                        , List<SenseCluster> senses
                                                        , List<PropertyInput> properties
                                                        , List<StatisticInput> statistics
                                                        , List<ActionCluster> actions
                                                        )
        {
            newAgent.AttachAttributes(senses
                                      , properties
                                      , statistics
                                      , actions);
        }
    }
}
