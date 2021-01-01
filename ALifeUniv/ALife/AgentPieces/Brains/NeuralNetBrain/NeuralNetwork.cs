using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.NeuralNetBrain
{
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }
        public double LearningRate { get; set; }
        public int LayerCount
        {
            get
            {
                return Layers.Count;
            }
        }

        public NeuralNetwork(double learningRate, int[] layers)
        {
            if(layers.Length < 2) return;

            LearningRate = learningRate;
            Layers = new List<Layer>();

            for(int j = 0; j < layers.Length; j++)
            {
                Layer layer = new Layer(layers[j]);
                Layers.Add(layer);

                for(int n = 0; n < layers[j]; n++)
                    layer.Neurons.Add(new Neuron());

                foreach(Neuron nn in layer.Neurons)
                {
                    if(j == 0)
                    {
                        nn.Bias = 0;
                    }
                    else
                    {
                        for(int d = 0; d < layers[j - 1]; d++)
                        {
                            nn.Dendrites.Add(new Dendrite());
                        }
                    }
                }
            }
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public double[] Run(List<double> input)
        {
            if(input.Count != this.Layers[0].NeuronCount) return null;

            for(int j = 0; j < Layers.Count; j++)
            {
                Layer layer = Layers[j];

                for(int n = 0; n < layer.Neurons.Count; n++)
                {
                    Neuron neuron = layer.Neurons[n];

                    if(j == 0)
                    {
                        neuron.Value = input[n];
                    }
                    else
                    {
                        neuron.Value = 0;
                        for(int np = 0; np < this.Layers[j - 1].Neurons.Count; np++)
                        {
                            neuron.Value += this.Layers[j - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;
                        }

                        neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                    }
                }
            }

            Layer last = this.Layers[this.Layers.Count - 1];
            int numOutput = last.Neurons.Count;
            double[] output = new double[numOutput];
            for(int i = 0; i < last.Neurons.Count; i++)
            {
                output[i] = last.Neurons[i].Value;
            }

            return output;
        }

        public bool Train(List<double> input, List<double> output)
        {
            if((input.Count != this.Layers[0].Neurons.Count)
                || (output.Count != this.Layers[this.Layers.Count - 1].Neurons.Count))
            {
                return false;
            }

            Run(input);

            for(int i = 0; i < Layers[Layers.Count - 1].Neurons.Count; i++)
            {
                Neuron neuron = Layers[Layers.Count - 1].Neurons[i];

                neuron.Delta = neuron.Value * (1 - neuron.Value) * (output[i] - neuron.Value);

                for(int j = Layers.Count - 2; j > 2; j--)
                {
                    for(int k = 0; k < Layers[j].Neurons.Count; k++)
                    {
                        Neuron n = Layers[j].Neurons[k];

                        n.Delta = n.Value *
                                  (1 - n.Value) *
                                  Layers[j + 1].Neurons[i].Dendrites[k].Weight *
                                  Layers[j + 1].Neurons[i].Delta;
                    }
                }
            }

            for(int i = Layers.Count - 1; i > 1; i--)
            {
                for(int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    Neuron n = Layers[i].Neurons[j];
                    n.Bias += LearningRate * n.Delta;

                    for(int k = 0; k < n.Dendrites.Count; k++)
                        n.Dendrites[k].Weight += (this.LearningRate
                                                  * this.Layers[i - 1].Neurons[k].Value
                                                  * n.Delta);
                }
            }

            return true;
        }
    }
}
