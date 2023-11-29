using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Scenarios.ScenarioHelpers
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
    }
}
