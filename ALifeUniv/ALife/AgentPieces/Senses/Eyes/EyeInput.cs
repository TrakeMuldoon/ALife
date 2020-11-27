using System.Collections.Generic;

namespace ALifeUni.ALife.Inputs.SenseClusters
{
    public class EyeInput : SenseInput<bool>
    {
        public EyeInput(string name) : base(name)
        {
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            Value = collisions.Count > 0;
        }
    }
}
