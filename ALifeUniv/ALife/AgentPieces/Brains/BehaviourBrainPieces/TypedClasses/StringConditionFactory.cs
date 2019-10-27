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
        NoEqualTo,
        Contains,
        DoesNotContain,
        StartsWith,
        DoesNotStartWith
    }
    public static class StringConditionFactory
    {
        public static BehaviourCondition GetRandomBehaviour(BehaviourInput b1, BehaviourInput b2)
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
                case StringOperationEnum.EqualTo:           return new BehaviourCondition<string>(b1, b2, (x, y) => x == y);
                case StringOperationEnum.NoEqualTo:         return new BehaviourCondition<string>(b1, b2, (x, y) => x != y);
                case StringOperationEnum.Contains:          return new BehaviourCondition<string>(b1, b2, (x, y) => x.Contains(y));
                case StringOperationEnum.DoesNotContain:    return new BehaviourCondition<string>(b1, b2, (x, y) => !x.Contains(y));
                case StringOperationEnum.StartsWith:        return new BehaviourCondition<string>(b1, b2, (x, y) => x.StartsWith(y));
                case StringOperationEnum.DoesNotStartWith:  return new BehaviourCondition<string>(b1, b2, (x, y) => !x.StartsWith(y));
            }
            throw new Exception("Impossible Exception!");
        }
    }
}
