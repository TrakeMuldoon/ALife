using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects.Agents.AgentActions;
using ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains;
using ALife.Core.WorldObjects.Agents.Senses;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Brains.FlatNeuronBrain
{
    /// <summary>
    /// Flat-array implementation of a layered feed-forward neural network brain.
    /// Designed to be functionally bit-identical to <see cref="NeuralNetworkBrain"/>
    /// when given the same RNG seed: same construction iteration order, same forward-pass
    /// accumulation order, same sigmoid formula.
    /// </summary>
    public class FlatNeuralNetworkBrain : IBrain
    {
        private readonly Agent _self;
        public double ModificationRate;
        public double MutabilityRate;

        // Layer structure
        private int _layerCount;
        private int[] _layerStart;     // first global neuron index per layer
        private int[] _layerEnd;       // exclusive end per layer
        private int _totalNeurons;

        // Per-neuron data (indexed by global neuron id 0.._totalNeurons-1)
        private double[] _values;      // current activation
        private double[] _biases;      // bias (0 for sense layer)
        private string[] _names;       // for export / clone-equality
        private double[][] _weights;   // _weights[id] = incoming weights array (null for sense)
        private int[][] _sourceIds;    // _sourceIds[id] = global ids of upstream neurons (null for sense)

        // External adapters
        private FuncNeuron[] _senseNeurons;   // sense layer FuncNeurons (drive _values for sense indices)
        private ActionPart[] _actionParts;    // action layer ActionParts (receive _values for action indices)

        public FlatNeuralNetworkBrain(Agent self, List<int> hiddenLayerNeuronCounts)
            : this(self, 0.70, 0.005, hiddenLayerNeuronCounts) { }

        public FlatNeuralNetworkBrain(Agent self, double modificationRate, double mutabilityRate, List<int> hiddenLayerNeuronCounts)
        {
            if(hiddenLayerNeuronCounts is null || hiddenLayerNeuronCounts.Count < 1)
            {
                throw new ArgumentOutOfRangeException("Not enough layers for a Neural Network Brain");
            }
            _self = self;
            ModificationRate = modificationRate;
            MutabilityRate = mutabilityRate;
            BuildRandom(hiddenLayerNeuronCounts);
        }

        private FlatNeuralNetworkBrain(Agent self, FlatNeuralNetworkBrain template, bool exactCopy)
        {
            _self = self;
            ModificationRate = template.ModificationRate;
            MutabilityRate = template.MutabilityRate;
            if(exactCopy)
            {
                BuildClone(template);
            }
            else
            {
                BuildEvolved(template);
            }
        }

        private void BuildRandom(List<int> hiddenLayerNeuronCounts)
        {
            List<FuncNeuron> senseList = BuildSenseNeurons();
            List<ActionPart> actionList = BuildActionPartList();

            int senseCount = senseList.Count;
            int hiddenCount = hiddenLayerNeuronCounts.Count;
            int actionCount = actionList.Count;

            _layerCount = 1 + hiddenCount + 1;
            _layerStart = new int[_layerCount];
            _layerEnd = new int[_layerCount];

            int cursor = 0;
            _layerStart[0] = 0;
            _layerEnd[0] = senseCount;
            cursor = senseCount;

            for(int h = 0; h < hiddenCount; ++h)
            {
                _layerStart[h + 1] = cursor;
                _layerEnd[h + 1] = cursor + hiddenLayerNeuronCounts[h];
                cursor = _layerEnd[h + 1];
            }

            int actionLayerIdx = _layerCount - 1;
            _layerStart[actionLayerIdx] = cursor;
            _layerEnd[actionLayerIdx] = cursor + actionCount;
            _totalNeurons = _layerEnd[actionLayerIdx];

            _values = new double[_totalNeurons];
            _biases = new double[_totalNeurons];
            _names = new string[_totalNeurons];
            _weights = new double[_totalNeurons][];
            _sourceIds = new int[_totalNeurons][];

            // Sense layer: names from FuncNeurons; biases stay 0
            _senseNeurons = senseList.ToArray();
            for(int i = 0; i < senseCount; ++i)
            {
                _names[i] = senseList[i].Name;
            }

            // Hidden layer names
            for(int h = 0; h < hiddenCount; ++h)
            {
                int layerIdx = h + 1;
                int start = _layerStart[layerIdx];
                int count = hiddenLayerNeuronCounts[h];
                for(int j = 0; j < count; ++j)
                {
                    _names[start + j] = $"HN:{layerIdx}.{j + 1}";
                }
            }

            // Action layer names + ActionPart adapters
            _actionParts = actionList.ToArray();
            int actionStart = _layerStart[actionLayerIdx];
            for(int i = 0; i < actionCount; ++i)
            {
                _names[actionStart + i] = actionList[i].Name;
            }

            // RNG-CRITICAL: biases generated in NeuralNetworkBrainFactory's order
            //   - hidden layers in order, neurons in order; then action layer neurons in order
            for(int layerIdx = 1; layerIdx < _layerCount; ++layerIdx)
            {
                int start = _layerStart[layerIdx];
                int end = _layerEnd[layerIdx];
                for(int j = start; j < end; ++j)
                {
                    _biases[j] = (Planet.World.NumberGen.NextDouble() * 2) - 1;
                }
            }

            // RNG-CRITICAL: dendrite weights generated in NeuralNetworkBrainFactory's order
            //   - layer 1..count, target neuron in order, parent neuron in parent-layer order
            for(int layerIdx = 1; layerIdx < _layerCount; ++layerIdx)
            {
                int start = _layerStart[layerIdx];
                int end = _layerEnd[layerIdx];
                int parentStart = _layerStart[layerIdx - 1];
                int parentCount = _layerEnd[layerIdx - 1] - parentStart;

                for(int j = start; j < end; ++j)
                {
                    double[] weights = new double[parentCount];
                    int[] sources = new int[parentCount];
                    for(int p = 0; p < parentCount; ++p)
                    {
                        weights[p] = (Planet.World.NumberGen.NextDouble() * 2) - 1;
                        sources[p] = parentStart + p;
                    }
                    _weights[j] = weights;
                    _sourceIds[j] = sources;
                }
            }
        }

        private void BuildClone(FlatNeuralNetworkBrain template)
        {
            // Sense layer rebuilt from agent (which presumably matches template by name)
            List<FuncNeuron> senseList = BuildSenseNeurons();
            VerifySenseLayerMatches(senseList, template);
            List<ActionPart> actionList = BuildActionPartList();

            _layerCount = template._layerCount;
            _layerStart = (int[])template._layerStart.Clone();
            _layerEnd = (int[])template._layerEnd.Clone();
            _totalNeurons = template._totalNeurons;

            _values = new double[_totalNeurons];
            _biases = (double[])template._biases.Clone();
            _names = (string[])template._names.Clone();
            _weights = new double[_totalNeurons][];
            _sourceIds = new int[_totalNeurons][];
            for(int i = 0; i < _totalNeurons; ++i)
            {
                if(template._weights[i] != null)
                {
                    _weights[i] = (double[])template._weights[i].Clone();
                    _sourceIds[i] = (int[])template._sourceIds[i].Clone();
                }
            }

            _senseNeurons = senseList.ToArray();
            _actionParts = actionList.ToArray();
        }

        private void BuildEvolved(FlatNeuralNetworkBrain template)
        {
            List<FuncNeuron> senseList = BuildSenseNeurons();
            VerifySenseLayerMatches(senseList, template);
            List<ActionPart> actionList = BuildActionPartList();

            _layerCount = template._layerCount;
            _layerStart = (int[])template._layerStart.Clone();
            _layerEnd = (int[])template._layerEnd.Clone();
            _totalNeurons = template._totalNeurons;

            _values = new double[_totalNeurons];
            _biases = new double[_totalNeurons];
            _names = (string[])template._names.Clone();
            _weights = new double[_totalNeurons][];
            _sourceIds = new int[_totalNeurons][];

            // RNG-CRITICAL: biases evolved in NeuralNetworkBrainFactory's order
            for(int layerIdx = 1; layerIdx < _layerCount; ++layerIdx)
            {
                int start = _layerStart[layerIdx];
                int end = _layerEnd[layerIdx];
                for(int j = start; j < end; ++j)
                {
                    _biases[j] = EvolveBetweenNegOneAndOne(template._biases[j]);
                }
            }

            // RNG-CRITICAL: weights evolved in NeuralNetworkBrainFactory's order
            for(int layerIdx = 1; layerIdx < _layerCount; ++layerIdx)
            {
                int start = _layerStart[layerIdx];
                int end = _layerEnd[layerIdx];
                for(int j = start; j < end; ++j)
                {
                    double[] templateWeights = template._weights[j];
                    int[] templateSources = template._sourceIds[j];
                    int count = templateWeights.Length;
                    double[] newWeights = new double[count];
                    int[] newSources = (int[])templateSources.Clone();
                    for(int p = 0; p < count; ++p)
                    {
                        newWeights[p] = EvolveBetweenNegOneAndOne(templateWeights[p]);
                    }
                    _weights[j] = newWeights;
                    _sourceIds[j] = newSources;
                }
            }

            _senseNeurons = senseList.ToArray();
            _actionParts = actionList.ToArray();
        }

        private double EvolveBetweenNegOneAndOne(double original)
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

        private List<FuncNeuron> BuildSenseNeurons()
        {
            List<FuncNeuron> senseNeurons = new List<FuncNeuron>();
            foreach(SenseCluster sc in _self.Senses)
            {
                foreach(SenseInput si in sc.SubInputs)
                {
                    senseNeurons.AddRange(FuncNeuronFactory.GenerateFuncNeuronsForSenseInput(si));
                }
            }
            return senseNeurons;
        }

        private List<ActionPart> BuildActionPartList()
        {
            List<ActionPart> parts = new List<ActionPart>();
            foreach(ActionCluster ac in _self.Actions.Values)
            {
                foreach(ActionPart ap in ac.SubActions.Values)
                {
                    parts.Add(ap);
                }
            }
            return parts;
        }

        private void VerifySenseLayerMatches(List<FuncNeuron> newSenseList, FlatNeuralNetworkBrain template)
        {
            int expected = template._layerEnd[0];
            if(newSenseList.Count != expected)
            {
                throw new Exception("Cannot evolve or clone an agent with a different number of senses.");
            }
            for(int i = 0; i < expected; ++i)
            {
                if(newSenseList[i].Name != template._names[i])
                {
                    throw new Exception($"Cannot evolve or clone an agent with a different sense layout. Missing {template._names[i]}");
                }
            }
        }

        public void ExecuteTurn()
        {
            // Match NeuralNetworkBrain.ExecuteTurn: detect, gather (all layers), apply actions, activate.
            for(int i = 0; i < _self.Senses.Count; ++i)
            {
                _self.Senses[i].Detect();
            }

            // Sense layer: pull values from FuncNeurons into _values
            int senseEnd = _layerEnd[0];
            for(int i = 0; i < senseEnd; ++i)
            {
                _senseNeurons[i].GatherValue();
                _values[i] = _senseNeurons[i].Value;
            }

            // Hidden + action layers: dot-product + sigmoid
            for(int layerIdx = 1; layerIdx < _layerCount; ++layerIdx)
            {
                int start = _layerStart[layerIdx];
                int end = _layerEnd[layerIdx];
                for(int j = start; j < end; ++j)
                {
                    double[] weights = _weights[j];
                    int[] sources = _sourceIds[j];
                    double sum = 0;
                    for(int p = 0; p < weights.Length; ++p)
                    {
                        sum += _values[sources[p]] * weights[p];
                    }
                    _values[j] = Sigmoid(sum + _biases[j]);
                }
            }

            // Apply action neuron values to ActionParts (mirrors ActionNeuron.ApplyValue)
            int actionStart = _layerStart[_layerCount - 1];
            for(int i = 0; i < _actionParts.Length; ++i)
            {
                _actionParts[i].Intensity = _values[actionStart + i];
            }

            // Activate actions
            foreach(ActionCluster act in _self.Actions.Values)
            {
                act.ActivateAction();
            }
        }

        private static double Sigmoid(double x)
        {
            return ((1 / (1 + Math.Exp(-x))) * 2) - 1;
        }

        public IBrain Clone(Agent self) => new FlatNeuralNetworkBrain(self, this, true);

        public IBrain Reproduce(Agent self) => new FlatNeuralNetworkBrain(self, this, false);

        public bool CloneEquals(IBrain testBrain)
        {
            if(testBrain is not FlatNeuralNetworkBrain other)
            {
                return false;
            }
            if(other.MutabilityRate != MutabilityRate)
            {
                return false;
            }
            if(other.ModificationRate != ModificationRate)
            {
                return false;
            }
            if(other._totalNeurons != _totalNeurons || other._layerCount != _layerCount)
            {
                return false;
            }
            for(int i = 0; i < _totalNeurons; ++i)
            {
                if(_biases[i] != other._biases[i]) return false;
                if(_names[i] != other._names[i]) return false;
                bool weightsNullA = _weights[i] is null;
                bool weightsNullB = other._weights[i] is null;
                if(weightsNullA != weightsNullB) return false;
                if(weightsNullA) continue;
                if(_weights[i].Length != other._weights[i].Length) return false;
                for(int p = 0; p < _weights[i].Length; ++p)
                {
                    if(_weights[i][p] != other._weights[i][p]) return false;
                    if(_sourceIds[i][p] != other._sourceIds[i][p]) return false;
                }
            }
            return true;
        }

        public string ExportNewBrain()
        {
            throw new NotImplementedException("Serialization for FlatNeuralNetworkBrain not yet implemented.");
        }
    }
}
