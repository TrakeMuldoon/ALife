using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    internal class NeuralNetworkBrainFactory
    {
        public static List<Layer> CreateNeuralNetworkForAgent(Agent self, List<int> hiddenLayerNeuronCounts)
        {
            List<Layer> layers = new List<Layer>();
            
            //Create Sense Layer
            layers.Add(CreateSenseLayer(self));

            //Create Hidden Layer Neurons
            for(int layerNum = 0; layerNum < hiddenLayerNeuronCounts.Count; ++layerNum)
            {
                Layer newLayer = CreateHiddenLayer(hiddenLayerNeuronCounts[layerNum], layerNum + 1);
                Dictionary<string, double> layerBiases = GenerateRandDictForLayer(newLayer);
                ApplyBiasesToLayer(newLayer, layerBiases);
                layers.Add(newLayer);
            }

            //Create Action Layer
            Layer actionLayer = CreateActionLayer(self);
            Dictionary<string, double> actionBiases = GenerateRandDictForLayer(actionLayer);
            ApplyBiasesToLayer(actionLayer, actionBiases);
            layers.Add(actionLayer);

            //Create Dendrites
            for(int i = 1; i < layers.Count; ++i)
            {
                CreateRandomDendritesForLayer(layers[i], layers[i - 1]);
            }

            return layers;
        }

        private static void CreateRandomDendritesForLayer(Layer layer, Layer parentLayer)
        {
            foreach(Neuron n in layer.Neurons)
            {
                foreach(Neuron parentNeuron in parentLayer.Neurons)
                {
                    Dendrite den = new Dendrite(parentNeuron);
                    n.UpstreamDendrites.Add(den);
                }
            }
        }

        public NeuralNetworkBrain CreateClonedBrain(Agent self, NeuralNetworkBrain existingBrain)
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(CreateSenseLayer(self));

            //Verify

            //Loop
            //Create Hidden layers
            //Verify and SetBias

            return null;
        }

        public NeuralNetworkBrain CreateEvolvedBrain(Agent self, NeuralNetworkBrain existingBrain)
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(CreateSenseLayer(self));

            //Verify

            //Loop
            //Create Hidden layers
            //Verify and SetBias With Evo

            return null;
        }

        public NeuralNetworkBrain CreateFromBrainSpec(Agent self, NeuralNetworkBrainImport brainSpec)
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(CreateSenseLayer(self));

            //Verify

            //Loop
            //Create Hidden layers
            //Verify and SetBias

            return null;
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

        private static Layer CreateHiddenLayer(int numNeurons, int layerNum)
        {
            Layer hiddenLayer = new Layer(numNeurons);
            for(int i = 0; i < numNeurons; ++i)
            {
                Neuron neu = new Neuron($"HN:{i + 1}.{layerNum + 1}", 0);
                hiddenLayer.Neurons.Add(neu);
            }
            return hiddenLayer;
        }

        private static Layer CreateActionLayer(Agent self)
        {
            List<Neuron> actionNeurons = new List<Neuron>();
            foreach(ActionCluster ac in self.Actions.Values)
            {
                foreach(ActionPart ap in ac.SubActions.Values)
                {
                    ActionNeuron neu = new ActionNeuron(ap);
                    actionNeurons.Add(neu);
                }
            }
            Layer actionLayer = new Layer(actionNeurons.Count);
            actionLayer.Neurons.AddRange(actionNeurons);
            return actionLayer;
        }


        private static void ApplyBiasesToLayer(Layer newLayer, Dictionary<string, double> layerBiases)
        {
            foreach(Neuron n in newLayer.Neurons)
            {
                if(!layerBiases.ContainsKey(n.Name))
                {
                    throw new Exception($"Missing key from biases:{n.Name}");
                }
                n.Bias = layerBiases[n.Name];
                layerBiases.Remove(n.Name);
            }
            if(layerBiases.Count > 0)
            {
                throw new Exception("Layer Biases input does not match newLayer neurons.");
            }
            return;
        }

        private static Dictionary<string, double> GenerateRandDictForLayer(Layer newLayer)
        {
            Dictionary<string, double> neuronBiases = new Dictionary<string, double>();
            foreach(Neuron n in newLayer.Neurons)
            {
                neuronBiases.Add(n.Name, (Planet.World.NumberGen.NextDouble() * 2) - 1);
            }
            return neuronBiases;
        }

        //public NeuralNetworkBrain(Agent self, double modificationRate, double mutabilityRate, List<Layer> allLayers, Layer actionLayer)

        //1        public NeuralNetworkBrain(Agent self, double modificationRate, double mutabilityRate, List<int> layerNeuronCounts)
        //2        private NeuralNetworkBrain(Agent self, NeuralNetworkBrain templateBrain, bool exactCopy) TRUE
        //3        private NeuralNetworkBrain(Agent self, NeuralNetworkBrain templateBrain, bool exactCopy) FALSE
        //4        private NeuralNetworkBrain(Agent self, InputString, bool exactCopy)

        //Set Mod/Mut
        // Create SenseLayerFromSenses(self)
        //  if 234 validate Names

        //Foreach HiddenLayer
        //Create HiddenLayer(Self)
        // if 2 build hiddenLayerBiasDictFromTemplate
        // if 3 build hiddenLayerBiasDictFromTemplate with Evo
        // if 4 build hiddenLayerBiasDictFromString
        // if 234 SetHiddenLayerBias

        //CreateActionLayer(self)
        // if 2 build hiddenLayerBiasDictFromTemplate
        // if 3 build hiddenLayerBiasDictFromTemplate with Evo
        // if 4 build hiddenLayerBiasDictFromString
        // if 234 SetHiddenLayerBias

        // CreateAllDendrites(self, Dictionary<string:nametoname, double weight>)
    }
}
