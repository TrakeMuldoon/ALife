using ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces;
using ALifeUni.ALife.Inputs.SenseClusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
{
    public class Behaviour
    {
        public readonly String AsEnglish;
        public readonly List<BehaviourCondition> Conditions = new List<BehaviourCondition>();
        //public readonly Action SuccessAction;
        //public readonly Func<double> SuccessParam;
        Regex englishStringParser = new Regex("^IF (.*) THEN (.*)$");
        Regex resultParser = new Regex("^(WAIT (\\w+) )?(\\w+) INTENSITY (\\w+)$");

        public Behaviour(String englishString, BehaviourCabinet cabinet)
        {
            AsEnglish = englishString;
            //Spec: <BEHAVIOUR> = "IF <CONDITION> THEN <RESULT>"
            //Spec: <CONDITON> = "<VARIABLE> <OPERATION> <CONSTANT|VARIABLE>( AND <CONDITON>)?"     -- Conditions can be chained together
            //Spec: <VARIABLE> = "\\w+(\\.\\w+)+"                                                   -- Some property of the Agent which evaluates to a value
            //Spec: <OPERATION> = "\\w+"                                                            -- Some comparator which is valid to compare to values of the correct type
            //Spec: <CONSTANT> = "\\[\\w+\\]"                                                       -- Some constant of the type of the variable mentioned
            //Spec: <RESULT> = "( <WAIT>)? <ACTION> INTENSITY <CONSTANT|VARIABLE>"                  -- Waiting is optional
            //Spec: <WAIT> = "WAIT <CONSTANT>"                                                      -- Indicates the number of turns to wait
            //Spec: <ACTION> = "\\w+(\\.\\w+)"                                                      -- Some action of the Agent
            Match behaviourMatch = englishStringParser.Match(englishString);
            ParseConditions(behaviourMatch.Groups[1].Value, cabinet);
            ParseResults(behaviourMatch.Groups[2].Value, cabinet);
        }

        private void ParseResults(string value, BehaviourCabinet cabinet)
        {
            Match resultsMatch = resultParser.Match(value);
            string waitMatch = resultsMatch.Groups[2].Value;
            string actionMatch = resultsMatch.Groups[3].Value;
            string variableValue = resultsMatch.Groups[4].Value;
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

        //will be run once a "turn"
        public void EvaluateBehaviour()
        {
            foreach(BehaviourCondition bc in Conditions)
            {
                if(!bc.EvaluateSuccess())
                {
                    //AND is the only logic, therefore if they fail a condition, this behaviour does not evaluate
                    return;
                }
            }

            SuccessAction.Intensity += SuccessParam();
        }
    }
}
