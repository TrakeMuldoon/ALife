using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.NeuralNetBrain
{
    public class Neuron
    {
        public List<Dendrite> Dendrites { get; set; }
        public double Bias { get; set; }
        public double Delta { get; set; }
        public virtual double Value { 
            get; 
            set; 
        }

        public int DendriteCount
        {
            get
            {
                return Dendrites.Count;
            }
        }

        public Neuron()
        {
            this.Bias = Planet.World.NumberGen.NextDouble();

            this.Dendrites = new List<Dendrite>();
        }
    }
}
