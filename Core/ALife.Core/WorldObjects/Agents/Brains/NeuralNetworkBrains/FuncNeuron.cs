using System;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    public class FuncNeuron : Neuron
    {
        Func<double> GetValue;

        private double theVal;

        public FuncNeuron(String name, Func<double> getValueFunction)
            : base(name, 0.0)
        {
            GetValue = getValueFunction;
        }

        public override double Value
        {
            get
            {
                return theVal;
            }
            set
            {
                throw new Exception("Do Not Set FuncNeuron Value");
            }
        }

        public override void GatherValue()
        {
            theVal = GetValue();
        }
    }
}
