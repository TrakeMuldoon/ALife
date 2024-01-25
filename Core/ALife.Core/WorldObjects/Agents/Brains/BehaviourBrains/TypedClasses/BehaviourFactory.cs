using ALife.Core.WorldObjects.Agents.AgentActions;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Brains.BehaviourBrains.TypedClasses
{
    public static class BehaviourFactory
    {
        #region Generating Inputs
        public static List<BehaviourInput> GenerateBehaviourInputsFromInput(Input myInput)
        {
            List<BehaviourInput> bis = new List<BehaviourInput>();
            switch(myInput)
            {
                case Input<bool> b1:
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Value", () => b1.Value));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".MostRecentValue", () => b1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Modified", () => b1.Modified));
                    break;
                case Input<string> s1:
                    bis.Add(new BehaviourInput<string>(myInput.Name + ".Value", () => s1.Value));
                    bis.Add(new BehaviourInput<string>(myInput.Name + ".MostRecentValue", () => s1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Modified", () => s1.Modified));
                    break;
                case Input<double> d1:
                    bis.Add(new BehaviourInput<double>(myInput.Name + ".Value", () => d1.Value));
                    bis.Add(new BehaviourInput<double>(myInput.Name + ".MostRecentValue", () => d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Increased", () => d1.Value > d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Decreased", () => d1.Value < d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Modified", () => d1.Modified));
                    break;
                case Input<int> d1:
                    bis.Add(new BehaviourInput<int>(myInput.Name + ".Value", () => d1.Value));
                    bis.Add(new BehaviourInput<int>(myInput.Name + ".MostRecentValue", () => d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Increased", () => d1.Value > d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Decreased", () => d1.Value < d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Modified", () => d1.Modified));
                    break;
                default: throw new Exception("Unknown type " + myInput.GetContainedType() + " for BehaviourInputs");
            }
            return bis;
        }

        public static List<BehaviourInput> GenerateBehaviourInputsFromAction(ActionCluster act)
        {
            List<BehaviourInput> bis = new List<BehaviourInput>();
            bis.Add(new BehaviourInput<bool>(act.Name + ".ActivatedLastTurn", () => act.ActivatedLastTurn));
            foreach(ActionPart ap in act.SubActions.Values)
            {
                bis.Add(new BehaviourInput<double>(ap.FullName + ".IntensityLastTurn", () => ap.IntensityLastTurn));
            }
            return bis;
        }
        #endregion

        public static BehaviourCondition GetRandomConditionForInputs(BehaviourInput b1, BehaviourInput b2)
        {
            if(b1.GetContainedType() != b2.GetContainedType())
            {
                throw new Exception("Attempting to compare incomparables");
            }
            switch(b1)
            {
                case BehaviourInput<bool> boo1:     return BoolConditionFactory.GetRandomBehaviourOperation(b1, b2);
                case BehaviourInput<double> dob1:   return DoubleConditionFactory.GetRandomBehaviourOperation(b1, b2);
                case BehaviourInput<int> int1:      return IntConditionFactory.GetRandomBehaviourOperation(b1, b2);
                case BehaviourInput<string> str1:   return StringConditionFactory.GetRandomBehaviourOperation(b1, b2);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }

        public static BehaviourCondition GetConditionForInputsByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            if(b1.GetContainedType() != b2.GetContainedType())
            {
                throw new Exception("Attempting to compare incomparables");
            }
            switch(b1)
            {
                case BehaviourInput<bool> boo1:     return BoolConditionFactory.GetConditionByName(b1, b2, name);
                case BehaviourInput<double> dob1:   return DoubleConditionFactory.GetConditionByName(b1, b2, name);
                case BehaviourInput<int> int1:      return IntConditionFactory.GetConditionByName(b1, b2, name);    
                case BehaviourInput<string> str1:   return StringConditionFactory.GetConditionByName(b1, b2, name);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }

        internal static BehaviourInput GetBehaviourConstantFromString(BehaviourInput b1, string untrimmedConstant)
        {
            string con = untrimmedConstant.Trim('[', ']');
            switch(b1)
            {
                case BehaviourInput<bool> boo1:     bool bval = bool.Parse(con);
                    return new BehaviourInput<bool>(untrimmedConstant, () => bval);
                case BehaviourInput<double> dob1:   double dval = double.Parse(con);
                    return new BehaviourInput<double>(untrimmedConstant, () => dval);
                case BehaviourInput<int> int1:      int ival = int.Parse(con);
                    return new BehaviourInput<int>(untrimmedConstant, () => ival);
                case BehaviourInput<string> str1:   string sval = untrimmedConstant;
                    return new BehaviourInput<string>(untrimmedConstant, () => sval);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }

        internal static BehaviourCondition GetRandomBehaviorConditionFromInput(BehaviourInput b1, BehaviourCabinet cabinet)
        {
            switch(b1)
            {
                case BehaviourInput<bool> boo1:     return BoolConditionFactory.GetRandomBehaviourConditionForBehaviour(b1, cabinet);
                case BehaviourInput<double> dob1:   return DoubleConditionFactory.GetRandomBehaviourConditionForBehaviour(b1, cabinet);
                case BehaviourInput<int> int1:      return IntConditionFactory.GetRandomBehaviourConditionForBehaviour(b1, cabinet);
                case BehaviourInput<string> str1:   return StringConditionFactory.GetRandomBehaviourConditionForBehaviour(b1, cabinet);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }
    }
}
