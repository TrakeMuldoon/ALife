using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife.AgentPieces
{
    class BoolBehaviour : Behaviour
    {
        public readonly Func<bool> Source;
        public readonly NumericalOperationEnum Comparison;
        public readonly Func<bool> Target;


        //var beh = new Behaviour(null, NumericalOperationEnum.EqualTo, delegate () { return input.Value; }, null, null);
        public Behaviour(Func<bool> source, NumericalOperationEnum comparator, Func<bool> targetDouble, Action thenDoThis, Func<double> resultParam)
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
            if (compareResult)
            {
                SuccessAction.AttemptEnact(SuccessParam());
            }
        }
    }
}
