using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.NeuralNetBrain
{
    class NeuralNetworkBrain : IBrain
    {
        private Agent self;
        public double ModificationRate;
        public double MutabilityRate;
        public List<Layer> Layers = new List<Layer>();

        public NeuralNetworkBrain(Agent self, List<int> layers)
            : this(self, 0.1, 0.5, layers)
        { 
        }

        public NeuralNetworkBrain(Agent self, double modificationRate, double mutabilityRate, List<int> layerNeuronCounts)
        {
            if(layerNeuronCounts.Count < 1) throw new ArgumentOutOfRangeException("Not enough layers for a Neural Network Brain");

            this.self = self;
            ModificationRate = modificationRate;
            MutabilityRate = mutabilityRate;

            //First we initialize the Top Layer to have all the inputs available.
            Layer senseLayer = CreateSenseLayer(self);
            Layers.Add(senseLayer);

            //Add the "number of outputs" as an extra layer (the bottom one)
            layerNeuronCounts.Add(self.Actions.Count);

            for(int i = 0; i < layerNeuronCounts.Count; i++)
            {
                //Next we implement all the middle layers
                Layer newLayer = new Layer(layerNeuronCounts[i]);
                Layers.Add(newLayer);

                //This works because we know that there is always a layer above us.
                int aboveLayerCount = Layers[i].NeuronCount;

                for(int n = 0; n < layerNeuronCounts[i]; n++)
                {
                    //Add a neuron
                    Neuron neu = new Neuron("HN:" + (i + 1) + "." + (n + 1));
                    newLayer.Neurons.Add(neu);
                    for(int d = 0; d < aboveLayerCount; d++)
                    {
                        neu.Dendrites.Add(new Dendrite());
                    }
                }
            }

            //Next attach the ActionClusters to the outputs we created for them.
            throw new NotImplementedException("Have not implemented converting 'Actions' to Neurons");
        }

        private static Layer CreateSenseLayer(Agent self)
        {
            List<FuncNeuron> senseNeurons = new List<FuncNeuron>();
            foreach(SenseCluster sc in self.Senses)
            {
                foreach(SenseInput si in sc.SubInputs)
                {
                    List<FuncNeuron> siNeurons = FuncNeuronFactory.GenerateFuncNeuronsForSenseInput(si);
                    senseNeurons.AddRange(siNeurons);
                }
            }

            Layer senseLayer = new Layer(senseNeurons.Count);
            return senseLayer;
        }

        public void ExecuteTurn()
        {
            //Detect all the things.
            foreach(SenseCluster sc in self.Senses)
            {
                sc.Detect();
            }
            //Set the values within the neurons
            foreach(FuncNeuron fn in Layers[0].Neurons)
            {
                fn.RefreshValue();
            }
            //Plinko the values through the NN
            PermeateValues();
            //Then set the values into the ActionCluster
            throw new NotImplementedException("Have not yet written how to 'set the values into the ActionClusters'");
        }

        public void PermeateValues()
        {
            //We start with the "second" layer, becasue the top layer's value is set by the "Detecting" from the senses

            for(int i = 1; i < Layers.Count; i++)
            {
                Layer layer = Layers[i];
                foreach(Neuron neuron in layer.Neurons)
                {
                    neuron.Value = 0;
                    //Read the values of the layer above me;
                    List<Neuron> aboveNeurons = Layers[i - 1].Neurons;
                    for(int np = 0; np < aboveNeurons.Count; np++)
                    {
                        neuron.Value += (aboveNeurons[np].Value * neuron.Dendrites[np].Weight);
                    }

                    neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                }
            }
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        //public bool Train(List<double> input, List<double> output)
        //{
        //    Run(input);

        //    for(int i = 0; i < Layers[Layers.Count - 1].Neurons.Count; i++)
        //    {
        //        Neuron neuron = Layers[Layers.Count - 1].Neurons[i];

        //        neuron.Delta = neuron.Value * (1 - neuron.Value) * (output[i] - neuron.Value);

        //        for(int j = Layers.Count - 2; j > 2; j--)
        //        {
        //            for(int k = 0; k < Layers[j].Neurons.Count; k++)
        //            {
        //                Neuron n = Layers[j].Neurons[k];

        //                n.Delta = n.Value *
        //                          (1 - n.Value) *
        //                          Layers[j + 1].Neurons[i].Dendrites[k].Weight *
        //                          Layers[j + 1].Neurons[i].Delta;
        //            }
        //        }
        //    }

        //    for(int i = Layers.Count - 1; i > 1; i--)
        //    {
        //        for(int j = 0; j < Layers[i].Neurons.Count; j++)
        //        {
        //            Neuron n = Layers[i].Neurons[j];
        //            n.Bias += LearningRate * n.Delta;

        //            for(int k = 0; k < n.Dendrites.Count; k++)
        //                n.Dendrites[k].Weight += (this.LearningRate
        //                                          * this.Layers[i - 1].Neurons[k].Value
        //                                          * n.Delta);
        //        }
        //    }

        //    return true;
        //}


        public IBrain Clone(Agent self)
        {
            throw new NotImplementedException();
        }

        public IBrain Reproduce(Agent self)
        {
            throw new NotImplementedException();
        }
    }
}
