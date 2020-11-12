using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces;
using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces.TypedClasses;
using ALifeUni.ALife.Inputs.SenseClusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Services.Cortana;
using Windows.UI.WebUI;

namespace ALifeUni.ALife.Brains.BehaviourBrains
{
    public class Behaviour
    {
        public readonly String AsEnglish;
        public readonly List<BehaviourCondition> Conditions = new List<BehaviourCondition>();
        public Action SuccessAction;
        public Func<double> SuccessParam;
        private int waitTurns = 0;
        private string intensityString;
        Regex englishStringParser = new Regex("^IF (.*) THEN (.*)$");

        public Behaviour(String englishString, BehaviourCabinet cabinet)
        {
            AsEnglish = englishString;
            //Spec: <BEHAVIOUR> = "IF <CONDITION> THEN <RESULT>"
            //Spec: <CONDITON> = "<VARIABLE> <OPERATION> <CONSTANT|VARIABLE>( AND <CONDITON>)?"     -- Conditions can be chained together
            //Spec: <VARIABLE> = "\\w+(\\.\\w+)+"                                                   -- Some property of the Agent which evaluates to a value
            //Spec: <OPERATION> = "\\w+"                                                            -- Some comparator which is valid to compare to values of the correct type
            //Spec: <CONSTANT> = "\\[\\w+\\]"                                                       -- Some constant of the type of the variable mentioned
            //Spec: <RESULT> = "( <WAIT>)? <ACTION> AT <CONSTANT|VARIABLE>"                         -- Waiting is optional
            //Spec: <WAIT> = "WAIT <CONSTANT> TO"                                                   -- Indicates the number of turns to wait
            //Spec: <ACTION> = "\\w+(\\.\\w+)"                                                      -- Some action of the Agent
            //Spec: "*" = Random Behaviour
            if(englishString.Equals("*"))
            {
                AsEnglish = CreateRandomBehaviour(cabinet);
            }
            else
            {
                Match behaviourMatch = englishStringParser.Match(englishString);
                ParseConditions(behaviourMatch.Groups[1].Value, cabinet);
                ParseResults(behaviourMatch.Groups[2].Value, cabinet);
            }
        }

        private string CreateRandomBehaviour(BehaviourCabinet cabinet)
        {
            //IF <CONDITION> THEN <RESULT>

            //Generate between 1 and 3 conditions
            //TODO: There's a magic number thing going on here
            int conditionsInt = Planet.World.NumberGen.Next(1, 6);
            if(conditionsInt > 5)
            {
                Conditions.Add(GenerateRandomBehaviourCondition(cabinet));
            }
            if(conditionsInt > 3)
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
                waitTurns = Planet.World.NumberGen.Next(0, Settings.BehaviourWaitMax);
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
            BehaviourInput b2 = pieces[2].StartsWith("[") ? BehaviourFactory.GetBehaviourConstantFromString(b1, pieces[2]) :  cabinet.GetBehaviourInputByName(pieces[2]);

            BehaviourCondition bc = BehaviourFactory.GetConditionForInputsByName(b1, b2, pieces[1]);
            return bc;
        }

        Regex resultParser = new Regex("^(WAIT \\[(\\d+)\\] TO )?(\\w+) AT ([\\[\\]\\.\\w]+)$");
        private void ParseResults(string value, BehaviourCabinet cabinet)
        {
            Match resultsMatch = resultParser.Match(value);
            string waitMatch = resultsMatch.Groups[2].Value;
            string actionMatch = resultsMatch.Groups[3].Value;
            string variableValue = resultsMatch.Groups[4].Value;

            if(resultsMatch.Groups[1].Success)
            {
                waitTurns = int.Parse(waitMatch);
            }
            //else waitTurns remains zero. the default value.
            SuccessAction = cabinet.GetActionByName(actionMatch);

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
                    return;
                }
            }

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
            bwq.AddAction(() => SuccessAction.Intensity += SuccessParam(),waitTurns);
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
            sb.Append(this.SuccessAction.Name);
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
