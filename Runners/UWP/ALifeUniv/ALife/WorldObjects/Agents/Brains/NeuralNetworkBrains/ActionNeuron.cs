using ALifeUni.ALife.WorldObjects.Agents.AgentActions;

namespace ALifeUni.ALife.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    public class ActionNeuron : Neuron
    {
        public ActionNeuron(ActionPart actionPart) : base(actionPart.Name)
        {
            Activity = actionPart;
        }
        public ActionNeuron(ActionPart actionPart, double weight) : base(actionPart.Name, weight)
        {
            Activity = actionPart;
        }

        public readonly ActionPart Activity;

        public void ApplyValue()
        {
            Activity.Intensity = Value;
        }
    }
}
