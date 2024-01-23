using ALife.Core.Collision;
using ALife.Core.Scenarios.ScenarioHelpers;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Properties;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Prebuilt
{
    public class Rabbit : Agent
    {
        public Rabbit(Zone parentZone) : base("Rabbit", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {
            HomeZone = parentZone;

            int agentRadius = 5;
            ApplyCircleShapeToAgent(parentZone.Distributor, System.Drawing.Color.Red, agentRadius, 0);

            List<SenseCluster> agentSenses = CommonSenses.QuadrantEyes(this, 0);

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Caught", 0, Int32.MaxValue),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(this, ActionCluster.NullInteraction),
                new RotateCluster(this, ActionCluster.NullInteraction)
            };

            this.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(this, "IF EyeLeft.SeeSomething.Value Equals [True] THEN Move.GoRight AT [0.8]",
                                                       "IF EyeRight.SeeSomething.Value Equals [True] THEN Move.GoLeft AT [0.8]",
                                                       "IF EyeStraight.SeeSomething.Value Equals [True] THEN Move.GoBackward AT [0.8]",
                                                       "IF EyeBack.SeeSomething.Value Equals [True] THEN Move.GoForward AT [0.8]",
                                                       "IF ALWAYS THEN Rotate.TurnLeft AT [0.1]"
            );

            this.CompleteInitialization(null, 1, newBrain);
        }

        public override void ScenarioEndOfTurnTriggers()
        {
        }

        public override void Die()
        {
        }

        public void Caught(Agent caughtMe)
        {
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[CollisionLevel];

            //Get a new free Geometry.Shapes.Point within the start zone.
            Geometry.Shapes.Point myPoint = HomeZone.Distributor.NextObjectCentre(Shape.BoundingBox.XLength, Shape.BoundingBox.YHeight);
            Shape.CentrePoint = myPoint;
            collider.MoveObject(this);

            Statistics["Caught"].IncreasePropertyBy(1);
        }
    }
}

//Circle Rabbit Math
//t = some angle
//x = cos(t), y=sin(t)