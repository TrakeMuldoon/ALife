using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }

        public Layer(int numNeurons)
        {
            Neurons = new List<Neuron>(numNeurons);
        }
    }
}
