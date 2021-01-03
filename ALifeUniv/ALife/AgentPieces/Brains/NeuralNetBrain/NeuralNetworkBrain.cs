using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains
{
    class NeuralNetworkBrain : IBrain
    {
        private Agent self;
        public double ModificationRate;
        public double MutabilityRate;
        public List<Layer> Layers = new List<Layer>();

        public NeuralNetworkBrain(Agent self, List<int> layers)
            : this(self, 0.1, 0.05, layers)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self">The parent</param>
        /// <param name="modificationRate">The rate at which dendrite weights or neuron bias' get modified in Repro</param>
        /// <param name="mutabilityRate">The maximum percentage to which they will be modified</param>
        /// <param name="layerNeuronCounts">the neuron counts for the hidden layers</param>
        public NeuralNetworkBrain(Agent self, double modificationRate, double mutabilityRate, List<int> layerNeuronCounts)
        {
            if(layerNeuronCounts.Count < 1) throw new ArgumentOutOfRangeException("Not enough layers for a Neural Network Brain");

            this.self = self;
            ModificationRate = modificationRate;
            MutabilityRate = mutabilityRate;

            //First we initialize the Top Layer to have all the inputs available.
            Layer senseLayer = CreateSenseLayer(self);
            Layers.Add(senseLayer);


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
    }
}
