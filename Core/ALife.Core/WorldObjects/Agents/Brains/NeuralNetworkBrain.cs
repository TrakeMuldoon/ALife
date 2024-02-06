using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace ALife.Core.WorldObjects.Agents.Brains
{
    public class NeuralNetworkBrain : IBrain
    {
        private Agent self;
        public double ModificationRate;
        public double MutabilityRate;
        public List<Layer> Layers = new List<Layer>();
        private Layer actions;

        public NeuralNetworkBrain(Agent self, List<int> layers)
            : this(self, 0.70, 0.005, layers)
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

                //This pulls the layer above us. Layer 0 is the sense layer. Layer 1 is the first created hidden layer.
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
            actions = actionLayer;
        }

        public NeuralNetworkBrain(Agent self, string inputString)
        {
            this.self = self;
            ImportFromString(inputString);
        }

        private NeuralNetworkBrain(Agent self, NeuralNetworkBrain templateBrain, bool exactCopy)
        {
            this.self = self;
            ModificationRate = templateBrain.ModificationRate;
            MutabilityRate = templateBrain.MutabilityRate;

            Layer senseLayer = CreateSenseLayer(self);
            if(senseLayer.Neurons.Count != templateBrain.Layers[0].Neurons.Count)
            {
                throw new NotImplementedException("Cannot clone or reproduce brain with different number of inputs");
            }
            Layers.Add(senseLayer);

            //This is the number of hidden layers we need to make
            int hiddenLayerCount = templateBrain.Layers.Count - 2;
            for(int i = 0; i < hiddenLayerCount; i++)
            {
                List<Neuron> templateCurrLayerNeurons = templateBrain.Layers[i + 1].Neurons;
                int currentLayerNeuronCount = templateCurrLayerNeurons.Count;
                Layer newLayer = new Layer(currentLayerNeuronCount);
                Layers.Add(newLayer);

                //This works because we know that there is always a layer above us.
                List<Neuron> aboveLayerNeurons = Layers[i].Neurons;

                for(int n = 0; n < currentLayerNeuronCount; n++)
                {
                    Neuron templateNeuron = templateCurrLayerNeurons[n];
                    double neuBias = templateNeuron.Bias;
                    if(!exactCopy)
                    {
                        neuBias = ModifyBetweenNegOneAndOne(neuBias);
                    }

                    //Add a neuron
                    Neuron newNeuron = new Neuron("HN:" + (i + 1) + "." + (n + 1), neuBias);
                    newLayer.Neurons.Add(newNeuron);
                    CreateDendrites(aboveLayerNeurons, templateNeuron, newNeuron, exactCopy);
                }
            }

            Layer actionLayer = CreateClonedActionLayer(self, Layers[Layers.Count - 1], templateBrain.Layers[Layers.Count], exactCopy);
            Layers.Add(actionLayer);
            actions = actionLayer;
        }

        private void CreateDendrites(List<Neuron> aboveLayerNeurons, Neuron templateNeuron, Neuron newNeuron, bool exactCopy)
        {
            for(int d = 0; d < aboveLayerNeurons.Count; d++)
            {
                double denWeight = templateNeuron.UpstreamDendrites[d].Weight;
                if(!exactCopy)
                {
                    denWeight = ModifyBetweenNegOneAndOne(denWeight);
                }
                newNeuron.UpstreamDendrites.Add(new Dendrite(aboveLayerNeurons[d], denWeight));
            }
        }

        private double ModifyBetweenNegOneAndOne(double original)
        {
            double val = original;
            double shouldMod = Planet.World.NumberGen.NextDouble();
            if(shouldMod < ModificationRate)
            {
                double rawMod = Planet.World.NumberGen.NextDouble();
                double modification = ((rawMod * 2) - 1) * MutabilityRate;
                val += modification;
                val = ExtraMath.Clamp(val, -1, 1);
            }
            return val;
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

        private Layer CreateClonedActionLayer(Agent self, Layer aboveLayer, Layer templateLayer, bool exactCopy)
        {
            List<ActionNeuron> actionNeurons = new List<ActionNeuron>();
            int apIndex = 0;
            foreach(ActionCluster ac in self.Actions.Values)
            {
                foreach(ActionPart ap in ac.SubActions.Values)
                {
                    Neuron currNeuron = templateLayer.Neurons[apIndex];
                    double neuBias = currNeuron.Bias;
                    if(!exactCopy)
                    {
                        neuBias = ModifyBetweenNegOneAndOne(neuBias);
                    }

                    ActionNeuron neu = new ActionNeuron(ap, neuBias);
                    actionNeurons.Add(neu);
                    CreateDendrites(aboveLayer.Neurons, currNeuron, neu, exactCopy);
                    apIndex++;
                }
            }
            if(actionNeurons.Count != templateLayer.Neurons.Count)
            {
                throw new NotImplementedException("Cannot clone a brain with a different number of actions");
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

        public void ExecuteTurn()
        {
            //Detect all the things.
            self.Senses.ForEach((sc) => sc.Detect());

            //Permeate the values down the layers.
            //Each layer gathers from the layer above it.
            foreach(Layer lay in Layers)
            {
                lay.Neurons.ForEach((nn) => nn.GatherValue());
            }

            //This applies the neuron value to the underlying Action
            foreach(ActionNeuron an in actions.Neurons)
            {
                an.ApplyValue();
            }

            //This makes the agent enact those items.
            foreach(ActionCluster act in self.Actions.Values)
            {
                act.ActivateAction();
            }
        }

        public IBrain Clone(Agent self)
        {
            return new NeuralNetworkBrain(self, this, true);
        }

        public IBrain Reproduce(Agent self)
        {
            return new NeuralNetworkBrain(self, this, false);
        }

        public string ExportNewBrain()
        {
            //SPEC V2
            //Line1: Bitstream
            //Line2: NeuronNameArray

            //bitstream
            //int (number of layers) = L
            //int[L] neurons in each layer, sum = N
            //double ModificationRate
            //double MutationRate
            //double[N]  NeuronBiases
            //L double[n*pn] DendriteWeights for the neuron and parentNeurons


            byte layerCount = (byte)Layers.Count;
            byte[] layerInfoArray = new byte[layerCount+1];
            layerInfoArray[0] = layerCount;
            for(int i = 0; i < layerCount; ++i)
            {
                layerInfoArray[i + 1] = (byte)Layers[i].Neurons.Count;
            }

            List<double> doubleInfo = new List<double>();
            doubleInfo.Add(ModificationRate);
            doubleInfo.Add(MutabilityRate);

            List<double> biases = new List<double>();
            for(int j = 0; j < layerCount; ++j)
            {
                biases.AddRange(Layers[j].Neurons.Select(neu => neu.Bias));
            }
            //TODO: These could be added directly. But for now, easier to have some debuggability
            doubleInfo.AddRange(biases);

            List<double> dendriteWeights = new List<double>();
            for(int k = 0; k < layerCount; ++k)
            {
                Layer lay = Layers[k];
                for(int j = 0; j < lay.Neurons.Count; ++j) 
                {
                    Neuron n = lay.Neurons[j];
                    dendriteWeights.AddRange(n.UpstreamDendrites.Select(den => den.Weight));
                }
            }
            doubleInfo.AddRange(dendriteWeights);

            List<string> NeuronNames = new List<string>();
            for(int m = 0; m < layerCount; ++m)
            {
                NeuronNames.AddRange(Layers[m].Neurons.Select(neu => neu.Name));
            }

            StringBuilder sb = new StringBuilder();
            //sb.Append($"[{string.Join(",", layerInfoArray)}]");
            //sb.AppendLine($"[{string.Join(",", doubleInfo.ToArray())}]");
            sb.AppendLine(ConvertBrainArraysToStr(layerInfoArray, doubleInfo.ToArray()));
            sb.AppendLine(String.Join(",", NeuronNames));

            return sb.ToString();
        }

        private string ConvertBrainArraysToStr(byte[] layerInfo, double[] allOtherInfo)
        {
            int shortLength = layerInfo.Length * sizeof(byte);
            int doubleLength = allOtherInfo.Length * sizeof(double);

            byte[] asBytes = new byte[shortLength + doubleLength];
            Buffer.BlockCopy(layerInfo, 0, asBytes, 0, shortLength);
            Buffer.BlockCopy(allOtherInfo, 0, asBytes, shortLength, doubleLength);
            string s = Convert.ToBase64String(asBytes);

            return s;
        }
        
        struct BrainSpec
        {
            public byte LayerCount;
            public byte[] NeuronCounts;
            public double[] ModStats;
            public string[][] NeuronNames;
            public double[][] NeuronBiases;
            public double[][][] DendriteWeights;
        }


        private void ImportFromString(string inputString)
        {
            BrainSpec BrainSpecification = ExtractBrainInfoFromStr(inputString);

            int j = 12;
        }

        private BrainSpec ExtractBrainInfoFromStr(string exportedBrain)
        {
            string[] lines = exportedBrain.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string numeric = lines[0];

            BrainSpec output = new BrainSpec();

            byte[] inputBytes = Convert.FromBase64String(numeric);
            
            byte layerCount = inputBytes[0];
            output.LayerCount = layerCount;
            output.NeuronCounts = new ArraySegment<byte>(inputBytes, 1, layerCount).ToArray();

            output.NeuronBiases = new double[layerCount][];
            output.DendriteWeights = new double[layerCount][][];

            int streamCursor = 1 + layerCount; //This is in bytes because the preceding information are bytes!!! 

            double[] modStats = new double[2];
            Buffer.BlockCopy(inputBytes, streamCursor, modStats, 0, 2 * sizeof(double));
            streamCursor += 2 * sizeof(double);
            output.ModStats = modStats;

            for(int i = 0; i < layerCount; ++i)
            {
                int neuronCount = output.NeuronCounts[i];
                double[] neuronBiasesForLayer = new double[neuronCount];
                output.NeuronBiases[i] = neuronBiasesForLayer;

                int byteCount = neuronCount * sizeof(double);
                Buffer.BlockCopy(inputBytes, streamCursor, neuronBiasesForLayer, 0, byteCount);
                streamCursor += byteCount;
            }

            //Layer 0 (sense layer) has no dendrites
            for(int i = 1; i < layerCount; ++i)
            {
                int neuronCount = output.NeuronCounts[i];
                double[][] neuronDendriteWeightsForLayer = new double[neuronCount][];
                output.DendriteWeights[i] = neuronDendriteWeightsForLayer;

                for(int j = 0; j < neuronCount; ++j)
                {
                    int parentNodes = output.NeuronCounts[i-1];
                    double[] denWeights = new double[parentNodes];
                    neuronDendriteWeightsForLayer[j] = denWeights;

                    int byteCount = parentNodes * sizeof(double);
                    Buffer.BlockCopy(inputBytes, streamCursor, denWeights, 0, byteCount);
                    streamCursor += byteCount;
                }
            }

            string[] names = lines[1].Split(',');
            string[][] neuronNames = new string[layerCount][];
            output.NeuronNames = neuronNames;

            int nameCursor = 0;
            for(int i = 0; i < layerCount; ++i)
            {
                int layerNeuronCount = output.NeuronCounts[i];
                neuronNames[i] = new ArraySegment<string>(names, nameCursor, layerNeuronCount).ToArray();
                nameCursor += layerNeuronCount;
            }

            return output;
        }

        /// <summary>
        /// This function ensures that a brain has been cloned successfully.
        /// For a clone to be valid, the clone parent must have identical (and identically named) inputs (Senses, Properties and Statistics)
        /// to the original parent.
        /// </summary>
        /// <param name="cloneBrain">The Brain to test against.</param>
        /// <returns></returns>
        public bool CloneEquals(IBrain testBrain)
        {
            NeuralNetworkBrain cloneBrain = testBrain as NeuralNetworkBrain;
            if(cloneBrain is null)
            {
                return false;
            }    
            if(cloneBrain.MutabilityRate != this.MutabilityRate
                || cloneBrain.ModificationRate != this.ModificationRate)
            {
                return false;
            }

            Dictionary<string, double> NeuronBiases = new Dictionary<string, double>();
            Dictionary<string, double> DendriteWeights = new Dictionary<string, double>();

            // We add all the neurons in a dictionary
            // We add all the denrites in a dictionary.
            // Each Neuron should be uniquely named, each dendrite should uniquely match two neurons.
            foreach(Layer layer in Layers)
            {
                foreach(Neuron neuron in layer.Neurons)
                {
                    if(NeuronBiases.ContainsKey(neuron.Name))
                    {
                        throw new Exception("Invalid assumptions somewhere. Two neurons have the same name.");
                    }
                    NeuronBiases.Add(neuron.Name, neuron.Bias);
                    foreach(Dendrite dendrite in neuron.UpstreamDendrites)
                    {
                        string key = $"{neuron.Name}->{dendrite.TargetNeuronName}";
                        if(DendriteWeights.ContainsKey(key))
                        {
                            throw new Exception("Invalid assumption somewhere. Each dendrites should be unique");
                        }
                        DendriteWeights.Add(key, dendrite.Weight);
                    }
                }
            }

            //Now we iteratively remove the elements of the dictionaries while examining the second brain
            //If there are any mismatches, they aren't clones.
            foreach(Layer cloneLayer in cloneBrain.Layers)
            {
                foreach(Neuron cloneNeuron in cloneLayer.Neurons)
                {
                    if(!NeuronBiases.ContainsKey(cloneNeuron.Name)
                        || NeuronBiases[cloneNeuron.Name] != cloneNeuron.Bias)
                    {
                        //Neuron doesn't exist in original or doesn't match.
                        return false;
                    }

                    //Remove it. It's been matched.
                    NeuronBiases.Remove(cloneNeuron.Name);

                    foreach(Dendrite cloneDendrite in cloneNeuron.UpstreamDendrites)
                    {
                        string cloneKey = $"{cloneNeuron.Name}->{cloneDendrite.TargetNeuronName}";
                        if(!DendriteWeights.ContainsKey(cloneKey)
                            || DendriteWeights[cloneKey] != cloneDendrite.Weight)
                        {
                            //Dendrite doesn't exist in original or doesn't match
                            return false;
                        }

                        //Remove it, it's been matched.
                        DendriteWeights.Remove(cloneKey);
                    }
                }
            }

            if(DendriteWeights.Count > 0
                || NeuronBiases.Count > 0)
            {
                //There are extra dendrites or extra neurons
                return false;
            }


            return true;
        }
    }
}
