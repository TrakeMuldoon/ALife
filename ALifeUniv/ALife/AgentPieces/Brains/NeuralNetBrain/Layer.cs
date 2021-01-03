using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }

        public Layer(int numNeurons)
        {
            Neurons = new List<Neuron>(numNeurons);
        }
    }
}
