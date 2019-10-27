using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces.TypedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public static class BehaviourInputFactory
    {
        public static List<BehaviourInput> BehaviourInputsFromInput(Input myInput)
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
                    bis.Add(new BehaviourInput<bool>(myInput.Name + ".Modified", () => d1.Modified));
                    break;
                default: throw new Exception("Unknown type " + myInput.GetContainedType() + " for BehaviourInputs");
            }
            return bis;
        }

        public static List<BehaviourInput> BehaviourInputsFromAction(Action act)
        {
            List<BehaviourInput> bis = new List<BehaviourInput>();
            bis.Add(new BehaviourInput<double>(act.Name + ".IntensityLastTurn", () => act.IntensityLastTurn));
            bis.Add(new BehaviourInput<bool>(act.Name + ".ActivatedLastTurn", () => act.ActivatedLastTurn));
            return bis;
        }

        public static BehaviourCondition GetRandomConditionForInputs(BehaviourInput b1, BehaviourInput b2)
        {
            switch(b1)
            {
                case BehaviourInput<bool> boo1: return BoolConditionFactory.GetRandomBehaviour(b1, b2);
                case BehaviourInput<double> dob1: return DoubleConditionFactory.GetRandomBehaviour(b1, b2);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }

        public static BehaviourCondition GetConditionForInputsByName(BehaviourInput b1, BehaviourInput b2, string name)
        {
            switch (b1)
            {
                case BehaviourInput<bool> boo1: return BoolConditionFactory.GetConditionByName(b1, b2, name);
                case BehaviourInput<double> dob1: return DoubleConditionFactory.GetConditionByName(b1, b2, name);
                default: throw new NotImplementedException("unimiplemented condition type: " + b1.GetContainedType());
            }
        }
    }
}
