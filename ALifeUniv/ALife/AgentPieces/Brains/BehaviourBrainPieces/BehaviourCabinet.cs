﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public class BehaviourCabinet
    {
        //TODO: Note that the dictionaries are unordered, which could cause reproducability problems.
        Dictionary<String, BehaviourInput> StringToBI = new Dictionary<string, BehaviourInput>();
        Dictionary<Type, List<BehaviourInput>> TypeToListBI = new Dictionary<Type, List<BehaviourInput>>();
        int totalInputs = 0;

        public BehaviourCabinet(Agent parent)
        {
            foreach(SenseCluster sc in parent.Senses)
            {
                foreach(SenseInput si in sc.SubInputs)
                {
                    List<BehaviourInput> currInputs = BehaviourInputFactory.BehaviourInputsFromInput((Input)si);
                    AddInputsFromInputList(currInputs);
                }
            }
            foreach(PropertyInput pi in parent.Properties.Values)
            {
                List<BehaviourInput> currInputs = BehaviourInputFactory.BehaviourInputsFromInput((Input)pi);
                AddInputsFromInputList(currInputs);
            }
            foreach(Action act in parent.Actions.Values)
            {
                List<BehaviourInput> currInputs = BehaviourInputFactory.BehaviourInputsFromAction(act);
                AddInputsFromInputList(currInputs);
            }
            totalInputs = StringToBI.Count;
        }

        private void AddInputsFromInputList(List<BehaviourInput> behaviours)
        {
            foreach (BehaviourInput bi in behaviours)
            {
                StringToBI.Add(bi.FullName, bi);
                Type bit = bi.GetContainedType();
                if (!TypeToListBI.ContainsKey(bit))
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
            int randomNumber = Planet.World.NumberGen.Next(0, totalInputs);
            foreach(Type aType in TypeToListBI.Keys)
            {
                randomNumber -= TypeToListBI[aType].Count;
                if (randomNumber < 0)
                {
                    randomNumber += TypeToListBI[aType].Count;
                    return TypeToListBI[aType][randomNumber];
                }
            }
            throw new IndexOutOfRangeException("Some, I was unable to return a random number");
        }
        
        public BehaviourCondition GetRandomConditionForInputs(BehaviourInput b1, BehaviourInput b2)
        {
            return BehaviourInputFactory.GetRandomConditionForInputs(b1, b2);
        }
        public BehaviourCondition GetConditionForInputsByName(BehaviourInput b1, BehaviourInput b2, String name)
        {
            return BehaviourInputFactory.GetConditionForInputsByName(b1, b2, name);
        }
    }
}