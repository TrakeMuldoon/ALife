using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    internal class NeuralNetworkBrainFactory
    {
        public static List<Layer> CreateRandomNeuralNetwork(Agent self, List<int> hiddenLayerNeuronCounts)
        {
            List<Layer> layers = new List<Layer>();

            //Create Sense Layer
            layers.Add(CreateSenseLayer(self));

            //Create Hidden Layer Neurons
            for(int layerNum = 0; layerNum < hiddenLayerNeuronCounts.Count; ++layerNum)
            {
                Layer newLayer = CreateHiddenLayer(hiddenLayerNeuronCounts[layerNum], layerNum + 1);
                Dictionary<string, double> layerBiases = GenerateRandomNeuronBiasesDictionary(newLayer);
                ApplyBiasesToLayer(newLayer, layerBiases);
                layers.Add(newLayer);
            }

            //Create Action Layer
            Layer actionLayer = CreateActionLayer(self);
            Dictionary<string, double> actionBiases = GenerateRandomNeuronBiasesDictionary(actionLayer);
            ApplyBiasesToLayer(actionLayer, actionBiases);
            layers.Add(actionLayer);

            //Create Dendrites
            for(int i = 1; i < layers.Count; ++i)
            {
                CreateRandomDendritesForLayer(layers[i], layers[i - 1]);
            }

            return layers;
        }

        public static List<Layer> CreateClonedNeuralNetwork(Agent self, NeuralNetworkBrain existingBrain)
        {
            List<Layer> layers = new List<Layer>();

            //Create Sense Layer
            Layer senseLayer = CreateSenseLayer(self);
            VerifyLayerNames(senseLayer, existingBrain.Layers[0].Neurons.Select(neuron => neuron.Name).ToArray());
            layers.Add(senseLayer);

            int hiddenLayerCount = existingBrain.Layers.Count - 2; //We skip the top layer (Senses) and bottom layer (Actions)

            //Create Hidden Layer Neurons
            for(int layerNum = 0; layerNum < hiddenLayerCount; ++layerNum)
            {
                Layer oldLayer = existingBrain.Layers[layerNum + 1];
                Layer newLayer = CreateHiddenLayer(oldLayer.Neurons.Count, layerNum + 1);
                Dictionary<string, double> layerBiases = GenerateNeuronBiasesDictionaryFromLayer(oldLayer);
                ApplyBiasesToLayer(newLayer, layerBiases);
                layers.Add(newLayer);
            }

            //Create Action Layer
            Layer oldActionLayer = existingBrain.Layers[existingBrain.Layers.Count - 1];

            Layer actionLayer = CreateActionLayer(self);
            Dictionary<string, double> actionBiases = GenerateNeuronBiasesDictionaryFromLayer(oldActionLayer);
            ApplyBiasesToLayer(actionLayer, actionBiases);
            layers.Add(actionLayer);

            //Create Dendrites
            for(int i = 1; i < layers.Count; ++i)
            {
                CreateDendritesFromLayerTemplate(layers, existingBrain.Layers, i);
            }

            return layers;
        }

        public static List<Layer> CreateEvolvedNeuralNetwork(Agent self, NeuralNetworkBrain existingBrain)
        {
            List<Layer> layers = new List<Layer>();

            //Create Sense Layer
            Layer senseLayer = CreateSenseLayer(self);
            VerifyLayerNames(senseLayer, existingBrain.Layers[0].Neurons.Select(neuron => neuron.Name).ToArray());
            layers.Add(senseLayer);

            int hiddenLayerCount = existingBrain.Layers.Count - 2; //We skip the top layer (Senses) and bottom layer (Actions)

            //Create Hidden Layer Neurons
            for(int layerNum = 0; layerNum < hiddenLayerCount; ++layerNum)
            {
                Layer oldLayer = existingBrain.Layers[layerNum + 1];
                Layer newLayer = CreateHiddenLayer(oldLayer.Neurons.Count, layerNum + 1);
                Dictionary<string, double> layerBiases = GenerateEvolvedNeuronBiasesDictionaryFromLayer(oldLayer, existingBrain);
                ApplyBiasesToLayer(newLayer, layerBiases);
                layers.Add(newLayer);
            }

            //Create Action Layer
            Layer oldActionLayer = existingBrain.Layers[existingBrain.Layers.Count - 1];

            Layer actionLayer = CreateActionLayer(self);
            Dictionary<string, double> actionBiases = GenerateEvolvedNeuronBiasesDictionaryFromLayer(oldActionLayer, existingBrain);
            ApplyBiasesToLayer(actionLayer, actionBiases);
            layers.Add(actionLayer);

            //Create Dendrites
            for(int i = 1; i < layers.Count; ++i)
            {
                CreateEvolvedDendritesFromLayerTemplate(layers, existingBrain, i);
            }

            return layers;
        }

        public static List<Layer> CreateBrainSpecNeuralNetwork(Agent self, NeuralNetworkBrainImport brainSpec)
        {
            List<Layer> layers = new List<Layer>();

            //Create Sense Layer
            Layer senseLayer = CreateSenseLayer(self);
            VerifyLayerNames(senseLayer, brainSpec.NeuronNames[0]);
            layers.Add(senseLayer);

            int hiddenLayerCount = brainSpec.LayerCount - 2; //We skip the top layer (Senses) and bottom layer (Actions)

            //Create Hidden Layer Neurons
            for(int layerNum = 0; layerNum < hiddenLayerCount; ++layerNum)
            {
                int specNum = layerNum + 1;
                Layer newLayer = CreateHiddenLayer(brainSpec.NeuronCounts[specNum], specNum);
                Dictionary<string, double> layerBiases = GenerateNeuronBiasesFromBrainSpec(brainSpec.NeuronNames[specNum], brainSpec.NeuronBiases[specNum]);
                ApplyBiasesToLayer(newLayer, layerBiases);
                layers.Add(newLayer);
            }

            //Create Action Layer

            int aSpecNum = brainSpec.LayerCount - 1;
            Layer actionLayer = CreateActionLayer(self);
            Dictionary<string, double> actionBiases = GenerateNeuronBiasesFromBrainSpec(brainSpec.NeuronNames[aSpecNum], brainSpec.NeuronBiases[aSpecNum]);
            ApplyBiasesToLayer(actionLayer, actionBiases);
            layers.Add(actionLayer);

            //Create Dendrites
            for(int i = 1; i < layers.Count; ++i)
            {
                CreateDendritesFromBrainSpec(layers, brainSpec, i);
            }

            return layers;
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
                Neuron neu = new Neuron($"HN:{layerNum}.{i + 1}", 0);
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

        private static void CreateDendritesFromBrainSpec(List<Layer> newLayers, NeuralNetworkBrainImport brainSpec, int layerNum)
        {
            Layer parent = newLayers[layerNum - 1];
            Layer current = newLayers[layerNum];

            string[] specParentNames = brainSpec.NeuronNames[layerNum - 1];
            string[] specNeuronNames = brainSpec.NeuronNames[layerNum];
            double[][] dendritesForLayer = brainSpec.DendriteWeights[layerNum];

            Dictionary<string, Neuron> parentNameToNeuron = PopulateNameToNeuron(parent.Neurons);
            //Dictionary<string, Neuron> templateNameToNeuron = PopulateNameToNeuron(templateLayer.Neurons);
            Dictionary<string, Dictionary<string, double>> nameToTargetToWeight = CreateNeuronDenWeightDictionary(specNeuronNames, specParentNames, dendritesForLayer);

            foreach(Neuron n in current.Neurons)
            {
                if(!nameToTargetToWeight.ContainsKey(n.Name))
                {
                    throw new Exception($"Neuron {n.Name} missing from spec. Should have been caught earlier");
                }
                Dictionary<string, double> templateDenWeights = nameToTargetToWeight[n.Name];
                foreach(string targetName in templateDenWeights.Keys)
                {
                    if(!parentNameToNeuron.ContainsKey(targetName))
                    {
                        throw new Exception($"Neuron {n.Name} missing from parent. Should have been caught earlier");
                    }
                    Dendrite newDendrite = new Dendrite(parentNameToNeuron[targetName], templateDenWeights[targetName]);
                    n.UpstreamDendrites.Add(newDendrite);
                }
            }
        }

        private static Dictionary<string, Dictionary<string, double>> CreateNeuronDenWeightDictionary(string[] specNeuronNames, string[] specParentNames, double[][] dendritesForLayer)
        {
            Dictionary<string, Dictionary<string, double>> output = new Dictionary<string, Dictionary<string, double>>();
            for(int i = 0; i < specNeuronNames.Length; ++i)
            {
                Dictionary<string, double> denWeightsDict = new Dictionary<string, double>();
                output.Add(specNeuronNames[i], denWeightsDict);
                double[] weightsArray = dendritesForLayer[i];
                for(int j = 0; j < specParentNames.Length; ++j)
                {
                    denWeightsDict.Add(specParentNames[j], weightsArray[j]);
                }
            }
            return output;
        }

        private static Dictionary<string, double> GenerateRandomNeuronBiasesDictionary(Layer newLayer)
        {
            Dictionary<string, double> neuronBiases = new Dictionary<string, double>();
            foreach(Neuron n in newLayer.Neurons)
            {
                neuronBiases.Add(n.Name, (Planet.World.NumberGen.NextDouble() * 2) - 1);
            }
            return neuronBiases;
        }

        private static Dictionary<string, double> GenerateNeuronBiasesDictionaryFromLayer(Layer oldLayer)
        {
            Dictionary<string, double> neuronBiases = new Dictionary<string, double>();
            foreach(Neuron n in oldLayer.Neurons)
            {
                neuronBiases.Add(n.Name, n.Bias);
            }
            return neuronBiases;
        }

        //TODO: Dupe of GenerateNeuronBiasesDictionaryFromLayer
        private static Dictionary<string, double> GenerateEvolvedNeuronBiasesDictionaryFromLayer(Layer oldLayer, NeuralNetworkBrain oldBrain)
        {
            Dictionary<string, double> neuronBiases = new Dictionary<string, double>();
            foreach(Neuron n in oldLayer.Neurons)
            {
                neuronBiases.Add(n.Name, EvolveBetweenNegOneAndOne(n.Bias, oldBrain)); //Line diff
            }
            return neuronBiases;
        }

        private static double EvolveBetweenNegOneAndOne(double original, NeuralNetworkBrain oldBrain)
        {
            double val = original;
            double shouldMod = Planet.World.NumberGen.NextDouble();
            if(shouldMod < oldBrain.ModificationRate)
            {
                double rawMod = Planet.World.NumberGen.NextDouble();
                double modification = ((rawMod * 2) - 1) * oldBrain.MutabilityRate;
                val += modification;
                val = ExtraMath.Clamp(val, -1, 1);
            }
            return val;
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

        //TODO: DUPE OF CreateDendritesFromLayerTemplate
        private static void CreateEvolvedDendritesFromLayerTemplate(List<Layer> newLayers, NeuralNetworkBrain templateBrain, int layerNum)
        {
            Layer templateLayer = templateBrain.Layers[layerNum]; //Line Diff

            Layer parent = newLayers[layerNum - 1];
            Layer current = newLayers[layerNum];

            Dictionary<string, Neuron> parentNameToNeuron = PopulateNameToNeuron(parent.Neurons);
            Dictionary<string, Neuron> templateNameToNeuron = PopulateNameToNeuron(templateLayer.Neurons);

            foreach(Neuron n in current.Neurons)
            {
                if(!templateNameToNeuron.ContainsKey(n.Name))
                {
                    throw new Exception($"Neuron {n.Name} missing from template. Should have been caught earlier");
                }
                Neuron templateNeuron = templateNameToNeuron[n.Name];
                foreach(Dendrite templateDendrite in templateNeuron.UpstreamDendrites)
                {
                    if(!parentNameToNeuron.ContainsKey(templateDendrite.TargetNeuronName))
                    {
                        throw new Exception($"Neuron {n.Name} missing from parent. Should have been caught earlier");
                    }
                    Dendrite newDendrite = new Dendrite(parentNameToNeuron[templateDendrite.TargetNeuronName]
                                                        , EvolveBetweenNegOneAndOne(templateDendrite.Weight, templateBrain)); //Line Diff
                    n.UpstreamDendrites.Add(newDendrite);
                }
            }
        }

        private static void CreateDendritesFromLayerTemplate(List<Layer> newLayers, List<Layer> templateLayers, int layerNum)
        {
            Layer templateLayer = templateLayers[layerNum];

            Layer parent = newLayers[layerNum - 1];
            Layer current = newLayers[layerNum];

            Dictionary<string, Neuron> parentNameToNeuron = PopulateNameToNeuron(parent.Neurons);
            Dictionary<string, Neuron> templateNameToNeuron = PopulateNameToNeuron(templateLayer.Neurons);

            foreach(Neuron n in current.Neurons)
            {
                if(!templateNameToNeuron.ContainsKey(n.Name))
                {
                    throw new Exception($"Neuron {n.Name} missing from template. Should have been caught earlier");
                }
                Neuron templateNeuron = templateNameToNeuron[n.Name];
                foreach(Dendrite templateDendrite in templateNeuron.UpstreamDendrites)
                {
                    if(!parentNameToNeuron.ContainsKey(templateDendrite.TargetNeuronName))
                    {
                        throw new Exception($"Neuron {n.Name} missing from parent. Should have been caught earlier");
                    }
                    Dendrite newDendrite = new Dendrite(parentNameToNeuron[templateDendrite.TargetNeuronName]
                                                        , templateDendrite.Weight);
                    n.UpstreamDendrites.Add(newDendrite);
                }
            }
        }

        private static Dictionary<string, Neuron> PopulateNameToNeuron(List<Neuron> neurons)
        {
            Dictionary<string, Neuron> dict = new Dictionary<string, Neuron>();
            foreach(Neuron n in neurons)
            {
                dict.Add(n.Name, n);
            }
            return dict;
        }

        private static Dictionary<string, double> GenerateNeuronBiasesFromBrainSpec(string[] names, double[] biases)
        {
            Dictionary<string, double> neuronBiases = new Dictionary<string, double>();
            for(int i = 0; i < names.Length; ++i)
            {
                neuronBiases.Add(names[i], biases[i]);
            }
            return neuronBiases;
        }

        private static void VerifyLayerNames(Layer senseLayer, string[] names)
        {
            HashSet<string> layerNames = new HashSet<string>();
            foreach(string s in names)
            {
                layerNames.Add(s);
            }

            foreach(Neuron n in senseLayer.Neurons)
            {
                if(!layerNames.Contains(n.Name))
                {
                    throw new Exception($"Cannot evolve or clone an agent with a different number of senses. Missing {n.Name}");
                }
                layerNames.Remove(n.Name);
            }
            if(layerNames.Count > 0)
            {
                throw new Exception($"Cannot evolve or clone an agent with a different number of senses. Too Many.");
            }
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
