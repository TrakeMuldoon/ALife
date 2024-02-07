using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    internal class NeuralNetworkBrainFactory
    {
        public NeuralNetworkBrain CreateNNBrain(Agent self, double modificationRate, double mutabilityRate, List<int> layerNeuronCounts)
        {
            return null;
        }

        public NeuralNetworkBrain CreateClonedBrain(Agent self, NeuralNetworkBrain existingBrain)
        {
            return null;
        }

        public NeuralNetworkBrain CreateEvolvedBrain(Agent self, NeuralNetworkBrain existingBrain)
        {
            return null;
        }

        public NeuralNetworkBrain CreateFromBrainSpec(Agent self, NeuralNetworkBrainImport brainSpec)
        {
            return null;
        }



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
