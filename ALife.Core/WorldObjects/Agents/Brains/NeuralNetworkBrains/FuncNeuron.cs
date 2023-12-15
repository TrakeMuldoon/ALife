using System;
using ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    class FuncNeuron : Neuron
    {
        Func<double> GetValue;

        private double theVal;

        public FuncNeuron(String name, Func<double> getValueFunction)
            : base(name)
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
