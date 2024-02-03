﻿using System.Collections.Generic;
using System.ComponentModel;
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

        public string ExportNewBrain_Layer(Dictionary<string, int> neuronNameToId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"LAYER:{Neurons.Count}");

            if(neuronNameToId != null)
            {
                string[] parentNameArray = new string[neuronNameToId.Count];
                foreach(string key in neuronNameToId.Keys)
                {
                    parentNameArray[neuronNameToId[key]] = key;
                }
                sb.AppendLine($"PN:[{string.Join(",", parentNameArray)}]");
            }

            string[] currentNameArray = new string[Neurons.Count];
            double[] currentBiases = new double[Neurons.Count];
            for(int i = 0; i < Neurons.Count; ++i)
            {
                Neuron n = Neurons[i];
                currentNameArray[i] = n.Name;
                currentBiases[i] = n.Bias;
            }

            sb.AppendLine($"NN:[{string.Join(",", currentNameArray)}]");
            sb.AppendLine($"NB:[{string.Join(",", currentBiases)}]");

            if(neuronNameToId == null)
            {
                return sb.ToString();
            }

            foreach(Neuron neuron in Neurons)
            {
                sb.Append(neuron.ExportNewBrain_Neuron(neuronNameToId));
            }

            return sb.ToString();
        }
    }
}
