using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces;
using ALifeUni.ALife.Inputs.SenseClusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
{
    public class Behaviour
    {
        public readonly String AsEnglish;
        public readonly List<BehaviourCondition> Conditions = new List<BehaviourCondition>();
        public readonly Action SuccessAction;
        public readonly Func<double> SuccessParam;

        //protected Behaviour(Action thenDoThis, Func<double> resultParam)
        //{
        //    SuccessAction = thenDoThis;
        //    SuccessParam = resultParam;
        //}
        protected Behaviour(String englishString)
        {
            AsEnglish = englishString;
            Regex englishStringParser =
                new Regex("If ([^\\s]+)( and( [^\\s]+))* then( Wait\\(\\d+\\) to )?( [^\\s]+) with intensity( [^\\s]+)\\.");
        }

        //will be run once a "turn"
        public void EvaluateBehaviour()
        {
            foreach(BehaviourCondition bc in Conditions)
            {
                if(!bc.EvaluateSuccess())
                {
                    //AND is the only logic, therefore if they fail a condition, this behaviour does not evaluate
                    return;
                }
            }

            SuccessAction.Intensity += SuccessParam();
        }
    }
}
