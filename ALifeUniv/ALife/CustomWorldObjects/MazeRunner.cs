using ALifeUni.ALife.Brains;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.CustomWorldObjects
{
    public class MazeRunner : Agent
    {
        public MazeRunner(Zone parentZone, Zone targetZone) : base("MazeRunner", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {
            Zone = parentZone;
            TargetZone = targetZone;

            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10);

            IShape myShape = new Circle(centrePoint, 5);
            StartOrientation = 0;
            myShape.Orientation.Degrees = 0;
            myShape.Color = Colors.Red;
            SetShape(myShape);

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
                new StatisticInput("Age", 0, Int32.MaxValue),
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

        public override void EndOfTurnTriggers()
        {
        }

        public override void Die()
        {
            base.Clone();
            base.Die();
        }

        public override void AgentUpkeep()
        {
            //Increment or Decrement end of turn values
            this.Statistics["Age"].IncreasePropertyBy(1);
        }
    }
}
