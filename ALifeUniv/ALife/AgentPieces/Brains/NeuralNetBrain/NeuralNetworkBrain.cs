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

            //Next we implement all the middle layers
            for(int i = 0; i < layerNeuronCounts.Count; i++)
            {
                Layer newLayer = new Layer(layerNeuronCounts[i]);
                Layers.Add(newLayer);

                //This works because we know that there is always a layer above us.
                List<Neuron> aboveLayerNeurons = Layers[i].Neurons;

                for(int n = 0; n < layerNeuronCounts[i]; n++)
                {
                    //Add a neuron
                    Neuron neu = new Neuron("HN:" + (i + 1) + "." + (n + 1));
                    newLayer.Neurons.Add(neu);
                    for(int d = 0; d < aboveLayerNeurons.Count; d++)
                    {
                        neu.UpstreamDendrites.Add(new Dendrite(aboveLayerNeurons[d]));
                    }
                }
            }

            Layer actionLayer = CreateActionLayer(self, Layers[Layers.Count - 1]);
            Layers.Add(actionLayer);
        }

        private Layer CreateActionLayer(Agent self, Layer aboveLayer)
        {
            List<ActionNeuron> actionNeurons = new List<ActionNeuron>();
            foreach(ActionCluster ac in self.Actions.Values)
            {
                foreach(ActionPart ap in ac.SubActions.Values)
                {
                    ActionNeuron neu = new ActionNeuron(ap);
                    actionNeurons.Add(neu);
                    for(int d = 0; d < aboveLayer.Neurons.Count; d++)
                    {
                        neu.UpstreamDendrites.Add(new Dendrite(aboveLayer.Neurons[d]));
                    }
                }
            }
            Layer actionLayer = new Layer(actionNeurons.Count);
            actionLayer.Neurons.AddRange(actionNeurons);
            return actionLayer;
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
            senseLayer.Neurons.AddRange(senseNeurons);
            return senseLayer;
        }
    }
}
