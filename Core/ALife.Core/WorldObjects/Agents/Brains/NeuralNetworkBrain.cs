using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;
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
            // public double ModificationRate;
            // public double MutabilityRate;
            // public List<Layer> Layers = new List<Layer>();
            // private Layer actions;

            //BRAIN! {ModificationRate} {MutabilityRate}

            StringBuilder result = new StringBuilder();

            string topLevelBrainInfo = $"Brain: ModR: { ModificationRate} MutR: { MutabilityRate}";
            
            result.AppendLine(topLevelBrainInfo);

            for(int i = 0; i < Layers.Count; ++i)
            {
                //Build dictionary of the names of the parent nodes.
                //The dictionary will be null if it is the first layer. Inelegant... yes.
                Dictionary<string, int> neuronNameToID = null;
                if(i != 0)
                {
                    neuronNameToID = new Dictionary<string, int>();
                    for(int j = 0; j < Layers[i-1].Neurons.Count; ++j)
                    {
                        Neuron n = Layers[i-1].Neurons[j];
                        neuronNameToID.Add(n.Name, j);
                    }
                }

                result.Append(Layers[i].ExportNewBrain_Layer(neuronNameToID));
            }

            return result.ToString();
        }
    }
}
