using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife.AgentPieces
{
    class Behaviour
    {
        public readonly Input Source;
        public readonly NumericalOperationEnum Comparison;
        public readonly Func<double> Target;
        public readonly Action SuccessAction;
        public readonly Func<double> SuccessParam;


        //var beh = new Behaviour(null, NumericalOperationEnum.EqualTo, delegate () { return input.Value; }, null, null);
        public Behaviour(Input source, NumericalOperationEnum comparator, Func<double> targetDouble, Action thenDoThis, Func<double> resultParam)
        {
            Source = source;
            Comparison = comparator;
            Target = targetDouble;
            SuccessAction = thenDoThis;
            SuccessParam = resultParam;
        }

        //will be run once a "turn"
        public void EvaluateBehaviour()
        {
            bool compareResult = false;
            switch (Comparison)
            {
                case NumericalOperationEnum.GreaterThan:
                    compareResult = Source.Value > Target();
                    break;
                case NumericalOperationEnum.LessThan:
                    compareResult = Source.Value < Target();
                    break;
                case NumericalOperationEnum.EqualTo:
                    compareResult = Source.Value == Target();
                    break;
                case NumericalOperationEnum.NotEqualTo:
                    compareResult = Source.Value != Target();
                    break;
                case NumericalOperationEnum.LessThanOrEqualTo:
                    compareResult = Source.Value <= Target();
                    break;
                case NumericalOperationEnum.GreaterThanOrEqualTo:
                    compareResult = Source.Value >= Target();
                    break;
            }
            if(compareResult)
            {
                SuccessAction.AttemptEnact(SuccessParam());
            }
        }
    }
}
