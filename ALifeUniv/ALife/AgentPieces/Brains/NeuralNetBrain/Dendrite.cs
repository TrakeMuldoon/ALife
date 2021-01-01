using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.NeuralNetBrain
{
    public class Dendrite
    {
        public double Weight { get; set; }

        public Dendrite()
        {
            this.Weight = Planet.World.NumberGen.NextDouble();
        }
    }
}
