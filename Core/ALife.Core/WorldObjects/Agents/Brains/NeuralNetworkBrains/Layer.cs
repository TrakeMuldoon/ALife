using System.Collections.Generic;
using System.Text;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }

        public Layer(int numNeurons)
        {
            Neurons = new List<Neuron>(numNeurons);
        }

        public string ExportNewBrain_Layer()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\tLAYER: NeuronCount: {Neurons.Count}");
            foreach(Neuron neuron in Neurons)
            {
                sb.AppendLine(neuron.ExportNewBrain_Neuron());
            }

            return sb.ToString();
        }
    }
}
