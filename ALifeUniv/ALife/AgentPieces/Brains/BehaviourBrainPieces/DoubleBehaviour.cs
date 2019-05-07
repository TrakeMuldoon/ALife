using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
{
    class DoubleBehaviour : Behaviour
    {
        public readonly Func<double> Source;
        public readonly NumericalOperationEnum Operation;
        public readonly Func<double> Target;

        //var beh = new Behaviour(null, NumericalOperationEnum.EqualTo, delegate () { return input.Value; }, null, null);
        public DoubleBehaviour(Func<double> source, NumericalOperationEnum comparator, Func<double> targetDouble, Action thenDoThis, Func<double> resultParam) : base(thenDoThis, resultParam)
        {
            Source = source;
            Operation = comparator;
            Target = targetDouble;
        }

        //will be run once a "turn"
        public override void EvaluateBehaviour()
        {
            bool compareResult = false;
            switch (Operation)
            {
                case NumericalOperationEnum.GreaterThan:
                    compareResult = Source() > Target();
                    break;
                case NumericalOperationEnum.LessThan:
                    compareResult = Source() < Target();
                    break;
                case NumericalOperationEnum.EqualTo:
                    compareResult = Source() == Target();
                    break;
                case NumericalOperationEnum.NotEqualTo:
                    compareResult = Source() != Target();
                    break;
                case NumericalOperationEnum.LessThanOrEqualTo:
                    compareResult = Source() <= Target();
                    break;
                case NumericalOperationEnum.GreaterThanOrEqualTo:
                    compareResult = Source() >= Target();
                    break;
                case NumericalOperationEnum.DecimalEqual:
                    double comp = Source() - Target();
                    compareResult = Math.Abs(comp % 1) <= (Double.Epsilon * 100);
                    break;
                case NumericalOperationEnum.IntegerEqual:
                    int sub = (int)Source() - (int)Target();
                    compareResult = sub == 0;
                    break;
            }
            if(compareResult)
            {
                SuccessAction.AttemptEnact(SuccessParam());
            }
        }
    }
}
