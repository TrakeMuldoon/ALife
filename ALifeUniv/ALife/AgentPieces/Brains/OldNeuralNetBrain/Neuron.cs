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
        public String Name { get; protected set; }
        public double Bias { get; set; }

        public double Delta { get; set; } //TODO: Delete Delta, it isn't used in my version
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

        public Neuron(string name)
        {
            this.Bias = Planet.World.NumberGen.NextDouble();

            this.Dendrites = new List<Dendrite>();
            this.Name = name;
        }

        public virtual Neuron Clone()
        {
            Neuron neu = new Neuron(Name);
            neu.Bias = Bias;
            neu.Value = Value;
            foreach(Dendrite d in Dendrites)
            {
                neu.Dendrites.Add(new Dendrite(d.Weight));
            }
            return neu;
        }
    }
}
