using System;
using System.Collections.Generic;
using System.Linq;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public class BehaviourCabinet
    {
        //TODO: Note that the dictionaries are unordered, which could cause reproducability problems.
        Dictionary<String, BehaviourInput> StringToBI = new Dictionary<string, BehaviourInput>();
        Dictionary<Type, List<BehaviourInput>> TypeToListBI = new Dictionary<Type, List<BehaviourInput>>();
        Dictionary<String, ActionPart> FullStringToActionPart = new Dictionary<string, ActionPart>();
        int totalInputs = 0;
        Agent myParent;

        public BehaviourCabinet(Agent parent)
        {
            myParent = parent;
            foreach(SenseCluster sc in parent.Senses)
            {
                foreach(SenseInput si in sc.SubInputs)
                {
                    List<BehaviourInput> currInputs = BehaviourFactory.GenerateBehaviourInputsFromInput((Input)si);
                    AddInputsFromInputList(currInputs);
                }
            }

            foreach(PropertyInput pi in parent.Properties.Values)
            {
                List<BehaviourInput> currInputs = BehaviourFactory.GenerateBehaviourInputsFromInput(pi);
                AddInputsFromInputList(currInputs);
            }
            foreach(StatisticInput si in parent.Statistics.Values)
            {
                List<BehaviourInput> currInputs = BehaviourFactory.GenerateBehaviourInputsFromInput(si);
                AddInputsFromInputList(currInputs);
            }
            foreach(ActionCluster act in parent.Actions.Values)
            {
                List<BehaviourInput> currInputs = BehaviourFactory.GenerateBehaviourInputsFromAction(act);
                AddInputsFromInputList(currInputs);

                foreach(ActionPart ap in act.SubActions.Values)
                {
                    FullStringToActionPart.Add(ap.FullName, ap);
                }
            }
            totalInputs = StringToBI.Count;
        }

        private void AddInputsFromInputList(List<BehaviourInput> behaviours)
        {
            foreach(BehaviourInput bi in behaviours)
            {
                StringToBI.Add(bi.FullName, bi);
                Type bit = bi.GetContainedType();
                if(!TypeToListBI.ContainsKey(bit))
                {
                    TypeToListBI.Add(bit, new List<BehaviourInput>());
                }
                TypeToListBI[bit].Add(bi);
            }
        }

        public BehaviourInput GetBehaviourInputByName(string name)
        {
            return StringToBI[name];
        }
        public BehaviourInput GetRandomBehaviourInputByType(Type type)
        {
            List<BehaviourInput> theList = TypeToListBI[type];
            int randomNumber = Planet.World.NumberGen.Next(0, theList.Count);
            return theList[randomNumber];
        }
        public BehaviourInput GetRandomBehaviourInput()
        {
            //A strange, but simple, little algorithm.
            //Each type has a different number of values, so there is no simple way to go directly to the correct type for that random number
            //So we must iteratively subtract each type until we find the one that the number lies within.
            int randomNumber = Planet.World.NumberGen.Next(0, totalInputs);
            foreach(Type aType in TypeToListBI.Keys)
            {
                randomNumber -= TypeToListBI[aType].Count;
                if(randomNumber < 0)
                {
                    randomNumber += TypeToListBI[aType].Count;
                    return TypeToListBI[aType][randomNumber];
                }
            }
            throw new IndexOutOfRangeException("Some, I was unable to return a random number");
        }

        public BehaviourCondition GetRandomConditionForInputs(BehaviourInput b1, BehaviourInput b2)
        {
            return BehaviourFactory.GetRandomConditionForInputs(b1, b2);
        }
        public BehaviourCondition GetConditionForInputsByName(BehaviourInput b1, BehaviourInput b2, String name)
        {
            return BehaviourFactory.GetConditionForInputsByName(b1, b2, name);
        }

        public ActionPart GetActionPartByFullName(string name)
        {
            return FullStringToActionPart[name];
        }
        public ActionPart GetRandomAction()
        {
            int randomActionNum = Planet.World.NumberGen.Next(FullStringToActionPart.Count);
            ActionPart randomAction = FullStringToActionPart.Values.ElementAt(randomActionNum);
            return randomAction;
        }
    }
}
