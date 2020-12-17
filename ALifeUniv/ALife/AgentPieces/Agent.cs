﻿using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.TesterBrain;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class Agent : WorldObject
    {
        public IBrain myBrain
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

        public Zone Zone;
        public Zone TargetZone;
        public double StartOrientation
        {
            get;
            set;
        }

        public Agent(String genusLabel)
            : base(genusLabel
                  , AgentIDGenerator.GetNextAgentId()
                  , ReferenceValues.CollisionLevelPhysical)
        {

        }

        public Agent(String genusLabel, string individualLabel, string collisionLevel)
            : base(genusLabel, individualLabel, collisionLevel)
        {
            Parent = null;
            LivingAncestor = null;
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
            Shadow = new AgentShadow(this);
            myBrain.ExecuteTurn();
            if(!Alive)
            {
                return;
            }
            //TODO: Abstract this out.
            //Increment or Decrement end of turn values
            Statistics["Age"].IncreasePropertyBy(1);
            Statistics["DeathTimer"].IncreasePropertyBy(1);
            Statistics["ZoneEscapeTimer"].IncreasePropertyBy(1);

            //Reset all the senses. 
            Senses.ForEach((se) => se.Shape.Reset());
            foreach(StatisticInput stat in Statistics.Values)
            {
                stat.Reset();
            }
            foreach(PropertyInput prop in Properties.Values)
            {
                prop.Reset();
            }
            EndOfTurnTriggers();
        }

        internal void SetStaticInfo(int generation)
        {
            Generation = generation;
            Parent = null;
            LivingAncestor = null;
        }

        internal void AttachAttributes(IShape newShape, List<SenseCluster> senses, List<PropertyInput> properties, List<StatisticInput> statistics, List<ActionCluster> actions)
        {
            Shape = newShape;
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

        public void EndOfTurnTriggers()
        {
            if(Statistics["DeathTimer"].Value > 999)
            {
                Die();
            }
            List<Zone> inZones = Planet.World.ZoneMap.QueryForBoundingBoxCollisions(Shape.BoundingBox);
            foreach(Zone z in inZones)
            {
                if(z.Name == Zone.Name)
                {
                    if(Statistics["ZoneEscapeTimer"].Value > 200)
                    {
                        Die();
                    }
                }
                else if(z.Name == TargetZone.Name)
                {
                    ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[CollisionLevel];

                    Point myPoint = Zone.Distributor.NextAgentCentre(Shape.BoundingBox.XLength, Shape.BoundingBox.YHeight);
                    Shape.CentrePoint = myPoint;
                    collider.MoveObject(this);

                    foreach(Zone zon in Planet.World.Zones.Values)
                    {
                        if(zon.Name != Zone.Name)
                        {
                            Reproduce(zon, zon.OppositeZone, new Angle(zon.OrientationDegrees), zon.OppositeZone.Color);
                        }
                    }

                    //You have a new countdown
                    Statistics["DeathTimer"].Value = 0;
                    Statistics["ZoneEscapeTimer"].Value = 0;
                }
            }
        }


        public void ProduceOffspring()
        {
            //Reproduce();
        }

        public override WorldObject Clone()
        {
            return AgentFactory.CloneAgent(this);
        }

        public override WorldObject Reproduce()
        {
            return AgentFactory.ReproduceFromAgent(this);
        }
    }
}
