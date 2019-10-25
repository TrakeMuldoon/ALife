using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Brains.BehaviourBrainPieces
{
    public class BehaviourInputCabinet
    {
        public BehaviourInputCabinet(Agent parent)
        {
            foreach(SenseCluster sc in parent.Senses)
            {

            }
            foreach(PropertyInput pi in parent.Properties.Values)
            {

            }
            foreach(Action act in parent.Actions.Values)
            {

            }
        }

        public BehaviourInput GetBehaviourInputByName(string name)
        {
            throw new NotImplementedException();
        }
        public BehaviourInput GetRandomBehaviourInputByType(Type type)
        {
            throw new NotImplementedException();
        }
        public BehaviourInput GetRandomBehaviourInput()
        {
            throw new NotImplementedException();
        }
    }
}
