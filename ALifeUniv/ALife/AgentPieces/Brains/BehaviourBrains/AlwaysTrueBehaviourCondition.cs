namespace ALifeUni.ALife.Brains
{
    public class AlwaysTrueBehaviourCondition : BehaviourCondition<bool>
    {
        public AlwaysTrueBehaviourCondition() : base()
        {
            Text = "ALWAYS";
            LastState = true;
        }
        public override bool EvaluateSuccess()
        {
            return true;
        }
    }
}
