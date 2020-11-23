using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class EmptyObject : WorldObject
    {
        public EmptyObject(Point centrePoint, float startRadius, string collisionLevel) : this(centrePoint, startRadius, collisionLevel, String.Empty)
        {
        }
        public EmptyObject(Point centrePoint, float startRadius, string collisionLevel, string name) : base(centrePoint, startRadius, "Empty", name, collisionLevel, Colors.Gray)
        {
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteAliveTurn()
        {
           //Do Nothing
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Reproduce(bool exactCopy)
        {
            throw new NotImplementedException();
        }
    }
}
