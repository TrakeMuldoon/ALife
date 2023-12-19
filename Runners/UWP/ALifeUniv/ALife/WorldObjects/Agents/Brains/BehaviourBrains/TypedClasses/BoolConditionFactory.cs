using System;

namespace ALifeUni.ALife.WorldObjects.Agents.Brains.BehaviourBrains
{
    public enum BoolOperationEnum
    {
        Equals,
        NotEqualTo,
        WITH,
        OR,
        NAND,
        NOR,
        XOR,
        XNOR
    }
    public static class BoolConditionFactory
    {
        public static BehaviourCondition GetRandomBehaviourConditionForBehaviour(BehaviourInput b1, BehaviourCabinet cabinet)
        {
            BehaviourInput b2 = GetRandomVariableOrConstant(cabinet);
            return GetRandomBehaviourOperation(b1, b2);
        }

        public static BehaviourInput GetRandomVariableOrConstant(BehaviourCabinet cabinet)
        {
            double variableOrConstant = Planet.World.NumberGen.NextDouble();

            BehaviourInput b2;
            if(variableOrConstant > 0.5)
            {
                //variable
                b2 = cabinet.GetRandomBehaviourInputByType(typeof(bool));
            }
            else
            {
                //constant
                BehaviourInput dummybool = new BehaviourInput<bool>(null, null);
                b2 = BehaviourFactory.GetBehaviourConstantFromString(dummybool, "[" + GetRandomConstantValue().ToString() + "]");
            }
            return b2;
        }

        private static bool GetRandomConstantValue()
        {
            double randomBool = Planet.World.NumberGen.NextDouble();

            return randomBool > 0.5;
        }

        public static BehaviourCondition GetRandomBehaviourOperation(BehaviourInput b1, BehaviourInput b2)
        {
            Array arr = Enum.GetValues(typeof(BoolOperationEnum));
            int rand = Planet.World.NumberGen.Next(0, arr.Length);
            BoolOperationEnum val = (BoolOperationEnum)arr.GetValue(rand);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        internal static BehaviourCondition GetConditionByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            BoolOperationEnum val = (BoolOperationEnum)Enum.Parse(typeof(BoolOperationEnum), name);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        private static BehaviourCondition GetNewBehaviourByEnum(BehaviourInput b1, BehaviourInput b2, BoolOperationEnum val)
        {
            switch(val)
            {
                case BoolOperationEnum.Equals:      return new BehaviourCondition<bool>(b1, b2, (x, y) => x == y, val.ToString());
                case BoolOperationEnum.NotEqualTo:  return new BehaviourCondition<bool>(b1, b2, (x, y) => x != y, val.ToString());
                case BoolOperationEnum.WITH:        return new BehaviourCondition<bool>(b1, b2, (x, y) => x && y, val.ToString());
                case BoolOperationEnum.OR:          return new BehaviourCondition<bool>(b1, b2, (x, y) => x || y, val.ToString());
                case BoolOperationEnum.NAND:        return new BehaviourCondition<bool>(b1, b2, (x, y) => !(x && y), val.ToString());
                case BoolOperationEnum.NOR:         return new BehaviourCondition<bool>(b1, b2, (x, y) => !(x || y), val.ToString());
                case BoolOperationEnum.XOR:         return new BehaviourCondition<bool>(b1, b2, (x, y) => x ^ y, val.ToString());
                case BoolOperationEnum.XNOR:        return new BehaviourCondition<bool>(b1, b2, (x, y) => !(x ^ y), val.ToString());
            }
            throw new Exception("Impossible Exception!");
        }
    }
}
