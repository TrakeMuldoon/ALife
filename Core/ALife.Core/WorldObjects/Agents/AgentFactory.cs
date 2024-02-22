using ALife.Core.Distributors;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents
{
    public static class AgentFactory
    {
        private const int DEFAULT_AGENT_RADIUS = 5; //TODO: Migrate generic config values to config class.

        public static Agent ConstructCircularAgent(string genusName, Zone parentZone, Zone targetZone, Colour colour, Colour? debugColour, double startOrientation)
        {
            Agent newAgent = new Agent(genusName
                                        , AgentIDGenerator.GetNextAgentId()
                                        , ReferenceValues.CollisionLevelPhysical
                                        , parentZone
                                        , targetZone);

            ApplyCircleShapeToAgent(newAgent
                                    , parentZone.Distributor
                                    , colour, debugColour
                                    , DEFAULT_AGENT_RADIUS
                                    , startOrientation);
            return newAgent;
        }

        //TODO: This should be private. But I'm in the middle of migrating.
        public static void ApplyCircleShapeToAgent(Agent agent, WorldObjectDistributor distributor, Colour colour, Colour? debugColour, int circleRadius, double startOrientation)
        {
            Point centrePoint = distributor.NextObjectCentre(circleRadius * 2, circleRadius * 2);
            IShape myShape = new Circle(centrePoint, circleRadius);
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Colour = colour;
            if(debugColour.HasValue)
            {
                myShape.DebugColour = debugColour.Value;
            }
            agent.SetShape(myShape);
        }

        //TODO: Another area where code is duplicated.
        public static Agent CloneAgent(Agent toClone)
        {
            Agent newClone = new Agent(toClone.GenusLabel
                                       , AgentIDGenerator.GetNextChildId(toClone.IndividualLabel, toClone.NumChildren)
                                       , ReferenceValues.CollisionLevelPhysical); //TODO: This is a bug when you use "ReproduceBest" or reproduce anything that is already dead. 
            newClone.HomeZone = toClone.HomeZone;
            newClone.TargetZone = toClone.TargetZone;

            IShape clonedShape = toClone.Shape.CloneShape();
            newClone.StartOrientation = toClone.StartOrientation;
            clonedShape.Orientation.Degrees = toClone.StartOrientation;
            newClone.SetShape(clonedShape);

            Point newCentrePoint = toClone.HomeZone.Distributor.NextObjectCentre(clonedShape.BoundingBox.XLength, clonedShape.BoundingBox.YHeight);
            clonedShape.CentrePoint = newCentrePoint;

            List<SenseCluster> clonedSenses = new List<SenseCluster>();
            toClone.Senses.ForEach((sc) => clonedSenses.Add(sc.CloneSense(newClone)));

            List<PropertyInput> clonedProperties = new List<PropertyInput>();
            foreach(PropertyInput pi in toClone.Properties.Values) clonedProperties.Add(pi.ClonePropertyInput());

            List<StatisticInput> clonedStatistics = new List<StatisticInput>();
            foreach(StatisticInput si in toClone.Statistics.Values) clonedStatistics.Add(si.CloneStatisticInput());

            List<ActionCluster> clonedActions = new List<ActionCluster>();
            foreach(ActionCluster ac in toClone.Actions.Values) clonedActions.Add(ac.CloneAction(newClone));

            newClone.AttachAttributes(clonedSenses, clonedProperties, clonedStatistics, clonedActions);


            IBrain newBrain = toClone.MyBrain.Clone(newClone);

            newClone.CompleteInitialization(toClone, toClone.Generation + 1, newBrain);
            newClone.CustomEndOfTurnTriggers = toClone.CustomEndOfTurnTriggers;

            return newClone;
        }

        public static Agent ReproduceFromAgent(Agent newParent)
        {
            Agent newChild = new Agent(newParent.GenusLabel
                           , AgentIDGenerator.GetNextChildId(newParent.IndividualLabel, newParent.NumChildren)
                           , ReferenceValues.CollisionLevelPhysical); //TODO: This is hardcoded to solve a bug where "ReproduceBest" or any kind of out of band reproduction
                                                                      //was being used. It was reproducing on the dead layer. 
                                                                      //This should be changed to somehow understand the target layer it wishes to reproduce on. 

            newChild.HomeZone = newParent.HomeZone;
            newChild.TargetZone = newParent.TargetZone;

            IShape evolvedShape = newParent.Shape.CloneShape();
            newChild.StartOrientation = newParent.StartOrientation;
            evolvedShape.Orientation.Degrees = newParent.StartOrientation;
            newChild.SetShape(evolvedShape);

            Point newCentrePoint = newParent.HomeZone.Distributor.NextObjectCentre(evolvedShape.BoundingBox.XLength, evolvedShape.BoundingBox.YHeight);
            evolvedShape.CentrePoint = newCentrePoint;

            List<SenseCluster> evolvedSenses = new List<SenseCluster>();
            newParent.Senses.ForEach((sc) => evolvedSenses.Add(sc.ReproduceSense(newChild)));

            List<PropertyInput> evolvedProperties = new List<PropertyInput>();
            foreach(PropertyInput pi in newParent.Properties.Values) evolvedProperties.Add(pi.ClonePropertyInput()); //TODO: EVOLVE THIS

            List<StatisticInput> evolvedStatistics = new List<StatisticInput>();
            foreach(StatisticInput si in newParent.Statistics.Values) evolvedStatistics.Add(si.CloneStatisticInput()); //TODO: EVOLVE THIS

            List<ActionCluster> evolvedActions = new List<ActionCluster>();
            foreach(ActionCluster ac in newParent.Actions.Values) evolvedActions.Add(ac.CloneAction(newChild)); //TODO: EVOLVE THIS

            newChild.AttachAttributes(evolvedSenses, evolvedProperties, evolvedStatistics, evolvedActions);

            IBrain newBrain = newParent.MyBrain.Reproduce(newChild);

            newChild.CompleteInitialization(newParent, newParent.Generation + 1, newBrain);
            newChild.CustomEndOfTurnTriggers = newParent.CustomEndOfTurnTriggers;

            return newChild;
        }
    }
}
