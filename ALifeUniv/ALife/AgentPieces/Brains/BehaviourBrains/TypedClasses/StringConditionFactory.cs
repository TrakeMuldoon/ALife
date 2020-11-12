using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces.TypedClasses
{
    public enum StringOperationEnum
    {
        EqualTo,
        NotEqualTo,
        Contains,
        DoesNotContain,
        StartsWith,
        DoesNotStartWith
    }
    public static class StringConditionFactory
    {
        public static BehaviourCondition GetRandomBehaviourConditionForBehaviour(BehaviourInput b1, BehaviourCabinet cabinet)
        {
            double variableOrConstant = Planet.World.NumberGen.NextDouble();

            BehaviourInput b2;
            if(variableOrConstant > 0.5)
            {
                //variable
                b2 = cabinet.GetRandomBehaviourInputByType(b1.GetContainedType());
            }
            else
            {
                //constant
                b2 = BehaviourFactory.GetBehaviourConstantFromString(b1, GetRandomConstantValue().ToString());
            }
            return GetRandomBehaviourOperation(b1, b2);
        }

        private static string GetRandomConstantValue()
        {
            throw new NotImplementedException("This is not implemented");
        }

        public static BehaviourCondition GetRandomBehaviourOperation(BehaviourInput b1, BehaviourInput b2)
        {
            Array arr = Enum.GetValues(typeof(StringOperationEnum));
            int rand = Planet.World.NumberGen.Next(0, arr.Length);
            StringOperationEnum val = (StringOperationEnum) arr.GetValue(rand);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        internal static BehaviourCondition GetConditionByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            StringOperationEnum val = (StringOperationEnum) Enum.Parse(typeof(StringOperationEnum), name);
            return GetNewBehaviourByEnum(b1, b2, val);
        }

        private static BehaviourCondition GetNewBehaviourByEnum(BehaviourInput b1, BehaviourInput b2, StringOperationEnum val)
        {
            switch (val)
            {
                case StringOperationEnum.EqualTo:           return new BehaviourCondition<string>(b1, b2, (x, y) => x == y, val.ToString());
                case StringOperationEnum.NotEqualTo:        return new BehaviourCondition<string>(b1, b2, (x, y) => x != y, val.ToString());
                case StringOperationEnum.Contains:          return new BehaviourCondition<string>(b1, b2, (x, y) => x.Contains(y), val.ToString());
                case StringOperationEnum.DoesNotContain:    return new BehaviourCondition<string>(b1, b2, (x, y) => !x.Contains(y), val.ToString());
                case StringOperationEnum.StartsWith:        return new BehaviourCondition<string>(b1, b2, (x, y) => x.StartsWith(y), val.ToString());
                case StringOperationEnum.DoesNotStartWith:  return new BehaviourCondition<string>(b1, b2, (x, y) => !x.StartsWith(y), val.ToString());
            }
            throw new Exception("Impossible Exception!");
        }
    }
}
