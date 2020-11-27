using System;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces.TypedClasses
{
    public enum DoubleOperationEnum
    {
        GreaterThan,
        LessThan,
        EqualTo,
        NotEqualTo,
        LessThanOrEqualTo,
        GreaterThanOrEqualTo
    }
    public static class DoubleConditionFactory
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
                b2 = cabinet.GetRandomBehaviourInputByType(typeof(double));
            }
            else
            {
                //constant
                BehaviourInput dummydouble = new BehaviourInput<double>(null, null);
                b2 = BehaviourFactory.GetBehaviourConstantFromString(dummydouble, "[" + GetRandomConstantValue().ToString() + "]");
            }
            return b2;
        }

        private static double GetRandomConstantValue()
        {
            double randomDouble = Math.Round(Planet.World.NumberGen.NextDouble(), 5);
            return randomDouble;
        }

        public static BehaviourCondition GetRandomBehaviourOperation(BehaviourInput b1, BehaviourInput b2)
        {
            Array arr = Enum.GetValues(typeof(DoubleOperationEnum));
            int rand = Planet.World.NumberGen.Next(0, arr.Length);
            DoubleOperationEnum val = (DoubleOperationEnum)arr.GetValue(rand);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        internal static BehaviourCondition GetConditionByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            DoubleOperationEnum val = (DoubleOperationEnum)Enum.Parse(typeof(DoubleOperationEnum), name);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        private static BehaviourCondition GetNewBehaviourByEnum(BehaviourInput b1, BehaviourInput b2, DoubleOperationEnum val)
        {
            switch(val)
            {
                case DoubleOperationEnum.GreaterThan:           return new BehaviourCondition<double>(b1, b2, (x, y) => Math.Round(x, 4) > Math.Round(y, 4), val.ToString());
                case DoubleOperationEnum.LessThan:              return new BehaviourCondition<double>(b1, b2, (x, y) => Math.Round(x, 4) < Math.Round(y, 4), val.ToString());
                case DoubleOperationEnum.EqualTo:               return new BehaviourCondition<double>(b1, b2, (x, y) => Math.Round(x, 4) == Math.Round(y, 4), val.ToString());
                case DoubleOperationEnum.NotEqualTo:            return new BehaviourCondition<double>(b1, b2, (x, y) => Math.Round(x, 4) != Math.Round(y, 4), val.ToString());
                case DoubleOperationEnum.LessThanOrEqualTo:     return new BehaviourCondition<double>(b1, b2, (x, y) => Math.Round(x, 4) <= Math.Round(y, 4), val.ToString());
                case DoubleOperationEnum.GreaterThanOrEqualTo:  return new BehaviourCondition<double>(b1, b2, (x, y) => Math.Round(x, 4) >= Math.Round(y, 4), val.ToString());
            }
            throw new Exception("Impossible Exception!");
        }
    }
}
