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
    public class MushroomGatherer : Agent
    {
        public MushroomGatherer(Zone parentZone)
            : base("MushroomGatherer", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {
            HomeZone = parentZone;

            int agentRadius = 5;
            ApplyCircleShapeToAgent(HomeZone.Distributor, Colors.Red, agentRadius, 0);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(this, "EyeLeft", true
                                , new ROEvoNumber(startValue: -2, evoDeltaMax:0.2, hardMin:-360, hardMax: 360)  //Orientation Around Parent
                                , new ROEvoNumber(startValue: 15, evoDeltaMax:0.2, hardMin:-360, hardMax: 360)  //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax:0.2, hardMin:40, hardMax:90)      //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax:0.2, hardMin:15, hardMax:40)),    //Sweep
                new EyeCluster(this, "EyeRight", true
                                , new ROEvoNumber(startValue: 2, evoDeltaMax: 0.2, hardMin:-360, hardMax: 360)   //Orientation Around Parent
                                , new ROEvoNumber(startValue: -15, evoDeltaMax:0.2, hardMin:-360, hardMax: 360)  //Relative Orientation
                                , new ROEvoNumber(startValue: 60, evoDeltaMax:0.2, hardMin:40, hardMax:90)       //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax:0.2, hardMin:15, hardMax:40)),     //Sweep
                new EyeCluster(this, "BackEye", false
                                , new ROEvoNumber(startValue: 180, evoDeltaMax: 0.2, hardMin:-360, hardMax: 360) //Orientation Around Parent
                                , new ROEvoNumber(startValue: -90, evoDeltaMax:0.2, hardMin:-360, hardMax: 360)    //Relative Orientation
                                , new ROEvoNumber(startValue: 15, evoDeltaMax:0.2, hardMin:5, hardMax:50)        //Radius
                                , new ROEvoNumber(startValue: 170, evoDeltaMax:0.2, hardMin:160, hardMax:180))     //Sweep
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();

            List<StatisticInput> agentStatistics = new List<StatisticInput>()
            {
                new StatisticInput("Age", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("DeathTimer", 0, Int32.MaxValue, StatisticInputType.Incrementing),
                new StatisticInput("HowFullAmI", 0, Int32.MaxValue),
                new StatisticInput("Kills", 0, Int32.MaxValue),
            };

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(this),
                new RotateCluster(this)
            };

            AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            IBrain newBrain = new BehaviourBrain(this,
                "IF EyeLeft.IsRed.Value Equals [True] THEN Rotate.TurnRight AT [0.040]",
                "IF EyeRight.IsRed.Value Equals [True] THEN Rotate.TurnLeft AT [0.060]",
                "IF EyeLeft.IsBlue.Value Equals [True] THEN Rotate.TurnLeft AT [0.070]",
                "IF EyeLeft.IsBlue.Value Equals [True] THEN Move.GoForward AT [1.0]",
                "IF EyeRight.IsBlue.Value Equals [True] THEN Rotate.TurnRight AT [0.060]",
                "IF EyeRight.IsBlue.Value Equals [True] THEN Move.GoForward AT [1.0]",
                "IF EyeLeft.IsGreen.Value Equals [True] THEN Rotate.TurnLeft AT [0.070]",
                "IF EyeLeft.IsGreen.Value Equals [True] THEN Move.GoForward AT [1.0]",
                "IF EyeRight.IsGreen.Value Equals [True] THEN Rotate.TurnRight AT [0.060]",
                "IF EyeRight.IsGreen.Value Equals [True] THEN Move.GoForward AT [0.2]",
                "IF BackEye.SeeSomething.Value Equals [True] THEN Move.GoForward AT [1.0]",
                "IF ALWAYS THEN Move.GoForward AT [0.3]",
                "IF ALWAYS THEN Rotate.TurnRight AT [0.015]"
                );
            CompleteInitialization(null, 1, newBrain);
        }
    }
}
