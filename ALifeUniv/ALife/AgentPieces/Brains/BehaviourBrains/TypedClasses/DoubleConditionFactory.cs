using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces.TypedClasses
{
    public enum DoubleOperationEnum
    {
        GreaterThan,
        LessThan,
        EqualTo,
        NotEqualTo,
        LessThanOrEqualTo,
        GreaterThanOrEqualTo,
        IntegerEqual,
        DecimalEqual
    }
    public static class DoubleConditionFactory
    {
        public static BehaviourCondition GetRandomBehaviour(BehaviourInput b1, BehaviourInput b2)
        {
            Array arr = Enum.GetValues(typeof(DoubleOperationEnum));
            int rand = Planet.World.NumberGen.Next(0, arr.Length);
            DoubleOperationEnum val = (DoubleOperationEnum) arr.GetValue(rand);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        internal static BehaviourCondition GetConditionByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            DoubleOperationEnum val = (DoubleOperationEnum) Enum.Parse(typeof(DoubleOperationEnum), name);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        private static BehaviourCondition GetNewBehaviourByEnum(BehaviourInput b1, BehaviourInput b2, DoubleOperationEnum val)
        {
            switch (val)
            {
                case DoubleOperationEnum.GreaterThan:           return new BehaviourCondition<double>(b1, b2, (x, y) => x > y);
                case DoubleOperationEnum.LessThan:              return new BehaviourCondition<double>(b1, b2, (x, y) => x < y);
                case DoubleOperationEnum.EqualTo:               return new BehaviourCondition<double>(b1, b2, (x, y) => x == y);
                case DoubleOperationEnum.NotEqualTo:            return new BehaviourCondition<double>(b1, b2, (x, y) => x != y);
                case DoubleOperationEnum.LessThanOrEqualTo:     return new BehaviourCondition<double>(b1, b2, (x, y) => x <= y);
                case DoubleOperationEnum.GreaterThanOrEqualTo:  return new BehaviourCondition<double>(b1, b2, (x, y) => x >= y);
                case DoubleOperationEnum.IntegerEqual:          return new BehaviourCondition<double>(b1, b2, (x, y) => x/1 == y/1);
                case DoubleOperationEnum.DecimalEqual:          return new BehaviourCondition<double>(b1, b2, (x, y) => x%1 == y%1);
            }
            throw new Exception("Impossible Exception!");
        }
    }
}
