﻿using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces.TypedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
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
                case Input<double> d1:
                    bis.Add(new BehaviourInput<double>(myInput.Name + ".Value", () => d1.Value));
                    bis.Add(new BehaviourInput<double>(myInput.Name + ".MostRecentValue", () => d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Increased", () => d1.Value > d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Decreased", () => d1.Value < d1.MostRecentValue));
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Modified", () => d1.Modified));
                    break;
                default: throw new Exception("Unknown type " + myInput.GetContainedType() + " for BehaviourInputs");
            }
            return bis;
        }

        public static List<BehaviourInput> GenerateBehaviourInputsFromAction(Action act)
        {
            List<BehaviourInput> bis = new List<BehaviourInput>();
            bis.Add(new BehaviourInput<double>(act.Name + ".IntensityLastTurn", () => act.IntensityLastTurn));
            bis.Add(new BehaviourInput<bool>(act.Name + ".ActivatedLastTurn", () => act.ActivatedLastTurn));
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
                case BehaviourInput<bool> boo1: return BoolConditionFactory.GetRandomBehaviourOperation(b1, b2);
                case BehaviourInput<double> dob1: return DoubleConditionFactory.GetRandomBehaviourOperation(b1, b2);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }

        public static BehaviourCondition GetConditionForInputsByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            if(b1.GetContainedType() != b2.GetContainedType())
            {
                throw new Exception("Attempting to compare incomparables");
            }
            switch (b1)
            {
                case BehaviourInput<bool> boo1: return BoolConditionFactory.GetConditionByName(b1, b2, name);
                case BehaviourInput<double> dob1: return DoubleConditionFactory.GetConditionByName(b1, b2, name);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }

        internal static BehaviourInput GetBehaviourConstantFromString(BehaviourInput b1, string untrimmedConstant)
        {
            string con = untrimmedConstant.Trim('[', ']');
            switch (b1)
            {
                case BehaviourInput<bool> boo1:
                    bool bval = bool.Parse(con);
                    return new BehaviourInput<bool>(untrimmedConstant, () => bval);
                case BehaviourInput<double> dob1:
                    double dval = double.Parse(con);
                    return new BehaviourInput<double>(untrimmedConstant, () => dval);
                case BehaviourInput<string> str1:
                    string sval = untrimmedConstant;
                    return new BehaviourInput<string>(untrimmedConstant, () => sval);
                case BehaviourInput<int> int1:
                    int ival = int.Parse(untrimmedConstant);
                    return new BehaviourInput<int>(untrimmedConstant, () => ival);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }

        internal static BehaviourCondition GetRandomBehaviorConditionFromInput(BehaviourInput b1, BehaviourCabinet cabinet)
        {
            switch(b1)
            {
                case BehaviourInput<bool> boo1:
                    return BoolConditionFactory.GetRandomBehaviourConditionForBehaviour(b1, cabinet);
                case BehaviourInput<double> dob1:
                    return DoubleConditionFactory.GetRandomBehaviourConditionForBehaviour(b1, cabinet);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }
    }
}
