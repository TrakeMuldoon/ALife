using System;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    public class Dendrite
    {
        public double Weight { get; set; }
        public Neuron TargetNeuron { get; set; }
        public string TargetNeuronName { get; set; }

        public double CurrentValue
        {
            get
            {
                return TargetNeuron.Value * Weight;
            }

        }

        public Dendrite(Neuron targetNeuron)
            : this(targetNeuron, (Planet.World.NumberGen.NextDouble() * 2) - 1)
        {
        }

        public Dendrite(Neuron targetNeuron, double weight)
        {
            TargetNeuron = targetNeuron;
            TargetNeuronName = targetNeuron.Name;
            if(weight < -1.0 || weight > 1.0)
            {
                throw new ArgumentOutOfRangeException("Weight must be between -1 and 1");
            }
            Weight = weight;
        }
    }
}
