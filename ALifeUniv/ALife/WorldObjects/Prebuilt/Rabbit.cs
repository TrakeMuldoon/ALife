using ALifeUni.ALife.Collision;
using ALifeUni.ALife.Scenarios.ScenarioHelpers;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Brains.BehaviourBrains;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.WorldObjects.Agents.CustomAgents
{
    public class Rabbit : Agent
    {
        public Rabbit(Zone parentZone) : base("MazeRunner", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {
            HomeZone = parentZone;

            int agentRadius = 5;
            ApplyCircleShapeToAgent(parentZone.Distributor, Colors.Red, agentRadius, 0);

            List<SenseCluster> agentSenses = CommonSenses.QuadrantEyes(this, 0);

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Caught", 0, Int32.MaxValue),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(this),
                new RotateCluster(this)
            };

            this.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(this, "IF EyeLeft.SeeSomething.Value Equals [True] THEN Move.GoRight AT [0.8]",
                                                       "IF EyeRight.SeeSomething.Value Equals [True] THEN Move.GoLeft AT [0.8]",
                                                       "IF EyeStraight.SeeSomething.Value Equals [True] THEN Move.GoBackward AT [0.8]",
                                                       "IF EyeBack.SeeSomething.Value Equals [True] THEN Move.GoForward AT [0.8]",
                                                       "IF ALWAYS THEN Rotate.TurnLeft AT [0.1]"
                );
            //new EyeCluster(agent, "EyeStraight"
            //    new EyeCluster(agent, "EyeRight"
            //    new EyeCluster(agent, "EyeBack"
            //    new EyeCluster(agent, "EyeLeft"

            this.CompleteInitialization(null, 1, newBrain);
        }

        public override void ScenarioEndOfTurnTriggers()
        {
        }

        public override void Die()
        {
        }

        public void CollisionBehaviour(Agent me, List<WorldObject> collisions)
        {
            //So the rabbit doesn't kill people.
        }

        public void Caught(Agent caughtMe)
        {
            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[CollisionLevel];

            //Get a new free point within the start zone.
            Point myPoint = HomeZone.Distributor.NextObjectCentre(Shape.BoundingBox.XLength, Shape.BoundingBox.YHeight);
            Shape.CentrePoint = myPoint;
            collider.MoveObject(this);

            Statistics["Caught"].IncreasePropertyBy(1);
        }
    }
}

//Circle Rabbit Math
//t = some angle
//x = cos(t), y=sin(t)