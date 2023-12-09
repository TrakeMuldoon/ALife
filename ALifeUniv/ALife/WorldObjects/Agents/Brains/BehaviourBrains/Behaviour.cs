using ALifeUni.ALife.WorldObjects.Agents.AgentActions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ALifeUni.ALife.WorldObjects.Agents.Brains.BehaviourBrains
{
    public class Behaviour
    {
        public readonly String AsEnglish;
        public bool PassedThisTurn;
        public readonly List<BehaviourCondition> Conditions = new List<BehaviourCondition>();
        public ActionPart SuccessAction;
        public Func<double> SuccessParam;
        private int waitTurns = 0;
        private string intensityString;

        static readonly Regex EnglishStringParser = new Regex("^IF (.*) THEN (.*)$");

        public Behaviour(String englishString, BehaviourCabinet cabinet, int waitTurnsMax)
        {
            AsEnglish = englishString;
            //Spec: <BEHAVIOUR> = "IF <CONDITION> THEN <RESULT>"
            //Spec: <BEHAVIOUR> = "*" = Random Behaviour
            //Spec: <CONDITON> = "<VARIABLE> <OPERATION> <CONSTANT|VARIABLE>( AND <CONDITON>)?"     -- Conditions can be chained together
            //Spec: <CONDITON> = "ALWAYS"                                                           -- Condition can also be "ALWAYS"
            //Spec: <VARIABLE> = "\\w+(\\.\\w+)+"                                                   -- Some property of the Agent which evaluates to a value
            //Spec: <OPERATION> = "\\w+"                                                            -- Some comparator which is valid to compare to values of the correct type
            //Spec: <CONSTANT> = "\\[\\w+\\]"                                                       -- Some constant of the type of the variable mentioned
            //Spec: <RESULT> = "( <WAIT>)? <ACTION> AT <CONSTANT|VARIABLE>"                         -- Waiting is optional
            //Spec: <WAIT> = "WAIT <CONSTANT> TO"                                                   -- Indicates the number of turns to wait
            //Spec: <ACTION> = "\\w+(\\.\\w+)"                                                      -- Some action of the Agent

            if(englishString.Equals("*"))
            {
                AsEnglish = CreateRandomBehaviour(cabinet, waitTurnsMax);
            }
            else
            {
                Match behaviourMatch = EnglishStringParser.Match(englishString);
                ParseConditions(behaviourMatch.Groups[1].Value, cabinet);
                ParseResults(behaviourMatch.Groups[2].Value, cabinet);
            }

            if(AsEnglish != this.GenerateString())
            {
                throw new Exception("Created Behaviour does not match string");
            }
        }

        private string CreateRandomBehaviour(BehaviourCabinet cabinet, int waitTurnsMax)
        {
            //IF <CONDITION> THEN <RESULT>

            //Generate between 1 and 3 conditions
            //TODO: There's a magic number thing going on here
            int conditionsInt = Planet.World.NumberGen.Next(1, 10);
            if(conditionsInt > 8)
            {
                Conditions.Add(GenerateRandomBehaviourCondition(cabinet));
            }
            if(conditionsInt > 6)
            {
                Conditions.Add(GenerateRandomBehaviourCondition(cabinet));
            }
            Conditions.Add(GenerateRandomBehaviourCondition(cabinet));

            //Generate a result
            //Optionally add a "wait time"
            //TODO: There's a magic number thing going on here
            int addWaitOn6 = Planet.World.NumberGen.Next(1, 6);
            if(addWaitOn6 == 6)
            {
                waitTurns = Planet.World.NumberGen.Next(1, waitTurnsMax);
            }
            //select action
            SuccessAction = cabinet.GetRandomAction();
            //generate intensity
            BehaviourInput<double> intensity = (BehaviourInput<double>)DoubleConditionFactory.GetRandomVariableOrConstant(cabinet);

            intensityString = intensity.FullName;

            SuccessParam = intensity.MyFunc;

            return GenerateString();
        }

        private static BehaviourCondition GenerateRandomBehaviourCondition(BehaviourCabinet cabinet)
        {
            BehaviourInput b1 = cabinet.GetRandomBehaviourInput();
            BehaviourCondition bc = BehaviourFactory.GetRandomBehaviorConditionFromInput(b1, cabinet);
            return bc;
        }

        private void ParseConditions(String conditionsString, BehaviourCabinet cabinet)
        {
            if(conditionsString == "ALWAYS")
            {
                Conditions.Add(new AlwaysTrueBehaviourCondition());
                return;
            }
            string[] conditions = Regex.Split(conditionsString, " AND ");
            foreach(string condition in conditions)
            {
                Conditions.Add(ParseBehaviourCondition(condition, cabinet));
            }
        }

        private BehaviourCondition ParseBehaviourCondition(string condition, BehaviourCabinet cabinet)
        {
            string[] pieces = condition.Split(' ');
            if(pieces.Length != 3)
            {
                throw new Exception("Wtf number of pieces of a behaviour condition");
            }
            BehaviourInput b1 = cabinet.GetBehaviourInputByName(pieces[0]);

            //Check if b2 starts with "[" which means a constant, then it sends it to the constant factory
            //otherwise it gets the behaviour input from the Cabinet
            BehaviourInput b2 = pieces[2].StartsWith("[") ? BehaviourFactory.GetBehaviourConstantFromString(b1, pieces[2]) : cabinet.GetBehaviourInputByName(pieces[2]);

            BehaviourCondition bc = BehaviourFactory.GetConditionForInputsByName(b1, b2, pieces[1]);
            return bc;
        }

        static readonly Regex resultParser = new Regex("^(WAIT \\[(\\d+)\\] TO )?(\\w+(\\.\\w+)) AT ([\\[\\]\\.\\w]+)$");
        private void ParseResults(string value, BehaviourCabinet cabinet)
        {
            Match resultsMatch = resultParser.Match(value);
            string waitMatch = resultsMatch.Groups[2].Value;
            string actionMatch = resultsMatch.Groups[3].Value;
            string variableValue = resultsMatch.Groups[5].Value;

            if(resultsMatch.Groups[1].Success)
            {
                waitTurns = int.Parse(waitMatch);
            }
            //else waitTurns remains zero. the default value.
            SuccessAction = cabinet.GetActionPartByFullName(actionMatch);

            //dummy double exists because you need to pass in the "type" of the constant
            BehaviourInput dummydouble = new BehaviourInput<double>(null, null);

            //Check if b2 starts with "[" which means a constant, then it sends it to the constant factory
            //otherwise it gets the behaviour input from the Cabinet
            BehaviourInput intensity = variableValue.StartsWith("[") ? BehaviourFactory.GetBehaviourConstantFromString(dummydouble, variableValue) : cabinet.GetBehaviourInputByName(variableValue);
            intensityString = variableValue;

            SuccessParam = ((BehaviourInput<double>)intensity).MyFunc;
        }

        public void EvaluateAndEnqueue(BehaviourWaitQueue bwq)
        {
            foreach(BehaviourCondition bc in Conditions)
            {
                if(!bc.EvaluateSuccess())
                {
                    PassedThisTurn = false;
                    return;
                }
            }

            PassedThisTurn = true;
            bwq.AddAction(() => SuccessAction.Intensity += SuccessParam(), waitTurns);
        }

        public bool ConditionsPassed()
        {
            foreach(BehaviourCondition bc in Conditions)
            {
                if(!bc.EvaluateSuccess())
                {
                    return false;
                }
            }
            return true;
        }

        public void AddActionToWaitQueue(BehaviourWaitQueue bwq)
        {
            bwq.AddAction(() => SuccessAction.Intensity += SuccessParam(), waitTurns);
        }

        public string GenerateString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IF ");
            sb.Append(this.Conditions[0].ToString());
            for(int i = 1; i < this.Conditions.Count; i++)
            {
                sb.Append(" AND ");
                sb.Append(this.Conditions[i].ToString());
            }
            sb.Append(" THEN ");
            if(waitTurns > 0)
            {
                sb.Append("WAIT [" + waitTurns + "] TO ");
            }
            sb.Append(this.SuccessAction.FullName);
            sb.Append(" AT ");
            sb.Append(this.intensityString);

            return sb.ToString();
        }

        public override string ToString()
        {
            return AsEnglish;
        }
    }
}
