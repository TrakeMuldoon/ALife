using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
{
    class BoolBehaviour : Behaviour
    {
        public readonly Func<bool> Source;
        public readonly BoolOperationEnum Comparison;
        public readonly Func<bool> Target;


        //var beh = new Behaviour(null, NumericalOperationEnum.EqualTo, delegate () { return input.Value; }, null, null);
        public BoolBehaviour(Func<bool> source, BoolOperationEnum comparator, Func<bool> targetDouble, Action thenDoThis, Func<double> resultParam) : base(thenDoThis, resultParam)
        {
            Source = source;
            Comparison = comparator;
            Target = targetDouble;
        }

        //will be run once a "turn"
        public override void EvaluateBehaviour()
        {
            bool compareResult = false;
            switch (Comparison)
            {
                case BoolOperationEnum.EqualTo:
                    compareResult = Source() == Target();
                    break;
                case BoolOperationEnum.NoEqualTo:
                    compareResult = Source() != Target();
                    break;
                case BoolOperationEnum.IsTrue:
                    throw new Exception("Invalid behaviour asking for 'IsTrue'");
                case BoolOperationEnum.IsFalse:
                    throw new Exception("Invalid behaviour asking for 'IsFalse'");
                case BoolOperationEnum.AND:
                    compareResult = Source() && Target();
                    break;
                case BoolOperationEnum.OR:
                    compareResult = Source() || Target();
                    break;
                case BoolOperationEnum.NAND:
                    compareResult = !(Source() && Target());
                    break;
                case BoolOperationEnum.NOR:
                    compareResult = !(Source() || Target());
                    break;
                case BoolOperationEnum.XOR:
                    //same as !=
                    compareResult = Source() ^ Target();
                    break;
                case BoolOperationEnum.XNOR:
                    //same as ==
                    compareResult = !(Source() ^ Target());
                    break;
            }
            if (compareResult)
            {
                SuccessAction.Intensity += SuccessParam();
            }
        }
    }
}
