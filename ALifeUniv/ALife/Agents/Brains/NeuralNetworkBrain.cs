using ALifeUni.ALife.Agents.AgentActions;
using ALifeUni.ALife.Agents.Brains.NeuralNetworkBrains;
using ALifeUni.ALife.Agents.Senses;
using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Agents.Brains
{
    class NeuralNetworkBrain : IBrain
    {
        private Agent self;
        public double ModificationRate;
        public double MutabilityRate;
        public List<Layer> Layers = new List<Layer>();
        private Layer actions;

        public NeuralNetworkBrain(Agent self, List<int> layers)
            : this(self, 0.7, 0.005, layers)
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
                        neuBias = ModifyANumber(neuBias);
                    }

                    //Add a neuron
                    Neuron neu = new Neuron("HN:" + (i + 1) + "." + (n + 1), neuBias);
                    newLayer.Neurons.Add(neu);
                    for(int d = 0; d < aboveLayerNeurons.Count; d++)
                    {
                        double denWeight = templateCurrLayerNeurons[n].UpstreamDendrites[d].Weight;
                        if(!exactCopy)
                        {
                            denWeight = ModifyANumber(denWeight);
                        }
                        neu.UpstreamDendrites.Add(new Dendrite(aboveLayerNeurons[d], denWeight));
                    }
                }
            }

            Layer actionLayer = CreateClonedActionLayer(self, Layers[Layers.Count - 1], templateBrain.Layers[Layers.Count], exactCopy);
            Layers.Add(actionLayer);
            actions = actionLayer;
        }

        private double ModifyANumber(double original)
        {
            double val = original;
            double shouldMod = Planet.World.NumberGen.NextDouble();
            if(shouldMod < ModificationRate)
            {
                double rawMod = Planet.World.NumberGen.NextDouble();
                double modification = ((rawMod * 2) - 1) * MutabilityRate;
                val += modification;
                val = Math.Clamp(val, -1, 1);
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
                        neuBias = ModifyANumber(neuBias);
                    }

                    ActionNeuron neu = new ActionNeuron(ap, neuBias);
                    actionNeurons.Add(neu);
                    for(int d = 0; d < aboveLayer.Neurons.Count; d++)
                    {
                        double denWeight = currNeuron.UpstreamDendrites[d].Weight;
                        if(!exactCopy)
                        {
                            denWeight = ModifyANumber(denWeight);
                        }
                        neu.UpstreamDendrites.Add(new Dendrite(aboveLayer.Neurons[d], denWeight));
                    }
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


        //public bool Train(List<double> input, List<double> output)
        //{
        //    if((input.Count != this.Layers[0].Neurons.Count)
        //        || (output.Count != this.Layers[this.Layers.Count - 1].Neurons.Count))
        //    {
        //        return false;
        //    }

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
    }
}
