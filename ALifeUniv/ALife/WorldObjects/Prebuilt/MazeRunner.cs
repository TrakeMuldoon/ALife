using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using ALifeUni.ALife.WorldObjects.Agents.Brains;
using ALifeUni.ALife.WorldObjects.Agents.Brains.BehaviourBrains;
using ALifeUni.ALife.WorldObjects.Agents.Properties;
using ALifeUni.ALife.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife.WorldObjects.Agents.CustomAgents
{
    public class MazeRunner : Agent
    {
        public MazeRunner(Zone parentZone, Zone targetZone) : base("MazeRunner", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {
            HomeZone = parentZone;
            TargetZone = targetZone;

            int agentRadius = 5;
            ApplyCircleShapeToAgent(parentZone.Distributor, Colors.Red, agentRadius, 0);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(this, "EyeLeft"
                                , new ROEvoNumber(startValue: -15, evoDeltaMax: 20, hardMin:-360, hardMax: 360)  //Orientation Around Parent
                                , new ROEvoNumber(startValue: 10, evoDeltaMax:30, hardMin:-360, hardMax: 360)    //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax:3, hardMin:40, hardMax:90)         //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax:1, hardMin:15, hardMax:40)),       //Sweep
                new EyeCluster(this, "EyeRight"
                                , new ROEvoNumber(startValue: 15, evoDeltaMax: 20, hardMin:-360, hardMax: 360)   //Orientation Around Parent
                                , new ROEvoNumber(startValue: -10, evoDeltaMax:30, hardMin:-360, hardMax: 360)   //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax:3, hardMin:40, hardMax:90)         //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax:1, hardMin:15, hardMax:40)),       //Sweep
                new GoalSenseCluster(this, "Goals", TargetZone)
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("MaximumX", 0, Int32.MaxValue),
                new StatisticInput("MaxXTimer", 0, Int32.MaxValue)
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(this),
                new RotateCluster(this)
            };

            this.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(this, "IF EyeLeft.SeeSomething.Value Equals [True] THEN Rotate.TurnRight AT [0.045]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [True] THEN Rotate.TurnLeft AT [0.035]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [False] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [False] AND " +
                                                          "Goals.RelativeAngle.Value GreaterThan [0] " +
                                                          "THEN Rotate.TurnRight AT [0.1]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [False] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [False] AND " +
                                                          "Goals.RelativeAngle.Value LessThan [0] " +
                                                          "THEN Move.GoForward AT [0.2]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [False] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [False] AND " +
                                                          "Goals.RelativeAngle.Value LessThan [0] " +
                                                          "THEN Rotate.TurnLeft AT [0.1]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [False] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [False] AND " +
                                                          "Goals.RelativeAngle.Value LessThan [0] " +
                                                          "THEN Move.GoForward AT [0.2]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [False] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [False] AND " +
                                                          "Goals.RelativeAngle.Value EqualTo [0] " +
                                                          "THEN Move.GoForward AT [0.4]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [True] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [True] THEN Move.GoBackward AT [1.0]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [True] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [True] THEN Rotate.TurnRight AT [0.5]");
            //IBrain newBrain = new BehaviourBrain(this, "IF ALWAYS THEN Rotate.TurnRight AT [0.1]");

            this.CompleteInitialization(null, 1, newBrain);
        }

        public override void ScenarioEndOfTurnTriggers()
        {
        }

        public override void Die()
        {
            base.Clone();
            base.Die();
        }
    }
}
