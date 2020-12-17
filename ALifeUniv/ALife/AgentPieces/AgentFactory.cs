using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.AgentPieces
{
    public static class AgentFactory
    {
        public static Agent CreateAgent()
        {
            
        }

        public static Agent CloneAgent(Agent toClone)
        {
            Agent newClone = new Agent(toClone.GenusLabel
                                       , AgentIDGenerator.GetNextChildId(toClone.IndividualLabel, toClone.NumChildren)
                                       , toClone.CollisionLevel);

            //TODO: Get new CENTREPOINTsomehow
            Point newCentrePoint;


            newClone.StartOrientation = toClone.StartOrientation;
            IShape clonedShape = toClone.Shape.CloneShape();
            clonedShape.CentrePoint = newCentrePoint;
            clonedShape.Orientation.Degrees = toClone.StartOrientation;

            List<SenseCluster> clonedSenses = new List<SenseCluster>();
            toClone.Senses.ForEach((sc) => clonedSenses.Add(sc.CloneSense(newClone)));

            List<PropertyInput> clonedProperties = new List<PropertyInput>();
            foreach(PropertyInput pi in toClone.Properties.Values) clonedProperties.Add(pi.ClonePropertyInput());

            List<StatisticInput> clonedStatistics = new List<StatisticInput>();
            foreach(StatisticInput si in toClone.Statistics.Values) clonedStatistics.Add(si.CloneStatisticInput());

            List<ActionCluster> clonedActions = new List<ActionCluster>();
            foreach(ActionCluster ac in toClone.Actions.Values) clonedActions.Add(ac.CloneAction(newClone));

            CompleteAgentInitialization(newClone, toClone.Generation + 1
                                        , clonedShape, clonedSenses, clonedProperties, clonedStatistics, clonedActions); ;

            return newClone;
        }

        public static Agent ReproduceFromAgent(Agent newParent)
        {
            //Agent newChild = new Agent(newParent.GenusLabel
            //               , AgentIDGenerator.GetNextChildId(newParent.IndividualLabel, newParent.NumChildren)
            //               , newParent.CollisionLevel);

            
            ////TODO: Get new CENTREPOINTsomehow
            //Point newCentrePoint;

            //newClone.StartOrientation = toClone.StartOrientation;
            //IShape clonedShape = toClone.Shape.CloneShape();
            //clonedShape.CentrePoint = newCentrePoint;
            //clonedShape.Orientation.Degrees = toClone.StartOrientation;

            //List<SenseCluster> clonedSenses = new List<SenseCluster>();
            //toClone.Senses.ForEach((sc) => clonedSenses.Add(sc.CloneSense(newClone)));

            //List<PropertyInput> clonedProperties = new List<PropertyInput>();
            //foreach(PropertyInput pi in toClone.Properties.Values) clonedProperties.Add(pi.ClonePropertyInput());

            //List<StatisticInput> clonedStatistics = new List<StatisticInput>();
            //foreach(StatisticInput si in toClone.Statistics.Values) clonedStatistics.Add(si.CloneStatisticInput());

            //List<ActionCluster> clonedActions = new List<ActionCluster>();
            //foreach(ActionCluster ac in toClone.Actions.Values) clonedActions.Add(ac.CloneAction(newClone));

            //CompleteAgentInitialization(newClone, toClone.Generation + 1
            //                            , clonedShape, clonedSenses, clonedProperties, clonedStatistics, clonedActions); ;

            //return newClone;
        }

        private static void CompleteAgentInitialization(Agent newAgent
                                                        , int generation
                                                        , IShape newShape
                                                        , List<SenseCluster> senses
                                                        , List<PropertyInput> properties
                                                        , List<StatisticInput> statistics
                                                        , List<ActionCluster> actions
                                                        )
        {
            newAgent.AttachAttributes(newShape
                                      , senses
                                      , properties
                                      , statistics
                                      , actions);
            newAgent.SetStaticInfo(generation);

        }
    }
}
