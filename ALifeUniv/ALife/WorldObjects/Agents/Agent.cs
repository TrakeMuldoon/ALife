using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility.WorldObjects;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;

namespace ALifeUni.ALife.WorldObjects.Agents
{
    [DebuggerDisplay("AgentX:{Shape.CentrePoint.X}")]
    public class Agent : WorldObject
    {
        public IBrain MyBrain
        {
            get;
            private set;
        }

        public List<SenseCluster> Senses
        {
            get;
            private set;
        }
        public ReadOnlyDictionary<String, ActionCluster> Actions
        {
            get;
            private set;
        }

        public AgentShadow Shadow
        {
            get;
            private set;
        }


        public Agent Parent
        {
            get;
            set;
        }
        public Agent LivingAncestor
        {
            get;
            set;
        }

        public int Generation
        {
            get;
            private set;
        }

        public Zone HomeZone;
        public Zone TargetZone;
        public double StartOrientation
        {
            get;
            set;
        }

        public void SetShape(IShape newShape)
        {
            Shape = newShape;
        }

        public Agent(String genusLabel, string individualLabel, string collisionLevel)
            : base(genusLabel, individualLabel, collisionLevel)
        {
        }

        public Agent(String genusLabel, string individualLabel, string collisionLevel, Zone parentZone, Zone targetZone)
            : base(genusLabel, individualLabel, collisionLevel)
        {
            HomeZone = parentZone;
            TargetZone = targetZone;
        }

        internal void ApplyCircleShapeToAgent(Point centrePoint, Color colour, int circleRadius, double startOrientation)
        {
            IShape myShape = new Circle(centrePoint, circleRadius);
            StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = colour;
            SetShape(myShape);
        }

        internal void ApplyCircleShapeToAgent(WorldObjectDistributor distributor, Color colour, int circleRadius, double startOrientation)
        {
            Point centrePoint = distributor.NextObjectCentre(circleRadius * 2, circleRadius * 2);
            IShape myShape = new Circle(centrePoint, circleRadius);
            StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = colour;
            SetShape(myShape);
        }

        internal void CompleteInitialization(Agent parent, int generation, IBrain newBrain)
        {
            Generation = generation;
            MyBrain = newBrain;

            Parent = parent;
            LivingAncestor = parent;
            Shadow = new AgentShadow(this);
            //Release them out into the world
            Planet.World.AddObjectToWorld(this);
        }

        internal void AttachAttributes(List<SenseCluster> senses, List<PropertyInput> properties, List<StatisticInput> statistics, List<ActionCluster> actions)
        {
            Senses = senses;
            properties.ForEach((p) => Properties.Add(p.Name, p));
            statistics.ForEach((s) => Statistics.Add(s.Name, s));
            Actions = CreateRODForActions(actions);
        }

        private ReadOnlyDictionary<string, ActionCluster> CreateRODForActions(List<ActionCluster> actionList)
        {
            Dictionary<string, ActionCluster> myActions = new Dictionary<string, ActionCluster>();
            actionList.ForEach((ac) => myActions.Add(ac.Name, ac));

            return new ReadOnlyDictionary<string, ActionCluster>(myActions);
        }


        public override void Die()
        {
            Alive = false;
            Shape.DebugColor = Colors.Maroon;
            Planet.World.ChangeCollisionLayerForObject(this, ReferenceValues.CollisionLevelDead);
            CollisionLevel = ReferenceValues.CollisionLevelDead;
        }

        public override void ExecuteDeadTurn()
        {
            //TODO: Abstract this out
            Planet.World.RemoveWorldObject(this);
        }

        public override void ExecuteAliveTurn()
        {
            //Save the previous state of agent, so we can look back on it next turn.
            Shadow = new AgentShadow(this);

            MyBrain.ExecuteTurn();
            if(!Alive)
            {
                return;
            }

            AgentUpkeep();

            //Reset all the senses.
            Senses.ForEach((se) => se.Shape.Reset());

            //Reset all the properties
            foreach(StatisticInput stat in Statistics.Values)
            {
                stat.Reset();
            }
            foreach(PropertyInput prop in Properties.Values)
            {
                prop.Reset();
            }

            EndOfTurnTriggers();

            //The shape has moved, so its bounding box needs to be reset
            Shape.Reset();
        }

        public StatisticInput CreateIncrementingStatistic(string name, int statisticMinimum, int statisticMaximum, [Optional] int startValue)
        {
            StatisticInput si = new StatisticInput(name, statisticMinimum, statisticMaximum, startValue);
            IncrementingStatistics.Add(name);
            return si;
        }

        public StatisticInput CreateDecrementingStatistic(string name, int statisticMinimum, int statisticMaximum, [Optional] int startValue)
        {
            StatisticInput si = new StatisticInput(name, statisticMinimum, statisticMaximum, startValue);
            DecrementingStatistics.Add(name);
            return si;
        }

        List<string> IncrementingStatistics = new List<string>();
        List<string> DecrementingStatistics = new List<string>();

        public void AgentUpkeep()
        {
            foreach(string upStat in IncrementingStatistics)
            {
                Statistics[upStat].IncreasePropertyBy(1);
            }
            foreach(string downStat in DecrementingStatistics)
            {
                Statistics[downStat].DecreasePropertyBy(1);
            }
        }


        public virtual void EndOfTurnTriggers()
        {
            Planet.World.Scenario.AgentEndOfTurnTriggers(this);
        }

        public void CollisionBehvaviour(List<WorldObject> collisions)
        {
            Planet.World.Scenario.CollisionBehaviour(this, collisions);
        }

        public override WorldObject Clone()
        {
            NumChildren++;
            return AgentFactory.CloneAgent(this);
        }

        public override WorldObject Reproduce()
        {
            NumChildren++;
            return AgentFactory.ReproduceFromAgent(this);
        }
    }
}
