using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces;
using ALifeUni.ALife.Inputs.SenseClusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
{
    public abstract class Behaviour
    {
        public String AsEnglish;

        public readonly List<BehaviourCondition> Conditions = new List<BehaviourCondition>();
        public readonly Action SuccessAction;
        public readonly Func<double> SuccessParam;

        protected Behaviour(Action thenDoThis, Func<double> resultParam)
        {
            SuccessAction = thenDoThis;
            SuccessParam = resultParam;

            BehaviourCondition<bool> bc = new BehaviourCondition<bool>();
            //bc.inputTarget = new EyeInput();
            //bc.comparator = (x, y) =>  x == y;
            //bc.compareTo = true;


        }

        //will be run once a "turn"
        public abstract void EvaluateBehaviour();
    }
}
