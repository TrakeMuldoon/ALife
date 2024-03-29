﻿using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ALife.Core.WorldObjects.Agents
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

        /// <summary>
        /// The Parent of the agent.
        /// </summary>
        public Agent Parent
        {
            get;
            set;
        }

        /// <summary>
        /// The Nearest Living Ancestor of the parent.
        /// It is updated externally. 
        /// If the "LivingAncestor" is dead, 
        ///     1. Replace this value with the LivingAncestor of that agent
        ///     2. If LivingAncestor is still Dead, repeat step 1, until it is either NULL (the beginning of the scenario) or a living Agent.
        /// </summary>
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
        public bool JustReproduced
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

        internal void CompleteInitialization(Agent parent, int generation, IBrain newBrain, bool AddToWorld = true)
        {
            Generation = generation;
            MyBrain = newBrain;

            Parent = parent;
            LivingAncestor = parent;
            Shadow = new AgentShadow(this);
            if(AddToWorld)
            {
                //Release them out into the world
                Planet.World.AddObjectToWorld(this);
            }
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
            Shape.DebugColour = Colour.Maroon;
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
            if(Planet.World.GenerateShadow)
            {
                Shadow = new AgentShadow(this);
            }
            else
            {
                Shadow = null;
            }
            JustReproduced = false;

            MyBrain.ExecuteTurn();
            if(!Alive)
            {
                return;
            }

            InternalAgentUpkeep();

            ScenarioEndOfTurnTriggers();

            //The shape has moved, so its bounding box needs to be reset
            Shape.Reset();
        }

        public void InternalAgentUpkeep()
        {
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

            foreach(StatisticInput si in Statistics.Values)
            {
                switch(si.Disposition)
                {
                    case StatisticInputType.Incrementing: si.IncreasePropertyBy(1); break;
                    case StatisticInputType.Decrementing: si.DecreasePropertyBy(1); break;
                    default: break;
                }
            }
        }

        public Action<Agent> CustomEndOfTurnTriggers;

        public virtual void ScenarioEndOfTurnTriggers()
        {
            Planet.World.Scenario.AgentEndOfTurnTriggers(this);
        }

        public override WorldObject Clone()
        {
            JustReproduced = true;
            NumChildren++;
            return AgentFactory.CloneAgent(this);
        }

        public override WorldObject Reproduce()
        {
            JustReproduced = true;
            NumChildren++;
            return AgentFactory.ReproduceFromAgent(this);
        }
    }
}
