using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.WorldObjects
{
    public class MazeRunner : Agent
    {
        public MazeRunner(Zone parentZone, Zone targetZone) : base("MazeRunner", AgentIDGenerator.GetNextAgentId(), ReferenceValues.CollisionLevelPhysical)
        {
            this.Zone = parentZone;
            this.TargetZone = targetZone;

            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10); 

            IShape myShape = new Circle(centrePoint, 5);
            this.StartOrientation = 0;
            myShape.Orientation.Degrees = 0;
            myShape.Color = Colors.Red;
            this.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new EyeCluster(this, "EyeLeft"
                                , new ROEvoNumber(startValue: -30, evoDeltaMax: 20, hardMin:-360, hardMax: 360)   //Orientation Around Parent
                                , new ROEvoNumber(startValue: 10, evoDeltaMax:30, hardMin:-360, hardMax: 360)    //Relative Orientation
                                , new ROEvoNumber(startValue: 80, evoDeltaMax:3, hardMin:40, hardMax:120)        //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax:1, hardMin:15, hardMax:40)),       //Sweep
                new EyeCluster(this, "EyeRight"
                                , new ROEvoNumber(startValue: 30, evoDeltaMax: 20, hardMin:-360, hardMax: 360)   //Orientation Around Parent
                                , new ROEvoNumber(startValue: -10, evoDeltaMax:30, hardMin:-360, hardMax: 360)   //Relative Orientation
                                , new ROEvoNumber(startValue: 80, evoDeltaMax:3, hardMin:40, hardMax:120)        //Radius
                                , new ROEvoNumber(startValue: 25, evoDeltaMax:1, hardMin:15, hardMax:40)),       //Sweep
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

            IBrain newBrain = new BehaviourBrain(this, "IF EyeLeft.SeeSomething.Value Equals [True] THEN Rotate.TurnRight AT [0.1]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [True] THEN Rotate.TurnLeft AT [0.1]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [False] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [False] THEN Move.GoForward AT [3.0]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [True] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [True] THEN Move.GoBackward AT [5.0]"
                                                     , "IF EyeRight.SeeSomething.Value Equals [True] AND " +
                                                          "EyeLeft.SeeSomething.Value Equals [True] THEN Rotate.TurnRight AT [0.05]");
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
