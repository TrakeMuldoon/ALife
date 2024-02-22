using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ALife.Core.WorldObjects.Agents.Brains
{
    public class NeuralNetworkBrain : IBrain
    {
        private Agent self;
        public double ModificationRate;
        public double MutabilityRate;
        public List<Layer> Layers = new List<Layer>();
        private Layer actions;

        public NeuralNetworkBrain(Agent self, List<int> hiddenLayerNeuronCounts)
            : this(self, 0.70, 0.005, hiddenLayerNeuronCounts)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self">The parent</param>
        /// <param name="modificationRate">The rate at which dendrite weights or neuron bias' get modified in Repro</param>
        /// <param name="mutabilityRate">The maximum percentage to which they will be modified</param>
        /// <param name="layerNeuronCounts">the neuron counts for the hidden layers</param>
        public NeuralNetworkBrain(Agent self, double modificationRate, double mutabilityRate, List<int> hiddenLayerNeuronCounts)
        {
            if(hiddenLayerNeuronCounts is null
               || hiddenLayerNeuronCounts.Count < 1)
            {
                throw new ArgumentOutOfRangeException("Not enough layers for a Neural Network Brain");
            }
            this.self = self;
            this.ModificationRate = modificationRate;
            this.MutabilityRate = mutabilityRate;

            this.Layers = NeuralNetworkBrainFactory.CreateRandomNeuralNetwork(self, hiddenLayerNeuronCounts);
            this.actions = Layers[Layers.Count - 1];
        }

        private NeuralNetworkBrain(Agent self, NeuralNetworkBrain templateBrain, bool exactCopy)
        {
            this.self = self;
            this.ModificationRate = templateBrain.ModificationRate;
            this.MutabilityRate = templateBrain.MutabilityRate;

            if(exactCopy)
            {
                this.Layers = NeuralNetworkBrainFactory.CreateClonedNeuralNetwork(self, templateBrain);
            }
            else
            {
                this.Layers = NeuralNetworkBrainFactory.CreateEvolvedNeuralNetwork(self, templateBrain);
            }

            this.actions = Layers[Layers.Count - 1];
        }

        //TODO: Make Private?
        public NeuralNetworkBrain(Agent self, string inputString)
        {
            this.self = self;

            NeuralNetworkBrainImport BrainSpecification = ExtractBrainInfoFromStr(inputString);

            this.ModificationRate = BrainSpecification.ModStats[0];
            this.MutabilityRate = BrainSpecification.ModStats[1];

            this.Layers = NeuralNetworkBrainFactory.CreateBrainSpecNeuralNetwork(self, BrainSpecification);
            this.actions = Layers[Layers.Count - 1];
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

            //Item 1 in the double bitstream is ModificationRate
            //Item 2 in the double bitstream is MutabilityRate
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
            //Uncomment these lines if debugging is necessary
            //sb.Append($"[{string.Join(",", layerInfoArray)}]");
            //sb.AppendLine($"[{string.Join(",", doubleInfo.ToArray())}]");
            sb.AppendLine(ConvertBrainArraysToStr(layerInfoArray, doubleInfo.ToArray()));
            sb.AppendLine(String.Join(",", NeuronNames));

            return sb.ToString();
        }

        private string ConvertBrainArraysToStr(byte[] layerInfo, double[] allOtherInfo)
        {
            int shortLength = layerInfo.Length * sizeof(byte); //TODO: replace with constant value + comment
            int doubleLength = allOtherInfo.Length * sizeof(double);

            byte[] asBytes = new byte[shortLength + doubleLength];
            Buffer.BlockCopy(layerInfo, 0, asBytes, 0, shortLength);
            Buffer.BlockCopy(allOtherInfo, 0, asBytes, shortLength, doubleLength);
            string s = Convert.ToBase64String(asBytes);

            return s;
        }

        private NeuralNetworkBrainImport ExtractBrainInfoFromStr(string exportedBrain)
        {
            string[] lines = exportedBrain.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string numeric = lines[0];

            NeuralNetworkBrainImport output = new NeuralNetworkBrainImport();

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


            //TODO: Exctract the three tasks into Functions
            //Get Neurons and Neuron Biases
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
            //Get Dendrites
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

            //Get Names
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
            //Checking properties
            if(cloneBrain.MutabilityRate != this.MutabilityRate
                || cloneBrain.ModificationRate != this.ModificationRate)
            {
                return false;
            }

            Dictionary<string, double> NeuronBiases = new Dictionary<string, double>();
            Dictionary<string, double> DendriteWeights = new Dictionary<string, double>();

            //TODO: Move To HelperMethod
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

            //TODO: Move To HelperMethod
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
