namespace ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains
{
    internal struct NeuralNetworkBrainImport
    {
        public byte LayerCount;
        public byte[] NeuronCounts;
        public double[] ModStats;
        public string[][] NeuronNames;
        public double[][] NeuronBiases;
        public double[][][] DendriteWeights;
    }
}
