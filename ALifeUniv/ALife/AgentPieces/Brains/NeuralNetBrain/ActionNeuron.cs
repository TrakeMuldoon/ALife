using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains
{
    public class ActionNeuron : Neuron
    {
        public ActionNeuron(ActionPart actionPart) : base(actionPart.Name)
        {
            Activity = actionPart;
        }

        public readonly ActionPart Activity;

        public void ApplyValue()
        {
            Activity.Intensity = Value;
        }
    }
}
