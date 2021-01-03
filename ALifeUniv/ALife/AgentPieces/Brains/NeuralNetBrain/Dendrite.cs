using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains
{
    public class Dendrite
    {
        public double Weight { get; set; }
        public Neuron TargetNeuron { get; set; }

        public double CurrentValue
        {
            get
            {
                return TargetNeuron.Value * Weight;
            }

        }

        public Dendrite(Neuron targetNeuron)
        {
            TargetNeuron = targetNeuron;
            Weight = Planet.World.NumberGen.NextDouble();
        }

        //public Dendrite(double weight)
        //{
        //    Weight = weight;
        //}
    }
}
