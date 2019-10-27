using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces.TypedClasses
{
    public enum BoolOperationEnum
    {
        EqualTo,
        NotEqualTo,
        AND,
        OR,
        NAND,
        NOR,
        XOR,
        XNOR
    }
    public static class BoolConditionFactory
    {
        public static BehaviourCondition GetRandomBehaviour(BehaviourInput b1, BehaviourInput b2)
        {
            Array arr = Enum.GetValues(typeof(BoolOperationEnum));
            int rand = Planet.World.NumberGen.Next(0, arr.Length);
            BoolOperationEnum val = (BoolOperationEnum) arr.GetValue(rand);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        internal static BehaviourCondition GetConditionByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            BoolOperationEnum val = (BoolOperationEnum) Enum.Parse(typeof(BoolOperationEnum), name);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        private static BehaviourCondition GetNewBehaviourByEnum(BehaviourInput b1, BehaviourInput b2, BoolOperationEnum val)
        {
            switch (val)
            {
                case BoolOperationEnum.EqualTo:     return new BehaviourCondition<bool>(b1, b2, (x, y) => x == y);
                case BoolOperationEnum.NotEqualTo:  return new BehaviourCondition<bool>(b1, b2, (x, y) => x != y);
                case BoolOperationEnum.AND:         return new BehaviourCondition<bool>(b1, b2, (x, y) => x && y);
                case BoolOperationEnum.OR:          return new BehaviourCondition<bool>(b1, b2, (x, y) => x || y);
                case BoolOperationEnum.NAND:        return new BehaviourCondition<bool>(b1, b2, (x, y) => !(x && y));
                case BoolOperationEnum.NOR:         return new BehaviourCondition<bool>(b1, b2, (x, y) => !(x || y));
                case BoolOperationEnum.XOR:         return new BehaviourCondition<bool>(b1, b2, (x, y) => x ^ y);
                case BoolOperationEnum.XNOR:        return new BehaviourCondition<bool>(b1, b2, (x, y) => !(x ^ y));
            }
            throw new Exception("Impossible Exception!");
        }
    }
}
