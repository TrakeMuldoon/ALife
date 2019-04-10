using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    public abstract class Behaviour
    {
        public String AsEnglish;

        public readonly Action SuccessAction;
        public readonly Func<double> SuccessParam;

        protected Behaviour(Action thenDoThis, Func<double> resultParam)
        {
            SuccessAction = thenDoThis;
            SuccessParam = resultParam;
        }

        //will be run once a "turn"
        public abstract void EvaluateBehaviour();
    }
}
