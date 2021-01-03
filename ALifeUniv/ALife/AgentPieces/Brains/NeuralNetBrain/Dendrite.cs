﻿using System;
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

        public Dendrite()
        {
            Weight = Planet.World.NumberGen.NextDouble();
        }

        public Dendrite(double weight)
        {
            Weight = weight;
        }
    }
}
