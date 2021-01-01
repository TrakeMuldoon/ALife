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

        public FuncNeuron(Func<double> getValueFunction)
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
                throw new Exception("Do Not Sense FuncNeuron Value");
            }
        }

        public void RefreshValue()
        {
            theVal = GetValue();
        }
    }
}
