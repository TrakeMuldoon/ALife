using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Brains
{
    public class Neuron
    {
        public List<Dendrite> UpstreamDendrites { get; set; }
        public String Name { get; protected set; }
        public double Bias { get; set; }

        public virtual double Value
        {
            get;
            set;
        }

        public Neuron(string name)
            : this(name, Planet.World.NumberGen.NextDouble())
        {
        }

        private Neuron(string name, double bias)
        {
            this.UpstreamDendrites = new List<Dendrite>();
            bias = (bias * 2) - 1;
            if(bias < -1.0 || bias > 1.0)
            {
                throw new ArgumentOutOfRangeException("Bias must be between 0 and 1");
            }
            this.Bias = bias;
            this.Name = name;
        }

        public virtual void GatherValue()
        {
            Value = 0;
            foreach(Dendrite upDen in UpstreamDendrites)
            {
                //Apply all the values;
                Value += upDen.CurrentValue;
            }
            //Sigmoid it
            Value = Sigmoid(Value + Bias);
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public virtual Neuron Clone()
        {
            throw new NotImplementedException("clone no worky yet");
        }
    }
}
