using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.NeuralNetBrain
{
    class FuncNeuron : Neuron
    {
        Func<double> GetValue;

        private double theVal;

        public FuncNeuron(String name, Func<double> getValueFunction)
            : base(name)
        {
            GetValue = getValueFunction;
        }

        public override double Value
        {
            get
            {
                return theVal;
            }
            set
            {
                throw new Exception("Do Not Set FuncNeuron Value");
            }
        }

        public void RefreshValue()
        {
            theVal = GetValue();
        }
    }
}
