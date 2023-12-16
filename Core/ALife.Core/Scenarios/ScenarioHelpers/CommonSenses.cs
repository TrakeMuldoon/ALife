using System.Collections.Generic;
using ALife.Core.Utility;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Senses;

namespace ALife.Core.Scenarios.ScenarioHelpers
{
    public static class CommonSenses
    {
        public static List<SenseCluster> PairOfEyes(Agent agent)
        {
            return PairOfEyes(agent, 0);
        }

        public static List<SenseCluster> PairOfEyes(Agent agent, int orientationOffset)
        {
            return new List<SenseCluster>() {
                new EyeCluster(agent, "EyeLeft"
                    , new ROEvoNumber(startValue: -30 + orientationOffset, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: 10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)                         //Relative Orientation
                    , new ROEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15, hardMax: 40)),                          //Sweep
                new EyeCluster(agent, "EyeRight"
                    , new ROEvoNumber(startValue: 30 + orientationOffset, evoDeltaMax: 5, hardMin: -360, hardMax: 360)     //Orientation Around Parent
                    , new ROEvoNumber(startValue: -10, evoDeltaMax: 5, hardMin: -360, hardMax: 360)                        //Relative Orientation
                    , new ROEvoNumber(startValue: 80, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: 25, evoDeltaMax: 1, hardMin: 15, hardMax: 40))                           //Sweep
            };
        }

        public static List<SenseCluster> QuadrantEyes(Agent agent, int orientationOffset)
        {
            int sweep = 90;
            int radius = 40;

            return new List<SenseCluster>() {
                new EyeCluster(agent, "EyeStraight"
                    , new ROEvoNumber(startValue: 0 + orientationOffset, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: 0,  evoDeltaMax: 5, hardMin: -360, hardMax: 360)                         //Relative Orientation
                    , new ROEvoNumber(startValue: radius, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: sweep, evoDeltaMax: 1, hardMin: 80, hardMax: 110)),                          //Sweep
                new EyeCluster(agent, "EyeRight"
                    , new ROEvoNumber(startValue: 90 + orientationOffset, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: 0,  evoDeltaMax: 5, hardMin: -360, hardMax: 360)                         //Relative Orientation
                    , new ROEvoNumber(startValue: radius, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: sweep, evoDeltaMax: 1, hardMin: 80, hardMax: 110)),
                new EyeCluster(agent, "EyeBack"
                    , new ROEvoNumber(startValue: 180 + orientationOffset, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: 0,  evoDeltaMax: 5, hardMin: -360, hardMax: 360)                         //Relative Orientation
                    , new ROEvoNumber(startValue: radius, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: sweep, evoDeltaMax: 1, hardMin: 80, hardMax: 110)),
                new EyeCluster(agent, "EyeLeft"
                    , new ROEvoNumber(startValue: 270 + orientationOffset, evoDeltaMax: 5, hardMin: -360, hardMax: 360)    //Orientation Around Parent
                    , new ROEvoNumber(startValue: 0,  evoDeltaMax: 5, hardMin: -360, hardMax: 360)                         //Relative Orientation
                    , new ROEvoNumber(startValue: radius, evoDeltaMax: 3, hardMin: 40, hardMax: 120)                           //Radius
                    , new ROEvoNumber(startValue: sweep, evoDeltaMax: 1, hardMin: 80, hardMax: 110)),
            };
        }
    }
}
