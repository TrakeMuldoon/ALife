using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public class ProximityInput : SenseInput<bool>
    {
        public ProximityInput(string name) : base(name)
        {
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            Value = collisions.Count > 0;
        }
    }
}
