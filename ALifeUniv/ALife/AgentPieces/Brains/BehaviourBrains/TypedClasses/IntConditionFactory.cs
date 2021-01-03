using System;

namespace ALifeUni.ALife.Brains
{
    public enum IntOperationEnum
    {
        GreaterThan,
        LessThan,
        EqualTo,
        NotEqualTo,
        LessThanOrEqualTo,
        GreaterThanOrEqualTo
    }

    public static class IntConditionFactory
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
                b2 = cabinet.GetRandomBehaviourInputByType(typeof(int));
            }
            else
            {
                //constant
                BehaviourInput dummyint = new BehaviourInput<int>(null, null);
                b2 = BehaviourFactory.GetBehaviourConstantFromString(dummyint, "[" + GetRandomConstantValue().ToString() + "]");
            }
            return b2;
        }

        private static int GetRandomConstantValue()
        {
            int randomint = Planet.World.NumberGen.Next(0, 100);
            return randomint;
        }

        public static BehaviourCondition GetRandomBehaviourOperation(BehaviourInput b1, BehaviourInput b2)
        {
            Array arr = Enum.GetValues(typeof(IntOperationEnum));
            int rand = Planet.World.NumberGen.Next(0, arr.Length);
            IntOperationEnum val = (IntOperationEnum)arr.GetValue(rand);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        internal static BehaviourCondition GetConditionByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            IntOperationEnum val = (IntOperationEnum)Enum.Parse(typeof(IntOperationEnum), name);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        private static BehaviourCondition GetNewBehaviourByEnum(BehaviourInput b1, BehaviourInput b2, IntOperationEnum val)
        {
            switch(val)
            {
                case IntOperationEnum.GreaterThan:          return new BehaviourCondition<int>(b1, b2, (x, y) => x > y, val.ToString());
                case IntOperationEnum.LessThan:             return new BehaviourCondition<int>(b1, b2, (x, y) => x < y, val.ToString());
                case IntOperationEnum.EqualTo:              return new BehaviourCondition<int>(b1, b2, (x, y) => x == y, val.ToString());
                case IntOperationEnum.NotEqualTo:           return new BehaviourCondition<int>(b1, b2, (x, y) => x != y, val.ToString());
                case IntOperationEnum.LessThanOrEqualTo:    return new BehaviourCondition<int>(b1, b2, (x, y) => x <= y, val.ToString());
                case IntOperationEnum.GreaterThanOrEqualTo: return new BehaviourCondition<int>(b1, b2, (x, y) => x >= y, val.ToString());
            }
            throw new Exception("Impossible Exception!");
        }
    }
}
